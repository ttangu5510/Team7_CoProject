using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using JYL;

namespace JSW
{
    public sealed class InjuredAthInfoPanelView : MonoBehaviour
    {
        [Inject] private IDomAthReadModel _ath;

        [SerializeField] private int athleteId; // 외부에서 SetTarget으로 주입

        [Header("기본 정보")]
        [SerializeField] private Text nameText;
        [SerializeField] private Text ageText;
        [SerializeField] private Text stateText;
        [SerializeField] private Text leftInjuryText;

        [Header("스탯(옵션)")]
        [SerializeField] private Text healthText;
        [SerializeField] private Text quicknessText;
        [SerializeField] private Text flexibilityText;
        [SerializeField] private Text technicText;
        [SerializeField] private Text speedText;
        [SerializeField] private Text balanceText;
        [SerializeField] private Text fatigueText;

        private readonly CompositeDisposable _cd = new();

        public void SetTarget(int id)
        {
            athleteId = id;
            _cd.Clear();
            Bind();
        }

        private void OnEnable() => Bind();
        private void OnDisable() => _cd.Clear();

        private void Bind()
        {
            if (athleteId <= 0) return;

            _ath.ObserveById(athleteId)
                .Subscribe(Render)
                .AddTo(_cd);
        }

        private void Render(DomAthEntity a)
        {
            if (a == null) return;

            // 기본
            if (nameText) nameText.text = a.entityName;
            if (ageText)  ageText.text  = a.curAge != null ? a.curAge.Value.ToString() : "-";
            if (stateText) stateText.text = StateToKorean(a.curState);

            if (leftInjuryText)
            {
                leftInjuryText.text = a.curState == AthleteState.Injured
                    ? $"잔여 {a.leftInjury}턴"
                    : "-";
            }

            // 스탯(필드가 존재할 때만 표시; null 체크)
            if (a.stats != null)
            {
                TrySet(healthText,      a.stats.health);
                TrySet(quicknessText,   a.stats.quickness);
                TrySet(flexibilityText, a.stats.flexibility);
                TrySet(technicText,     a.stats.technic);
                TrySet(speedText,       a.stats.speed);
                TrySet(balanceText,     a.stats.balance);
                TrySet(fatigueText,     a.stats.fatigue);
            }
        }

        private static void TrySet(Text t, int value)
        {
            if (t) t.text = value.ToString();
        }

        private static string StateToKorean(AthleteState s) => s switch
        {
            AthleteState.Unrecruited => "미영입",
            AthleteState.Active      => "정상",
            AthleteState.Injured     => "부상",
            AthleteState.Retired     => "은퇴",
            _                        => "-"
        };
    }
}
