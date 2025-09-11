using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using JYL; // DomAthEntity, AthleteState

namespace JSW
{
    public sealed class InjuredRowView : MonoBehaviour
    {
        [SerializeField] private Text nameText;
        [SerializeField] private Text stateText;
        [SerializeField] private Text leftInjuryText;

        public void Bind(DomAthEntity a)
        {
            if (nameText) nameText.text = a.entityName; // BaseAthEntity에 entityName 존재

            if (stateText) stateText.text = StateToKorean(a.curState);

            if (leftInjuryText)
            {
                leftInjuryText.text = a.curState == AthleteState.Injured
                    ? $"잔여 {a.leftInjury}턴"
                    : "-";
            }
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