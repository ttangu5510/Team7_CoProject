using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using JYL;
using SHG;
using TMPro;
using UnityEngine.UI;

namespace JWS
{
    /// <summary>
    /// 휴게실(라운지) 탭 컨트롤러
    /// - 슬롯 8개 관리(드래프트/커밋 분리)
    /// - 빈 슬롯 클릭 시 후보 패널(피로>0, 비부상) 오픈
    /// - 확정 시에만 커밋/세이브 반영
    /// - 시설 단계 변화/인원 변화에 따라 UI 갱신
    /// </summary>
    public class RestRoomTabView : MonoBehaviour
    {
        [SerializeField] private List<RestSlotView> slots;   // 좌→우 A~H
        [Inject] private DomAthService athleteService;
        [Inject] private IFacilitiesController facilitiesController;
        [Inject] private SaveManager saveManager;

        [Header("Assign Pannel")]
        [SerializeField] private TextMeshProUGUI assignText;
        [SerializeField] private Button restButton;
        
        [Header("Popup Panels")]
        [SerializeField] private GameObject restPanelPUI;     // 팝업 루트(블로커)
        [SerializeField] private RestListPanel restListPanel; // 후보 리스트 패널
        [SerializeField] private InjureAthInfoPanel injureAthInfoPanel; // 상세 스탯 패널
        [SerializeField] private RestResultPanel restResultPanel; // 휴식 결과 패널
        [SerializeField] private RestProgressPUI restProgressPUI;

        [Inject] private ITimeFlowController flowController;
        
        private CompositeDisposable _enableCd;
        private readonly CompositeDisposable _wireCd = new();
        private IDisposable _panelSubs;

        // 커밋/초안
        private readonly DomAthEntity[] _assigned = new DomAthEntity[8]; // 커밋
        private DomAthEntity[] _draftAssigned;                            // 초안(패널 열릴 때만 생존)

        // 세이브 캐시(id 배열)
        private List<int> _savedAssignedIds = new();

        private void Awake()
        {
            for (int i = 0; i < slots.Count; i++)
            {
                int idx = i;
                slots[idx].Clicked
                    .Subscribe(_ =>
                    {
                        if (slots[idx].IsLocked || slots[idx].IsNoAvailable) return;
                        if (_assigned[idx] == null) OpenAssignPanel(idx);
                        else HandleAssignedSlot(idx);
                    })
                    .AddTo(_wireCd);
            }
        }

        private void OnEnable()
        {
            _enableCd = new CompositeDisposable();

            restPanelPUI?.SetActive(false);
            if (restListPanel) restListPanel.gameObject.SetActive(false);

            LoadAssignedFromSave();
            Refresh();

            // 휴게실 = Lounge
            var lounge = facilitiesController.Lounge;
            lounge.CurrentStage     .Subscribe(_ => Refresh()).AddTo(_enableCd);
            lounge.NumberOfAthletes .Subscribe(_ => Refresh()).AddTo(_enableCd);
            
            if (restButton)
                restButton.OnClickAsObservable()
                    .Subscribe(_ =>
                        {
                            if (!HasAnyAssigned()) { NudgeRestButton(); return; }
                            StartRest();
                        })
                        .AddTo(_enableCd);
        }

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

        // 빈 슬롯 클릭 → 후보 패널
        private void OpenAssignPanel(int _)
        {
            var candidates = athleteService.GetAllRecruitedAthleteList()
                .Where(a => a.stats.fatigue > 0)
                .OrderByDescending(a => a.stats.fatigue)
                .ToList();
            if (candidates.Count == 0) { Debug.Log("휴식 가능한 선수가 없습니다."); return; }

            restPanelPUI.SetActive(true);
            restListPanel.gameObject.SetActive(true);
            if (injureAthInfoPanel) injureAthInfoPanel.gameObject.SetActive(false);
            if (restResultPanel) restResultPanel.gameObject.SetActive(false);

            _draftAssigned = (DomAthEntity[])_assigned.Clone();
            
            // 휴게실에 이미 담긴 선수
            var restAssigned = GetAssignedIdSet(draft: true);
            // 의료센터(치료실)에 배치된 선수
            var treatIds = saveManager.GetAssignedTreatmentAthletes() ?? Array.Empty<int>();
            var treatmentAssigned = treatIds.Where(id => id >= 0).ToHashSet();
            // 의료센터 배치 우선 표기
            restListPanel.Open(candidates, restAssigned, treatmentAssigned);

            WireListPanelHandlers();
        }

        // 배치 슬롯 클릭 → 교체 허용
        private void HandleAssignedSlot(int _slotIndex)
        {
            OpenAssignPanel(_slotIndex);
        }

        private HashSet<int> GetAssignedIdSet(bool draft = false)
        {
            var src = draft && _draftAssigned != null ? _draftAssigned : _assigned;
            return src.Where(a => a != null).Select(a => a.id).ToHashSet();
        }

        // === 핵심 갱신: 유효성(비부상 + 피로>0) 재검증 포함 ===
        private void Refresh()
        {
            var all  = athleteService.GetAllRecruitedAthleteList() ?? new List<DomAthEntity>();
            var byId = all.ToDictionary(a => a.id, a => a);

            var restable = all.Where(a => a.stats.fatigue > 0).ToList();
            var restableIdSet = restable.Select(a => a.id).ToHashSet();

            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

            // 1) 뒤에서 잠금
            for (int i = slots.Count - 1; i >= usable; i--)
            {
                slots[i].ShowLocked();
                _assigned[i] = null;
            }

            // 2) 커밋 상태에서 '여전히 유효한' 애만 유지
            var picked = new List<DomAthEntity>(usable);
            var seen   = new HashSet<int>();
            for (int i = 0; i < usable; i++)
            {
                var ent = _assigned[i];
                if (ent == null)
                {
                    picked.Add(null);
                    continue;
                }

                if (!restableIdSet.Contains(ent.id) || !seen.Add(ent.id))
                {
                    _assigned[i] = null;
                    picked.Add(null);
                    continue;
                }

                picked.Add(ent);
            }

            // 3) 전부 비었으면 저장본 시도(저장본도 유효성 재검증)
            bool anyPicked = picked.Any(x => x != null);
            if (!anyPicked && _savedAssignedIds != null && _savedAssignedIds.Count > 0)
            {
                picked.Clear();
                seen.Clear();
                for (int i = 0; i < usable; i++)
                {
                    if (i >= _savedAssignedIds.Count) { picked.Add(null); continue; }
                    int id = _savedAssignedIds[i];
                    if (id < 0) { picked.Add(null); continue; }
                    if (!byId.TryGetValue(id, out var ent)) { picked.Add(null); continue; }
                    if (!restableIdSet.Contains(id) || !seen.Add(id)) { picked.Add(null); continue; }
                    picked.Add(ent);
                }

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
                    int alreadyPicked = picked.Count(x => x != null);
                    int unpicked = Math.Max(0, restable.Count - alreadyPicked);
                    if (unpicked > 0) slots[iWrite].ShowEmpty();
                    else              slots[iWrite].ShowNoAvailable();
                }
            }
        }

        private void WireListPanelHandlers()
        {
            _panelSubs?.Dispose();
            var cd = new CompositeDisposable();
            cd.Add(Disposable.Create(() => _draftAssigned = null)); // 닫히면 초안 폐기

            // 배치
            restListPanel.OnRequestAssign
                .Subscribe(ath =>
                {
                    if (_draftAssigned == null) _draftAssigned = (DomAthEntity[])_assigned.Clone();
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

                    int firstEmpty = -1;
                    for (int i = 0; i < usable; i++) if (_draftAssigned[i] == null) { firstEmpty = i; break; }
                    if (firstEmpty < 0) { restListPanel.NudgeAssignButton(ath.id); return; }

                    for (int i = 0; i < usable; i++) if (_draftAssigned[i]?.id == ath.id) _draftAssigned[i] = null;

                    _draftAssigned[firstEmpty] = ath;
                    restListPanel.UpdateItemAssigned(ath.id, true); // 리스트만 갱신(커밋 X)
                })
                .AddTo(cd);

            // 해제
            restListPanel.OnRequestUnassign
                .Subscribe(ath =>
                {
                    if (_draftAssigned == null) return;
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
                    for (int i = 0; i < usable; i++) if (_draftAssigned[i]?.id == ath.id) _draftAssigned[i] = null;
                    restListPanel.UpdateItemAssigned(ath.id, false);
                })
                .AddTo(cd);

            // 리셋
            restListPanel.OnRequestReset
                .Subscribe(_ =>
                {
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
                    if (_draftAssigned == null) _draftAssigned = (DomAthEntity[])_assigned.Clone();
                    for (int i = 0; i < usable; i++) _draftAssigned[i] = null;
                    restListPanel.UpdateAllAssignedFalse();
                })
                .AddTo(cd);

            // 확정: 초안 → 커밋 + 저장 + 갱신
            restListPanel.OnRequestConfirm
                .Subscribe(_ =>
                {
                    if (_draftAssigned != null)
                        Array.Copy(_draftAssigned, _assigned, _assigned.Length);

                    var ids = GetAssignedIdsForSave().ToArray();
                    saveManager.SetAssignedRestAthletes(ids); // SaveManager에 API 필요
                    saveManager.SaveProgress(saveManager.GetCurrentSlotIndex());
                    _savedAssignedIds = ids.ToList();
                    
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
                    int assignedCnt = 0;
                    for (int i = 0; i < usable; i++) if (_assigned[i] != null) assignedCnt++;
                    assignText?.SetText($"휴식 진행할 선수 배치 ({assignedCnt}/{usable})");

                    Refresh();
                    restPanelPUI?.SetActive(false);
                    _draftAssigned = null;
                })
                .AddTo(cd);

            _panelSubs = cd;
        }

        private int GetUsableSlots()
        {
            var level = facilitiesController.Lounge.CurrentStage.Value; // 휴게실 단계
            switch (level)
            {
                case 0: return 2; // 2개 개방
                case 1: return 4; // 4개 개방
                case 2: return 6; // 6개 개방
                default: return 8; // 전부 개방
            }
        }
        
        public IReadOnlyList<int> GetAssignedIdsForSave()
        {
            var list = new List<int>(_assigned.Length);
            foreach (var ent in _assigned) list.Add(ent?.id ?? -1);
            return list;
        }

        private void LoadAssignedFromSave()
        {
            var ids = saveManager.GetAssignedRestAthletes(); // int[8], -1=빈칸
            if (ids == null || ids.Length == 0) return;

            _savedAssignedIds = ids.ToList();

            var all  = athleteService.GetAllRecruitedAthleteList() ?? new List<DomAthEntity>();
            var byId = all.ToDictionary(a => a.id, a => a);
            var restableIdSet = all
                .Where(a => a.stats.fatigue > 0)
                .Select(a => a.id)
                .ToHashSet();

            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

            for (int i = 0; i < usable; i++)
            {
                int id = (i < ids.Length) ? ids[i] : -1;
                _assigned[i] = (id >= 0
                                && byId.TryGetValue(id, out var ent)
                                && restableIdSet.Contains(id))
                    ? ent
                    : null;
            }
            for (int i = usable; i < _assigned.Length; i++) _assigned[i] = null;
        }

        private bool HasAnyAssigned()
        {
            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
            for (int i = 0; i < usable; i++)
                if (_assigned[i] != null) return true;
            return false;
        }

        private void StartRest()
        {
            Debug.Log("휴식시작");
        
            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
        
            if (!HasAnyAssigned())
            {
                NudgeRestButton();
                Debug.Log("휴식 진행할 선수가 없습니다.");
                return;
            }
            
            int lv = Mathf.Clamp(facilitiesController.Lounge.CurrentStage.Value, 0, 4);
            int recover = lv switch { 0=>40, 1=>50, 2=>55, 3=>60, 4=>70, _=>40 };
            
            var ids = new List<int>(usable);
            for (int i = 0; i < usable; i++)
            {
                var ath = _assigned[i];
                if (ath != null) ids.Add(ath.id);
            }
            
            var results = new List<RestResultData>(ids.Count);
            foreach (var id in ids)
            {
                var ent = _assigned.FirstOrDefault(a => a != null && a.id == id);
                if (ent == null) continue;

                int before  = ent.stats.fatigue;
                int reduced = Mathf.Min(before, recover); // 실제 깎이는 양
                results.Add(new RestResultData
                {
                    portrait = null,                 // 있으면 넣어(스프라이트)
                    name     = ent.entityName,
                    reducedFatigue = reduced
                });
            }
            
            athleteService.ApplyRestRecovery(ids, recover);
            
            // 휴식 진행 팝업 표시
            if (restPanelPUI && !restPanelPUI.activeSelf) restPanelPUI.SetActive(true);
            
            RestProgressPUI progPui = Instantiate(restProgressPUI, restPanelPUI.transform);
            progPui.gameObject.SetActive(true);
            progPui.Init();
            
            progPui.Confirmed.Subscribe(_ =>
            {
                if (restPanelPUI && !restPanelPUI.activeSelf) restPanelPUI.SetActive(true);

                int year = flowController.Year.Value;
                string season = SeasonKo(flowController.CurrentSeason.Value);
                int weekInSeason = ((flowController.WeekInYear.Value - 1) % SHG.ITimeFlowController.WEEK_FOR_SEASON) + 1;

                var resultPanel = Instantiate(restResultPanel, restPanelPUI.transform);
                resultPanel.gameObject.SetActive(true);
                
                resultPanel.Open(
                    year.ToString(),
                    season,
                    weekInSeason.ToString(),
                    results,
                    recover,
                    onClose: () =>
                    {
                        // ▶ 확인 버튼 눌렀을 때 실행
                        flowController.ProgressWeek();                         // 다음 주차로
                        saveManager.SaveProgress(saveManager.GetCurrentSlotIndex()); // 세이브
                        restPanelPUI?.SetActive(false);                        // 오버레이 닫기

                        // (옵션) UI 보정
                        int usableNow = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
                        assignText?.SetText($"휴식 진행할 선수 배치 (0/{usableNow})");
                        Refresh();
                    }
                );
            }).AddTo(progPui);

            

            
            // UI 슬롯 전부 비우기
            for (int i = 0; i < _assigned.Length; i++) _assigned[i] = null;

            // 세이브 슬롯도 전부 -1로 초기화
            _savedAssignedIds = Enumerable.Repeat(-1, _assigned.Length).ToList();
            saveManager.SetAssignedRestAthletes(_savedAssignedIds.ToArray());

            Refresh();
            
        }
        
        public void NudgeRestButton()
        {
            var btn = restButton ? restButton.transform : transform;
            StartCoroutine(NudgeCo(btn));
        }

        private System.Collections.IEnumerator NudgeCo(Transform t)
        {
            var basePos = t.localPosition;
            float d = 6f, dur = 0.08f;
            for (int i = 0; i < 3; i++)
            {
                t.localPosition = basePos + Vector3.right * d; yield return new WaitForSeconds(dur);
                t.localPosition = basePos - Vector3.right * d; yield return new WaitForSeconds(dur);
            }
            t.localPosition = basePos;
        }
        private static string SeasonKo(SHG.Season s) => s switch
        {
            SHG.Season.Spring => "봄",
            SHG.Season.Summer => "여름",
            SHG.Season.Fall => "가을",
            SHG.Season.Winter => "겨울",
            _ => ""
        };
    }
}
