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
        [SerializeField] private InjureAthInfoPanel infoPanel;
        [SerializeField] private RestResultPanel restResultPanel;
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

        private readonly List<RestAthleteItem> _items = new();
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

            var list = allCandidates?.Where(a => a.stats.fatigue > 0).OrderByDescending(a => a.stats.fatigue).ToList()
                       ?? new List<DomAthEntity>();
            
            if (!content.gameObject.activeSelf) content.gameObject.SetActive(true);
            
            // 피로도 ≥1만, 피로도 높은 순으로 표시
            foreach (var ath in list.OrderByDescending(a => a.stats.fatigue))
            {
                if (ath.stats.fatigue <= 0) continue;

                var ui = Instantiate(itemPrefab, content, false);
                if (!ui.gameObject.activeSelf) ui.gameObject.SetActive(true); // ★ prefab 비활성 강제 ON

                _spawned.Add(ui.gameObject);
                _items.Add(ui);
                _itemById[ath.id] = ui;

                bool isAssigned = assignedIds != null && assignedIds.Contains(ath.id);
                ui.Bind(ath, isAssigned);

                ui.OnAssign   .Subscribe(_reqAssign.OnNext)   .AddTo(ui);
                ui.OnUnassign .Subscribe(_reqUnassign.OnNext) .AddTo(ui);
                ui.OnOpenInfo .Subscribe(a => ShowInfo(a))    .AddTo(ui);
            }

            
            // 레이아웃 강제 갱신(ScrollView/VerticalLayout/SizeFitter 모두 반영)
            Canvas.ForceUpdateCanvases();
            var rt = content as RectTransform;
            if (rt != null) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            Canvas.ForceUpdateCanvases();
        }

        // 오버로드 추가
        public void Open(IEnumerable<DomAthEntity> source,
            HashSet<int> restAssigned,
            HashSet<int> treatmentAssigned)
        {
            gameObject.SetActive(true);
            Clear();

            var list = source?.ToList() ?? new List<DomAthEntity>();
            if (!content.gameObject.activeSelf) content.gameObject.SetActive(true);

            foreach (var ath in list.OrderByDescending(a => a.stats.fatigue))
            {
                var ui = Instantiate(itemPrefab, content, false);
                if (!ui.gameObject.activeSelf) ui.gameObject.SetActive(true);

                _spawned.Add(ui.gameObject);
                _items.Add(ui);
                _itemById[ath.id] = ui;

                bool inTreatment = treatmentAssigned != null && treatmentAssigned.Contains(ath.id);
                bool inRest      = restAssigned      != null && restAssigned.Contains(ath.id);

                // 의료센터 배치 우선
                bool isAssigned = inTreatment || inRest;
                ui.Bind(ath, isAssigned);

                // (선택) 배치 사유 뱃지 표기 지원 시
                // if (inTreatment) ui.SetAssignedReason("의료센터 배치중");
                // else if (inRest) ui.SetAssignedReason("휴게실 배치중");

                ui.OnAssign   .Subscribe(_reqAssign.OnNext)   .AddTo(ui);
                ui.OnUnassign .Subscribe(_reqUnassign.OnNext) .AddTo(ui);
                ui.OnOpenInfo .Subscribe(a => ShowInfo(a))    .AddTo(ui);
            }

            Canvas.ForceUpdateCanvases();
            var rt = content as RectTransform;
            if (rt != null) UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            Canvas.ForceUpdateCanvases();
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
