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

        private readonly Subject<DomAthEntity> _onPick = new();
        public IObservable<DomAthEntity> OnPick => _onPick;

        private readonly List<GameObject> _spawned = new();

        void Awake()
        {
            if (closeButton)
            {
                closeButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        if (injuredAthleteInfoPui) injuredAthleteInfoPui.SetActive(false);
                        else gameObject.SetActive(false);
                    })
                    .AddTo(this);
            }
        }

        public void Open(IEnumerable<DomAthEntity> injuredAll, HashSet<int> assignedIds)
        {
            gameObject.SetActive(true);

            Clear();

            var list = injuredAll?
                .Where(a => a.curState == AthleteState.Injured)
                .ToList() ?? new List<DomAthEntity>();

            if (list.Count == 0)
                return;

            foreach (var ath in list)
            {
                var item = Instantiate(itemPrefab, content);
                _spawned.Add(item.gameObject);

                bool isAssigned = assignedIds != null && assignedIds.Contains(ath.id);
                item.Bind(ath, isAssigned);

                item.OnAssign
                    .TakeUntilDestroy(item)
                    .Subscribe(_onPick.OnNext)
                    .AddTo(item);

                item.OnOpenInfo
                    .TakeUntilDestroy(item)
                    .Subscribe(ShowInfo)
                    .AddTo(item);
            }
        }

        private void ShowInfo(DomAthEntity ath)
        {
            if (!infoPanel.gameObject.activeSelf) 
                infoPanel.gameObject.SetActive(true);

            infoPanel.transform.SetAsLastSibling();
            infoPanel.Open(ath);
        }

        private void Clear()
        {
            foreach (var go in _spawned) 
                if (go) Destroy(go);
            _spawned.Clear();
        }
    }
}
