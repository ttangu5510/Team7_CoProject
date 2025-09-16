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
    /// </summary>
    public class TreatmentRoomTabView : MonoBehaviour
    {
        // ------------------------------
        // [필드 및 DI]
        // ------------------------------
        [SerializeField] private List<TreatmentSlotView> slots; // Slot A~H 순서대로
        [Inject] private DomAthService athleteService;
        [Inject] private IFacilitiesController facilitiesController;

        [SerializeField] private GameObject injuredAthleteInfoPUI; // 팝업 루트
        [SerializeField] private InjureListPanel injureListPanel;  // 부상자 목록 패널
        [SerializeField] private InjureAthInfoPanel injureAthInfoPanel; // 상세 스탯 패널

        private CompositeDisposable _enableCd;                 // 활성화 기간 구독 모음
        private readonly CompositeDisposable _wireCd = new();  // 고정 배선(슬롯 클릭 등) 구독 모음
        private IDisposable _pickSub;                          // 리스트 OnPick(선수 선택) 단일 구독 레퍼런스

        // 슬롯별 배치 현황(세이브/로드 대상)
        private readonly DomAthEntity[] _assigned = new DomAthEntity[8];

        // 세이브/로드 용: 슬롯에 배치된 선수 id들 (A→H 순)
        private List<int> _savedAssignedIds = new();

        // ------------------------------
        // [1] 고정 배선: 슬롯 클릭 구독 → 플로우 진입점
        //     - 게임 실행 중 한 번만 연결
        //     - 슬롯 클릭 시, 빈/배치 여부로 분기
        // ------------------------------
        private void Awake()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                int idx = i; // 클로저 캡처
                slots[idx].Clicked
                    .Subscribe(_ =>
                    {
                        // 잠금/배치 불가 상태는 무시
                        if (slots[idx].IsLocked || slots[idx].IsNoAvailable) return;

                        // 빈 슬롯 → 배치 패널
                        if (_assigned[idx] == null)
                        {
                            OpenAssignPanel(idx);
                        }
                        else
                        {
                            // 배치된 슬롯 → 남은 치료 기간 체크
                            HandleAssignedSlot(idx);
                        }
                    })
                    .AddTo(_wireCd);
            }
        }

        // ------------------------------
        // [2] 패널 활성 시 초기화 + 데이터 구독
        //     - 팝업 루트/리스트/상세 패널 초기 상태 OFF
        //     - MedicalCenter 변화 구독 → Refresh()
        // ------------------------------
        private void OnEnable()
        {
            _enableCd = new CompositeDisposable();

            // 초기 UI 상태 리셋 (뚜껑 열린 채로 남는 것 방지)
            if (injuredAthleteInfoPUI) injuredAthleteInfoPUI.SetActive(false);
            if (injureListPanel)       injureListPanel.gameObject.SetActive(false);
            if (injureAthInfoPanel)    injureAthInfoPanel.gameObject.SetActive(false);

            // 최초 갱신
            Refresh();

            // 업그레이드 단계/수용 인원 변화 → 즉시 갱신
            var mc = facilitiesController.MedicalCenter;
            mc.CurrentStage     .Subscribe(_ => Refresh()).AddTo(_enableCd);
            mc.NumberOfAthletes .Subscribe(_ => Refresh()).AddTo(_enableCd);
        }

        // ------------------------------
        // [3] 빈 슬롯 클릭 → 배치 패널 열기
        //     - InjureListPanel.Open(injuredAll, assignedSet)
        //     - OnPick(선수 선택) 1회만 받아서 슬롯에 배치 후 Refresh()
        // ------------------------------
        private void OpenAssignPanel(int slotIndex)
        {
            // 전체 부상자 목록 수집
            var injuredAll = athleteService.GetAllRecruitedAthleteList()
                .Where(a => a.curState == AthleteState.Injured)
                .ToList();

            if (injuredAll.Count == 0) { ShowNoCandidateHint(); return; }

            // 팝업 루트/리스트 표시, 상세는 OFF
            if (injuredAthleteInfoPUI) injuredAthleteInfoPUI.SetActive(true);
            if (injureListPanel)       injureListPanel.gameObject.SetActive(true);
            if (injureAthInfoPanel)    injureAthInfoPanel.gameObject.SetActive(false);

            // 이미 배치된 선수 보호 세트(리스트에서 '배치됨' 비활성 표시 용)
            var assignedSet = GetAssignedIdSet();

            // 리스트 오픈 + 재구독 전 기존 구독 해제(누적 방지)
            injureListPanel.Open(injuredAll, assignedSet);
            _pickSub?.Dispose();
            _pickSub = injureListPanel.OnPick
                .Take(1)
                .Subscribe(ath =>
                {
                    _assigned[slotIndex] = ath; // 배치
                    Refresh();
                    // 필요 시: 리스트/루트 닫기, 상세로 전환 등 추가 가능
                });
        }

        // ------------------------------
        // [4] 배치된 슬롯 클릭 → 남은 치료 기간 분기
        //     - 남은 기간 ≥ 2주 → 무반응
        //     - 남은 기간 ≤ 1주 → 교체 패널 오픈
        // ------------------------------
        private void HandleAssignedSlot(int slotIndex)
        {
            var ath = _assigned[slotIndex];
            if (ath == null) return;

            // leftInjury 단위가 "주" 기준: 2주 이상이면 교체 불가
            if (ath.leftInjury > 1)
            {
                // 필요하면 토스트/사운드/UI 힌트 출력
                Debug.Log("남은 치료 기간이 2주 이상 → 교체 불가");
                return;
            }

            // 1주 이내 → 교체 허용
            OpenReplacePanel(slotIndex);
        }

        // ------------------------------
        // [5] 교체 패널 열기 (배치 패널과 동일 흐름, 선택 시 교체)
        //     - InjureListPanel.Open(injuredAll, assignedSet)
        //     - OnPick(새 선수) 1회만 받아서 해당 슬롯에 교체 후 Refresh()
        // ------------------------------
        private void OpenReplacePanel(int slotIndex)
        {
            var injuredAll = athleteService.GetAllRecruitedAthleteList()
                .Where(a => a.curState == AthleteState.Injured)
                .ToList();

            if (injuredAll.Count == 0) { ShowNoCandidateHint(); return; }

            if (injuredAthleteInfoPUI) injuredAthleteInfoPUI.SetActive(true);
            if (injureListPanel)       injureListPanel.gameObject.SetActive(true);
            if (injureAthInfoPanel)    injureAthInfoPanel.gameObject.SetActive(false);

            var assignedSet = GetAssignedIdSet();

            injureListPanel.Open(injuredAll, assignedSet);
            _pickSub?.Dispose();
            _pickSub = injureListPanel.OnPick
                .Take(1)
                .Subscribe(newAth =>
                {
                    _assigned[slotIndex] = newAth; // 교체
                    Refresh();
                    // 필요 시: 패널 닫기/상세 전환 추가 가능
                });
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
            var all = athleteService.GetAllRecruitedAthleteList();
            var byId = all.ToDictionary(a => a.id, a => a);
            var injuredAll = all.Where(a => a.curState == AthleteState.Injured).ToList();

            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

            // 1) 뒤에서부터 NeedUpgrade(잠금)
            for (int i = slots.Count - 1; i >= usable; i--)
            {
                slots[i].ShowLocked();
                _assigned[i] = null;
            }

            // 2) 저장된 배치 → 앞에서부터 배치
            var picked = new List<DomAthEntity>();
            var seen = new HashSet<int>();
            foreach (var id in _savedAssignedIds)
            {
                if (picked.Count >= usable) break;
                if (!byId.TryGetValue(id, out var ent)) continue;
                if (ent.curState != AthleteState.Injured) continue;
                if (!seen.Add(id)) continue;
                picked.Add(ent);
            }

            int write = 0;
            for (; write < picked.Count && write < usable; write++)
            {
                var ent = picked[write];
                _assigned[write] = ent;
                slots[write].ShowAssigned(ent);
            }

            // 3) 남은 usable 슬롯 처리
            int unassignedInjuredCount = injuredAll.Count - picked.Count;
            for (int i = write; i < usable; i++)
            {
                _assigned[i] = null;

                if (unassignedInjuredCount <= 0)
                    slots[i].ShowNoAvailable(); // 배치 가능한 부상자 없음
                else
                    slots[i].ShowEmpty();       // 부상자 있음 → 빈 슬롯
            }
        }

        // ------------------------------
        // [7] 활성화 기간 끝 → 구독 정리
        // ------------------------------
        private void OnDisable()
        {
            _enableCd?.Dispose();
        }

        // ------------------------------
        // [8] 오브젝트 파괴 시 → 고정 배선/단일 구독 정리
        // ------------------------------
        private void OnDestroy()
        {
            _wireCd.Dispose();
            _pickSub?.Dispose();
        }

        // ------------------------------
        // 개방 가능한 슬롯 수 계산 (MedicalCenter 단계 기준)
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
    }
}
