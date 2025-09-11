// Assets/Programming/JWS/Scripts/UI/TreatmentRoomTabView.cs
using System;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using JYL; // DomAthEntity, AthleteState

namespace JSW
{
    [DisallowMultipleComponent]
    public sealed class TreatmentRoomTabView : MonoBehaviour
    {
        private const int CAPACITY = 8; // 인스펙터 비노출, 고정

        [Header("UI")]
        [SerializeField] private Text totalCountText;   // "8" 고정 표시
        [SerializeField] private Text injuredCountText; // 부상자 수 표시

        private readonly CompositeDisposable _cd = new();

        private void OnEnable()
        {
            if (totalCountText) totalCountText.text = CAPACITY.ToString();
        }

        private void OnDisable() => _cd.Clear();

        // --- 바인딩 방법 1: '부상자 리스트' 스트림을 그대로 주입 ---
        public void Bind(IObservable<System.Collections.Generic.IReadOnlyList<DomAthEntity>> injuredStream)
        {
            _cd.Clear();
            injuredStream
                .Select(list => list?.Count ?? 0)
                .Subscribe(UpdateInjuredCount)
                .AddTo(_cd);
        }

        // --- 바인딩 방법 2: '전체 선수' 스트림만 있으면 여기로 주입 ---
        public void BindFromAll(IObservable<System.Collections.Generic.IReadOnlyList<DomAthEntity>> allStream)
        {
            _cd.Clear();
            allStream
                .Select(list => list == null ? 0 : list.Count(a => a.curState == AthleteState.Injured))
                .Subscribe(UpdateInjuredCount)
                .AddTo(_cd);
        }

        // --- 바인딩 방법 3: 스트림 없이 외부에서 숫자만 갱신 ---
        public void SetInjuredCount(int count) => UpdateInjuredCount(count);

        private void UpdateInjuredCount(int count)
        {
            if (injuredCountText) injuredCountText.text = count.ToString();
        }
    }
}