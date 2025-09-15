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
        [SerializeField] private Button closeButton;             // X (루트 닫기)

        [Header("List")]
        [SerializeField] private Transform content;              // ScrollView/Viewport/Content
        [SerializeField] private InjureListItemUI itemPrefab;
        [SerializeField] private CanvasGroup listCanvasGroup;    // 리스트 입력 차단용
        [SerializeField] private InjureAthInfoPanel infoPanel;   // 형제 상세 패널
        [SerializeField] private GameObject injuredAthleteInfoPUI; // 팝업 루트

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
                        if (injuredAthleteInfoPUI) injuredAthleteInfoPUI.SetActive(false);
                        else gameObject.SetActive(false);
                    })
                    .AddTo(this);
            }
        }

        public void Open(IEnumerable<DomAthEntity> injuredAll, HashSet<int> assignedIds)
        {
            gameObject.SetActive(true);

            // 리스트 리빌드
            Clear();

            var list = injuredAll?
                .Where(a => a.curState == AthleteState.Injured)
                .ToList() ?? new List<DomAthEntity>();

            if (list.Count == 0)
            {
                SetListInteractable(true);
                return;
            }

            foreach (var ath in list)
            {
                var item = Instantiate(itemPrefab, content);
                bool isAssigned = assignedIds != null && assignedIds.Contains(ath.id);
                item.Bind(
                    ath,
                    isAssigned,
                    onAssign: a => { _onPick.OnNext(a); }, // 배치하기
                    onOpenInfo: a => ShowInfo(a)           // 상세 덮기
                );
                _spawned.Add(item.gameObject);
            }

            SetListInteractable(true);
        }

        private void ShowInfo(DomAthEntity ath)
        {
            // 리스트 보이되 입력 차단(깜빡임 없음)
            SetListInteractable(false);

            infoPanel.transform.SetAsLastSibling();
            infoPanel.Open(ath);

            // 상세 X 닫히면 입력 복구 (이 구독은 전환 시점에 1회만 붙임)
            infoPanel.OnClosed
                .Take(1)
                .Subscribe(_ => SetListInteractable(true))
                .AddTo(this);
        }

        private void SetListInteractable(bool on)
        {
            if (!listCanvasGroup) return;
            listCanvasGroup.interactable = on;
            listCanvasGroup.blocksRaycasts = on;
        }

        private void Clear()
        {
            foreach (var go in _spawned) if (go) Destroy(go);
            _spawned.Clear();
        }
    }
}

