using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using JYL;
using UnityEngine.UI;

namespace JWS
{
    public class InjureListPanel : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private Button closeButton;                 // X (루트 닫기)

        [Header("List")]
        [SerializeField] private Transform content;                  // ScrollView/Viewport/Content
        [SerializeField] private InjureListItemUI itemPrefab;
        [SerializeField] private InjureAthInfoPanel infoPanel;       // 형제 상세 패널
        [SerializeField] private GameObject injuredAthleteInfoPui;   // 팝업 루트
        
        [Header("Footer")]
        [SerializeField] private Button confirmButton; // ✓ 확정
        [SerializeField] private Button resetButton;   // ↺ 초기화

        private readonly Subject<DomAthEntity> _reqAssign   = new();
        private readonly Subject<DomAthEntity> _reqUnassign = new();
        private readonly Subject<Unit>         _reqReset    = new();
        private readonly Subject<Unit>         _reqConfirm  = new();

        public IObservable<DomAthEntity> OnRequestAssign   => _reqAssign;
        public IObservable<DomAthEntity> OnRequestUnassign => _reqUnassign;
        public IObservable<Unit>         OnRequestReset    => _reqReset;
        public IObservable<Unit>         OnRequestConfirm  => _reqConfirm;

        private readonly List<InjureListItemUI> _items = new();
        private readonly Dictionary<int, InjureListItemUI> _itemById = new();
        private readonly List<GameObject> _spawned = new();

        private void Awake()
        {
            closeButton?.OnClickAsObservable()
                .Subscribe(_ => { if (injuredAthleteInfoPui) injuredAthleteInfoPui.SetActive(false); else gameObject.SetActive(false); })
                .AddTo(this);

            confirmButton?.OnClickAsObservable()
                .Subscribe(_ => _reqConfirm.OnNext(Unit.Default))
                .AddTo(this);

            resetButton?.OnClickAsObservable()
                .Subscribe(_ => _reqReset.OnNext(Unit.Default))
                .AddTo(this);
        }

        /// <summary>
        /// injuredAll: 부상자 전체, assignedIds: 이미 슬롯에 배치된 선수들
        /// slotIndex: 어떤 슬롯에 반영할지(확정시 사용)
        /// </summary>
        public void Open(IEnumerable<DomAthEntity> injuredAll, HashSet<int> assignedIds)
        {
            gameObject.SetActive(true);
            Clear();

            var list = injuredAll?.Where(a => a.curState == AthleteState.Injured).ToList() ?? new List<DomAthEntity>();
            foreach (var ath in list)
            {
                var ui = Instantiate(itemPrefab, content);
                _spawned.Add(ui.gameObject);
                _items.Add(ui);
                _itemById[ath.id] = ui;

                bool isAssigned = assignedIds != null && assignedIds.Contains(ath.id);
                ui.Bind(ath, isAssigned);

                ui.OnAssign   .Subscribe(_reqAssign.OnNext)   .AddTo(ui);
                ui.OnUnassign .Subscribe(_reqUnassign.OnNext) .AddTo(ui);
                ui.OnOpenInfo .Subscribe(a => ShowInfo(a))    .AddTo(ui);
            }
        }

        private void ShowInfo(DomAthEntity ath)
        {
            if (!infoPanel.gameObject.activeSelf) infoPanel.gameObject.SetActive(true);
            infoPanel.transform.SetAsLastSibling();
            infoPanel.Open(ath);
        }

        public void UpdateItemAssigned(int athId, bool assigned)
        {
            if (_itemById.TryGetValue(athId, out var ui))
                ui.SetAssigned(assigned);
        }

        public void NudgeAssignButton(int athId)
        {
            if (_itemById.TryGetValue(athId, out var ui))
                ui.NudgeAssignButton();
        }

        public void UpdateAllAssignedFalse()
        {
            foreach (var kv in _itemById) kv.Value.SetAssigned(false);
        }

        private void Clear()
        {
            foreach (var go in _spawned) if (go) Destroy(go);
            _spawned.Clear();
            _items.Clear();
            _itemById.Clear();
        }
    }
}
