using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using JYL;
using SHG;

namespace JWS
{
    /// <summary>
    /// 치료실 탭 전체 UI 컨트롤러
    /// - 슬롯 8개 A~H 상태 갱신/클릭 처리
    /// - 빈 슬롯: 부상자 목록(배치) 패널 오픈
    /// - 배치된 슬롯: 남은 기간이 1주 이내면 교체 패널 오픈, 아니면 무반응
    /// - MedicalCenter 단계/슬롯수 변화에 따라 UI 자동 갱신
    /// - InjureListPanel과 이벤트 연동 (배치하기 / 해제 / 리셋 / 확정)
    /// </summary>
    public class TreatmentRoomTabView : MonoBehaviour
    {
        // ------------------------------
        // [필드 및 DI]
        // ------------------------------
        [SerializeField] private List<TreatmentSlotView> slots; // Slot A~H 순서대로
        [Inject] private DomAthService athleteService;
        [Inject] private IFacilitiesController facilitiesController;
        [Inject] private SaveManager saveManager;

        [SerializeField] private GameObject injuredAthleteInfoPUI; // 팝업 루트
        [SerializeField] private InjureListPanel injureListPanel;  // 부상자 목록 패널
        [SerializeField] private InjureAthInfoPanel injureAthInfoPanel; // 상세 스탯 패널

        private CompositeDisposable _enableCd;                 // 활성화 기간 구독 모음
        private readonly CompositeDisposable _wireCd = new();  // 고정 배선(슬롯 클릭 등) 구독 모음
        private IDisposable _panelSubs;                        // 부상자 목록 패널 이벤트 구독 모음

        // 슬롯별 배치 현황(세이브/로드 대상)
        // 커밋(실제 슬롯)
        private readonly DomAthEntity[] _assigned = new DomAthEntity[8];
        // 초안(패널 열렸을 때만)
        private DomAthEntity[] _draftAssigned;

        // 세이브/로드 용: 슬롯에 배치된 선수 id들 (A→H 순)
        private List<int> _savedAssignedIds = new();

        // ============================================================
        // [1] 고정 배선: Awake()
        // ============================================================
        private void Awake()
        {
            // 슬롯 클릭 → 빈 슬롯 / 배치 슬롯 분기 처리
            for (int i = 0; i < slots.Count; i++)
            {
                int idx = i; // 클로저 캡처
                slots[idx].Clicked
                    .Subscribe(_ =>
                    {
                        // 잠금/배치 불가 상태는 무시
                        if (slots[idx].IsLocked || slots[idx].IsNoAvailable) return;

                        // 빈 슬롯 클릭 → 배치 패널 열기
                        if (_assigned[idx] == null)
                        {
                            OpenAssignPanel(idx);
                        }
                        // 배치된 슬롯 클릭 → 남은 치료 기간 확인 후 교체 여부 판단
                        else
                        {
                            HandleAssignedSlot(idx);
                        }
                    })
                    .AddTo(_wireCd);
            }
        }

        // ============================================================
        // [2] OnEnable() : 탭이 열릴 때 초기화
        // ============================================================
        private void OnEnable()
        {
            _enableCd = new CompositeDisposable();

            // 초기 UI 상태 리셋 (팝업/리스트/상세 패널 모두 OFF)
            if (injuredAthleteInfoPUI) injuredAthleteInfoPUI.SetActive(false);
            if (injureListPanel)       injureListPanel.gameObject.SetActive(false);
            if (injureAthInfoPanel)    injureAthInfoPanel.gameObject.SetActive(false);

            // 슬롯 UI 최초 갱신
            LoadAssignedFromSave();
            Refresh();

            // MedicalCenter 단계/수용 인원 변화시 즉시 Refresh()
            var mc = facilitiesController.MedicalCenter;
            mc.CurrentStage     .Subscribe(_ => Refresh()).AddTo(_enableCd);
            mc.NumberOfAthletes .Subscribe(_ => Refresh()).AddTo(_enableCd);
        }

        // ============================================================
        // [3] 빈 슬롯 클릭 → 배치 패널 열기
        // ============================================================
        private void OpenAssignPanel(int slotIndex)
        {
            var injuredAll = athleteService.GetAllRecruitedAthleteList()
                .Where(a => a.curState == AthleteState.Injured).ToList();
            if (injuredAll.Count == 0) { ShowNoCandidateHint(); return; }

            injuredAthleteInfoPUI.SetActive(true);      // 팝업 루트(=블로커) 켜기
            injureListPanel.gameObject.SetActive(true);
            if (injureAthInfoPanel) injureAthInfoPanel.gameObject.SetActive(false);

            _draftAssigned = (DomAthEntity[])_assigned.Clone();
            // 리스트 패널 열기 (현재 배치된 선수들은 배치중 상태로 표시됨)
            injureListPanel.Open(injuredAll, GetAssignedIdSet(draft:true));
            
            // 패널 이벤트 핸들러 연결
            WireListPanelHandlers(); // ★ 여기
        }
        
        private HashSet<int> GetAssignedIdSet(bool draft = false)
        {
            var src = draft && _draftAssigned != null ? _draftAssigned : _assigned;
            return src.Where(a => a != null).Select(a => a.id).ToHashSet();
        }

        // ============================================================
        // [4] 배치된 슬롯 클릭 → 남은 치료 기간 분기
        // ============================================================
        private void HandleAssignedSlot(int slotIndex)
        {
            var ath = _assigned[slotIndex];
            if (ath == null) return;

            // 치료기간이 2주 이상 남으면 교체 불가
            if (ath.leftInjury > 1)
            {
                Debug.Log("남은 치료 기간이 2주 이상 → 교체 불가");
                return;
            }

            // 1주 이내 → 교체 허용
            OpenReplacePanel(slotIndex);
        }

        // ------------------------------
        // [5] 교체 패널 열기 (배치 패널과 동일 흐름, 선택 시 교체)
        // ------------------------------
        private void OpenReplacePanel(int slotIndex)
        {
            var injuredAll = athleteService.GetAllRecruitedAthleteList()
                .Where(a => a.curState == AthleteState.Injured).ToList();
            if (injuredAll.Count == 0) { ShowNoCandidateHint(); return; }

            injuredAthleteInfoPUI.SetActive(true);
            injureListPanel.gameObject.SetActive(true);
            if (injureAthInfoPanel) injureAthInfoPanel.gameObject.SetActive(false);

            injureListPanel.Open(injuredAll, GetAssignedIdSet());
            WireListPanelHandlers();
        }

        // ------------------------------
        // [6] UI 전체 갱신 (데이터 → 슬롯 상태 반영)
        //     - 개방 슬롯 수 계산
        //     - 뒤에서부터 잠금(ShowLocked)
        //     - 저장된 배치 반영
        //     - 남은 슬롯: NoAvailable/Empty 판단
        // ------------------------------
        private void Refresh()
        {
            var all = athleteService.GetAllRecruitedAthleteList() ?? new List<DomAthEntity>();
            var byId = all.ToDictionary(a => a.id, a => a);
            var injuredAll = all.Where(a => a.curState == AthleteState.Injured).ToList();

            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

            // 1) 뒤에서부터 잠금
            for (int i = slots.Count - 1; i >= usable; i--)
            {
                slots[i].ShowLocked();
                _assigned[i] = null;
            }

            // 2) 현재 작업중인 배치(_assigned) 우선 수집
            var picked = new List<DomAthEntity>();
            var seen = new HashSet<int>();
            for (int i = 0; i < usable; i++)
            {
                var ent = _assigned[i];
                if (ent == null) continue;
                if (ent.curState != AthleteState.Injured) continue;
                if (!seen.Add(ent.id)) continue;
                picked.Add(ent);
            }

            // 3) 작업중 배치가 없다면 저장본을 초기 배치로 사용
            if (picked.Count == 0 && _savedAssignedIds != null && _savedAssignedIds.Count > 0)
            {
                foreach (var id in _savedAssignedIds)
                {
                    if (picked.Count >= usable) break;
                    if (id < 0) { picked.Add(null); continue; }  // 빈 칸 유지
                    if (!byId.TryGetValue(id, out var ent)) { picked.Add(null); continue; }
                    if (!seen.Add(id)) { picked.Add(null); continue; }
                    picked.Add(ent.curState == AthleteState.Injured ? ent : null);
                }

                // 저장본 → _assigned에도 반영하여 일관성 유지
                for (int i = 0; i < usable; i++)
                    _assigned[i] = i < picked.Count ? picked[i] : null;
            }

            // 4) 슬롯 그리기
            int iWrite = 0;
            for (; iWrite < usable; iWrite++)
            {
                var ent = (iWrite < picked.Count) ? picked[iWrite] : null;
                if (ent != null) slots[iWrite].ShowAssigned(ent);
                else
                {
                    // 아직 배치되지 않은 부상자 수로 빈/불가 구분
                    int alreadyPicked = picked.Count(x => x != null);
                    int unpicked = Math.Max(0, injuredAll.Count - alreadyPicked);
                    if (unpicked > 0) slots[iWrite].ShowEmpty();
                    else              slots[iWrite].ShowNoAvailable();
                }
            }
        }

        
        // ============================================================
        // [7] InjureListPanel과 이벤트 연결
        // ============================================================
        private void WireListPanelHandlers()
        {
            _panelSubs?.Dispose();
            var cd = new CompositeDisposable();
            cd.Add(Disposable.Create(() => _draftAssigned = null)); // 패널 닫히면 초안 폐기

            // "배치하기"
            injureListPanel.OnRequestAssign
                .Subscribe(ath =>
                {
                    if (_draftAssigned == null) _draftAssigned = (DomAthEntity[])_assigned.Clone();
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

                    // 첫 빈칸
                    int firstEmpty = -1;
                    for (int i = 0; i < usable; i++) if (_draftAssigned[i] == null) { firstEmpty = i; break; }
                    if (firstEmpty < 0) { injureListPanel.NudgeAssignButton(ath.id); return; }

                    // 중복 제거
                    for (int i = 0; i < usable; i++) if (_draftAssigned[i]?.id == ath.id) _draftAssigned[i] = null;

                    _draftAssigned[firstEmpty] = ath;
                    injureListPanel.UpdateItemAssigned(ath.id, true); // 리스트만 업데이트
                })
                .AddTo(cd);

            // 해제
            injureListPanel.OnRequestUnassign
                .Subscribe(ath =>
                {
                    if (_draftAssigned == null) return;
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
                    for (int i = 0; i < usable; i++) if (_draftAssigned[i]?.id == ath.id) _draftAssigned[i] = null;
                    injureListPanel.UpdateItemAssigned(ath.id, false);
                })
                .AddTo(cd);

            // 리셋
            injureListPanel.OnRequestReset
                .Subscribe(_ =>
                {
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
                    if (_draftAssigned == null) _draftAssigned = (DomAthEntity[])_assigned.Clone();
                    for (int i = 0; i < usable; i++) _draftAssigned[i] = null;
                    injureListPanel.UpdateAllAssignedFalse();
                })
                .AddTo(cd);

            // 확정: 초안→커밋 + 저장 + Refresh
            injureListPanel.OnRequestConfirm
                .Subscribe(_ =>
                {
                    if (_draftAssigned != null)
                        Array.Copy(_draftAssigned, _assigned, _assigned.Length);

                    var ids = GetAssignedIdsForSave().ToArray();          // 커밋 기준
                    saveManager.SetAssignedTreatmentAthletes(ids);
                    saveManager.SaveProgress(saveManager.GetCurrentSlotIndex());
                    _savedAssignedIds = ids.ToList();

                    Refresh();
                    injuredAthleteInfoPUI?.SetActive(false);
                    _draftAssigned = null;
                })
                .AddTo(cd);

            _panelSubs = cd;
        }
        
        

        // ============================================================
        // [8] OnDisable / OnDestroy : 구독 정리
        // ============================================================
        private void OnDisable()
        {
            _panelSubs?.Dispose();
            _enableCd?.Dispose();
        }
        
        private void OnDestroy()
        {
            _wireCd.Dispose();
            _panelSubs?.Dispose();
        }
        

        // ------------------------------
        // 선수 아이디로 슬롯 인덱스 찾기
        // ------------------------------
        private int FindSlotIndexByAthleteId(int athId)
        {
            for (int i = 0; i < _assigned.Length; i++)
                if (_assigned[i]?.id == athId) return i;
            return -1;
        }


        // ------------------------------
        // 개방 가능한 슬롯 수 계산
        // ------------------------------
        private int GetUsableSlots()
        {
            var level = facilitiesController.MedicalCenter.CurrentStage.Value;
            switch (level)
            {
                case 0: return 2; // 2개 개방
                case 1: return 4; // 4개 개방
                case 2: return 6; // 6개 개방
                default: return 8; // 전부 개방
            }
        }

        // ------------------------------
        // 이미 배치된 선수 id 집합
        // ------------------------------
        private HashSet<int> GetAssignedIdSet()
            => _assigned.Where(a => a != null).Select(a => a.id).ToHashSet();

        // ------------------------------
        // 배치 후보 없으면
        // ------------------------------
        private void ShowNoCandidateHint()
        {
            Debug.Log("배치 가능한 부상 선수가 없습니다.");
        }

        // ------------------------------
        // [세이브/로드 API]
        // ------------------------------
        public void SetSavedAssignments(IEnumerable<int> ids)
        {
            _savedAssignedIds = ids?.ToList() ?? new List<int>();
        }

        public IReadOnlyList<int> GetAssignedIdsForSave()
        {
            var list = new List<int>(_assigned.Length);
            foreach (var ent in _assigned)
                list.Add(ent?.id ?? -1); // 배치 안된 칸은 -1
            return list;
        }
        
        private void LoadAssignedFromSave()
        {
            var ids = saveManager.GetAssignedTreatmentAthletes(); // int[8] 반환 가정 (-1=빈칸)
            if (ids == null || ids.Length == 0) return;

            _savedAssignedIds = ids.ToList();

            // 저장본 → 현재 상태에도 하이드레이트(즉시 보이게)
            var all  = athleteService.GetAllRecruitedAthleteList() ?? new List<DomAthEntity>();
            var byId = all.ToDictionary(a => a.id, a => a);
            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

            for (int i = 0; i < usable; i++)
            {
                var id = ids[i];
                _assigned[i] = (id >= 0 && byId.TryGetValue(id, out var ent) && ent.curState == AthleteState.Injured)
                    ? ent
                    : null;
            }
            for (int i = usable; i < _assigned.Length; i++) _assigned[i] = null;
        }
    }
}
