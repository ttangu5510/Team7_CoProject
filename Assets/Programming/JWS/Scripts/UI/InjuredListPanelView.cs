using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;
using JYL; // DomAthEntity

namespace JSW
{
    public sealed class InjuredListPanelView : MonoBehaviour
    {
        //[Inject] private IDomAthReadModel _ath;

        [SerializeField] private Transform content;        // ScrollView Content
        [SerializeField] private InjuredRowView rowPrefab; // 아이템 프리팹

        private readonly CompositeDisposable _cd = new();
        private readonly List<GameObject> _pool = new();

        private void OnEnable()
        {
            //_ath.ObserveInjured()
            //    .Subscribe(Render)
            //    .AddTo(_cd);
        }

        private void OnDisable() => _cd.Clear();

        private void Render(IReadOnlyList<DomAthEntity> list)
        {
            foreach (var go in _pool) go.SetActive(false);

            while (_pool.Count < list.Count)
                _pool.Add(Instantiate(rowPrefab, content).gameObject);

            for (int i = 0; i < list.Count; i++)
            {
                var row = _pool[i].GetComponent<InjuredRowView>();
                row.gameObject.SetActive(true);
                row.Bind(list[i]);
            }
        }
    }
}