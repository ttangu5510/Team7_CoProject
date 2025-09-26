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
        private readonly DomAthEntity[] _assigned = new DomAthEntity[8];
        private DomAthEntity[] _draftAssigned;

        // 세이브 캐시
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

            // RestRoom 단계 변화 → 즉시 갱신
            var rr = facilitiesController.RestRoom; // 인터페이스에 RestRoom 있다고 가정
            rr.CurrentStage     .Subscribe(_ => Refresh()).AddTo(_enableCd);
            rr.NumberOfAthletes .Subscribe(_ => Refresh()).AddTo(_enableCd);
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
                .Where(a => a.curState != AthleteState.Injured && a.fatigue > 0)
                .ToList();
            if (candidates.Count == 0) { Debug.Log("휴식 가능한 선수가 없습니다."); return; }

            restPanelPUI.SetActive(true);
            restListPanel.gameObject.SetActive(true);

            _draftAssigned = (DomAthEntity[])_assigned.Clone();
            restListPanel.Open(candidates, GetAssignedIdSet(draft: true));

            WireListPanelHandlers();
        }

        private void HandleAssignedSlot(int _slotIndex)
        {
            // 휴게실은 교체 제한 없음 → 그냥 패널 열어 교체 허용
            OpenAssignPanel(_slotIndex);
        }

        private HashSet<int> GetAssignedIdSet(bool draft = false)
        {
            var src = draft && _draftAssigned != null ? _draftAssigned : _assigned;
            return src.Where(a => a != null).Select(a => a.id).ToHashSet();
        }

        private void Refresh()
        {
            var all = athleteService.GetAllRecruitedAthleteList() ?? new List<DomAthEntity>();
            var byId = all.ToDictionary(a => a.id, a => a);
            var restable = all.Where(a => a.curState != AthleteState.Injured && a.fatigue > 0).ToList();

            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

            // 뒤에서 잠금
            for (int i = slots.Count - 1; i >= usable; i--)
            {
                slots[i].ShowLocked();
                _assigned[i] = null;
            }

            // 커밋 상태 우선
            var picked = new List<DomAthEntity>();
            var seen = new HashSet<int>();
            for (int i = 0; i < usable; i++)
            {
                var ent = _assigned[i];
                if (ent == null) continue;
                if (ent.curState == AthleteState.Injured) continue;
                if (!seen.Add(ent.id)) continue;
                picked.Add(ent);
            }

            // 없으면 저장본 적용
            if (picked.Count == 0 && _savedAssignedIds.Count > 0)
            {
                foreach (var id in _savedAssignedIds)
                {
                    if (picked.Count >= usable) break;
                    if (id < 0) { picked.Add(null); continue; }
                    if (!byId.TryGetValue(id, out var ent)) { picked.Add(null); continue; }
                    if (!seen.Add(id)) { picked.Add(null); continue; }
                    picked.Add(ent.curState != AthleteState.Injured ? ent : null);
                }
                for (int i = 0; i < usable; i++)
                    _assigned[i] = i < picked.Count ? picked[i] : null;
            }

            // 그리기
            for (int i = 0; i < usable; i++)
            {
                var ent = (i < picked.Count) ? picked[i] : null;
                if (ent != null) slots[i].ShowAssigned(ent);
                else
                {
                    int already = picked.Count(x => x != null);
                    int unpicked = Math.Max(0, restable.Count - already);
                    if (unpicked > 0) slots[i].ShowEmpty();
                    else               slots[i].ShowNoAvailable();
                }
            }
        }

        private void WireListPanelHandlers()
        {
            _panelSubs?.Dispose();
            var cd = new CompositeDisposable();
            cd.Add(Disposable.Create(() => _draftAssigned = null)); // 닫히면 초안 폐기

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
                    restListPanel.UpdateItemAssigned(ath.id, true);
                })
                .AddTo(cd);

            restListPanel.OnRequestUnassign
                .Subscribe(ath =>
                {
                    if (_draftAssigned == null) return;
                    int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);
                    for (int i = 0; i < usable; i++) if (_draftAssigned[i]?.id == ath.id) _draftAssigned[i] = null;
                    restListPanel.UpdateItemAssigned(ath.id, false);
                })
                .AddTo(cd);

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
            var level = facilitiesController.RestRoom.CurrentStage.Value; // RestRoom 가정
            return level switch { 0 => 2, 1 => 4, 2 => 6, _ => 8 };
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

            var all = athleteService.GetAllRecruitedAthleteList() ?? new List<DomAthEntity>();
            var byId = all.ToDictionary(a => a.id, a => a);
            int usable = Mathf.Clamp(GetUsableSlots(), 0, slots.Count);

            for (int i = 0; i < usable; i++)
            {
                var id = ids[i];
                _assigned[i] = (id >= 0 && byId.TryGetValue(id, out var ent) && ent.curState != AthleteState.Injured)
                    ? ent
                    : null;
            }
            for (int i = usable; i < _assigned.Length; i++) _assigned[i] = null;
        }
    }
}
