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

        [Header("Panels")]
        [SerializeField] private GameObject restPanelPUI;     // 팝업 루트(블로커)
        [SerializeField] private RestListPanel restListPanel; // 후보 리스트 패널

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
                .Where(a => a.curState != AthleteState.Injured && a.stats.fatigue > 0)
                .ToList();
            if (candidates.Count == 0) { Debug.Log("휴식 가능한 선수가 없습니다."); return; }

            restPanelPUI.SetActive(true);
            restListPanel.gameObject.SetActive(true);

            _draftAssigned = (DomAthEntity[])_assigned.Clone();
            restListPanel.Open(candidates, GetAssignedIdSet(draft: true));

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

            var restable = all.Where(a => a.curState != AthleteState.Injured && a.stats.fatigue > 0).ToList();
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

            // 4) 슬롯 그리기 (가용 후보 중 아직 배치 안 된 수로 Empty/NoAvailable 결정)
            var pickedIds = picked.Where(x => x != null).Select(x => x.id).ToHashSet();
            int unpickedCount = restable.Count(a => !pickedIds.Contains(a.id));

            for (int i = 0; i < usable; i++)
            {
                var ent = (i < picked.Count) ? picked[i] : null;
                if (ent != null)
                {
                    slots[i].ShowAssigned(ent);
                }
                else
                {
                    if (unpickedCount > 0) { slots[i].ShowEmpty(); unpickedCount--; }
                    else                   { slots[i].ShowNoAvailable(); }
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
            return level switch { 0 => 2, 1 => 4, 2 => 6, _ => 8 };
        }

        public IReadOnlyList<int> GetAssignedIdsForSave()
        {
            var list = new List<int>(_assigned.Length);
            foreach (var ent in _assigned) list.Add(ent?.id ?? -1);
            return list;
        }

        // === 저장본 로드: 유효성(비부상 + 피로>0) 재검증 ===
        private void LoadAssignedFromSave()
        {
            var ids = saveManager.GetAssignedRestAthletes(); // int[8], -1=빈칸
            if (ids == null || ids.Length == 0) return;

            _savedAssignedIds = ids.ToList();

            var all  = athleteService.GetAllRecruitedAthleteList() ?? new List<DomAthEntity>();
            var byId = all.ToDictionary(a => a.id, a => a);
            var restableIdSet = all
                .Where(a => a.curState != AthleteState.Injured && a.stats.fatigue > 0)
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
    }
}
