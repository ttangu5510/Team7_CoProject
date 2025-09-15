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
        [SerializeField] private Transform content;               // ScrollView/Viewport/Content
        [SerializeField] private InjureListItemUI itemPrefab;     // 프리팹
        [SerializeField] private InjureAthInfoPanel infoPanel;    // 필요 없다면 제거 가능

        private readonly List<GameObject> _spawned = new();

        // ★ 외부에서 구독할 수 있는 이벤트
        private readonly Subject<DomAthEntity> _onPick = new();
        public IObservable<DomAthEntity> OnPick => _onPick;

        /// <param name="injuredAll">부상자 전체 리스트</param>
        /// <param name="assignedIds">이미 치료실에 배치된 선수 id 집합</param>
        public void Open(IEnumerable<DomAthEntity> injuredAll, HashSet<int> assignedIds)
        {
            gameObject.SetActive(true);
            Clear();

            var list = injuredAll?.Where(a => a.curState == AthleteState.Injured).ToList() ?? new();
            if (list.Count == 0) return;

            foreach (var ath in list)
            {
                var item = Instantiate(itemPrefab, content);
                bool isAssigned = assignedIds != null && assignedIds.Contains(ath.id);
                item.Bind(ath, isAssigned, OnClickAssign);
                _spawned.Add(item.gameObject);
            }
        }

        private void OnClickAssign(DomAthEntity ath)
        {
            // 외부(TreatmentRoomTabView 등)에 알림
            _onPick.OnNext(ath);

            // 만약 아이템 클릭 시 패널 닫고 싶다면:
            gameObject.SetActive(false);

            // 상세 패널 열고 싶다면:
            // if (infoPanel != null) { infoPanel.Open(ath); }
        }

        private void Clear()
        {
            foreach (var go in _spawned) if (go) Destroy(go);
            _spawned.Clear();
        }
    }

    
}

