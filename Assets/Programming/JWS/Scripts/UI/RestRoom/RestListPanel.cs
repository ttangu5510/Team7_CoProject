using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using JYL;

namespace JWS
{
    public class RestListPanel : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button closeButton;

        [Header("List")]
        [SerializeField] private Transform content;            // ScrollView/Viewport/Content
        [SerializeField] private RestAthleteItem itemPrefab;   // ★ Prefab 에셋 연결
        [SerializeField] private GameObject restPanelPUI;      // 팝업 루트

        [Header("Footer")]
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button resetButton;

        private readonly Subject<DomAthEntity> _reqAssign   = new();
        private readonly Subject<DomAthEntity> _reqUnassign = new();
        private readonly Subject<Unit>         _reqReset    = new();
        private readonly Subject<Unit>         _reqConfirm  = new();

        public IObservable<DomAthEntity> OnRequestAssign   => _reqAssign;
        public IObservable<DomAthEntity> OnRequestUnassign => _reqUnassign;
        public IObservable<Unit>         OnRequestReset    => _reqReset;
        public IObservable<Unit>         OnRequestConfirm  => _reqConfirm;

        private readonly Dictionary<int, RestAthleteItem> _itemById = new();
        private readonly List<GameObject> _spawned = new();

        private void Awake()
        {
            closeButton?.OnClickAsObservable()
                .Subscribe(_ => { if (restPanelPUI) restPanelPUI.SetActive(false); else gameObject.SetActive(false); })
                .AddTo(this);

            confirmButton?.OnClickAsObservable()
                .Subscribe(_ => _reqConfirm.OnNext(Unit.Default))
                .AddTo(this);

            resetButton?.OnClickAsObservable()
                .Subscribe(_ => _reqReset.OnNext(Unit.Default))
                .AddTo(this);
        }

        public void Open(IEnumerable<DomAthEntity> allCandidates, HashSet<int> assignedIds)
        {
            gameObject.SetActive(true);
            Clear();

            var list = allCandidates?.Where(a => a.curState != AthleteState.Injured && a.stats.fatigue > 0).ToList() ?? new();
            foreach (var ath in list)
            {
                var ui = Instantiate(itemPrefab, content, false);
                _spawned.Add(ui.gameObject);
                _itemById[ath.id] = ui;

                bool isAssigned = assignedIds != null && assignedIds.Contains(ath.id);
                ui.Bind(ath, isAssigned);

                ui.OnAssign   .Subscribe(_reqAssign.OnNext).AddTo(ui);
                ui.OnUnassign .Subscribe(_reqUnassign.OnNext).AddTo(ui);
            }
        }

        public void UpdateItemAssigned(int athId, bool assigned)
        {
            if (_itemById.TryGetValue(athId, out var ui))
                ui.SetAssigned(assigned);
        }

        public void UpdateAllAssignedFalse()
        {
            foreach (var kv in _itemById) kv.Value.SetAssigned(false);
        }

        public void NudgeAssignButton(int athId)
        {
            if (_itemById.TryGetValue(athId, out var ui))
                ui.NudgeAssignButton();
        }

        private void Clear()
        {
            foreach (var go in _spawned) if (go) Destroy(go);
            _spawned.Clear();
            _itemById.Clear();
        }
    }
}
