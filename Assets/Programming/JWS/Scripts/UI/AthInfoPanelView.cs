using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using JYL;
using System.Linq;

public class AthInfoPanelView : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] private TMP_Text titleText;

    [Header("Basic")]
    [SerializeField] private TMP_Text ageText;
    [SerializeField] private TMP_Text stateText;
    [SerializeField] private TMP_Text injuryText;
    [SerializeField] private Image profileImage;

    [Header("Stats")]
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text quickText;
    [SerializeField] private TMP_Text flexText;
    [SerializeField] private TMP_Text technicText;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private TMP_Text balanceText;

    [Inject] private DomAthService _ath;

    public void RenderDefaultIfNoneSelected()
    {
        var e = _ath.GetAllRecruitedAthleteList()            // <-- 수정: 메서드 사용
                   .FirstOrDefault(a => a.curState == AthleteState.Injured);
        Select(e);
    }

    public void Select(DomAthEntity e)
    {
        if (titleText)  titleText.text  = e?.entityName ?? "";
        if (ageText)    ageText.text    = (e?.curAge != null) ? $"{e.curAge.Value}세" : "";
        if (stateText)  stateText.text  = e?.curState.ToString() ?? "";
        if (injuryText) injuryText.text = (e != null && e.curState == AthleteState.Injured) ? $"{e.leftInjury}턴 남음" : "이상 없음";

        if (e?.stats != null)
        {
            if (healthText)  healthText.text  = $"{e.stats.health}";
            if (quickText)   quickText.text   = $"{e.stats.quickness}";
            if (flexText)    flexText.text    = $"{e.stats.flexibility}";
            if (technicText) technicText.text = $"{e.stats.technic}";
            if (speedText)   speedText.text   = $"{e.stats.speed}";
            if (balanceText) balanceText.text = $"{e.stats.balance}";
        }
        else
        {
            if (healthText)  healthText.text  = "0";
            if (quickText)   quickText.text   = "0";
            if (flexText)    flexText.text    = "0";
            if (technicText) technicText.text = "0";
            if (speedText)   speedText.text   = "0";
            if (balanceText) balanceText.text = "0";
        }

        if (profileImage) profileImage.enabled = (profileImage.sprite != null);
    }
}
