using TMPro;
using UnityEngine;
using Zenject;
using System.Linq;
using JWS;
using JYL;
using SHG;

public class ClanInfoUI : MonoBehaviour
{
    [Inject] private SaveManager saveManager;
    [Inject] private DomAthService domAthService;
    [Inject] private IResourceController resourceController;


    [SerializeField] private TextMeshProUGUI clanNameText;
    [SerializeField] private TextMeshProUGUI foundedDateText;
    [SerializeField] private TextMeshProUGUI ownedAthleteCountText;
    [SerializeField] private TextMeshProUGUI retiredAthleteCountText;
    [SerializeField] private TextMeshProUGUI retiredCoachCountText;
    [SerializeField] private TextMeshProUGUI matchEntryCountText;
    [SerializeField] private TextMeshProUGUI fameText;
    [SerializeField] private TextMeshProUGUI achievementCountText;


    private void OnEnable()
    {
        Refresh(); // 창이 열릴 때 갱신
    }

    private void Refresh()
    {
        var save = saveManager.GetCurrentSave();
        if (save == null) return;

        clanNameText.text = save.clanName;

        if (!string.IsNullOrEmpty(save.foundedUtcIso))
        {
            foundedDateText.text = $"창단일: {save.foundedUtcIso}";
        }
        else
        {
            foundedDateText.text = "창단일 기록 없음";
        }

        int athleteCount = domAthService.GetAllRecruitedAthleteList().Count(ath => ath.curState != AthleteState.Retired);
        ownedAthleteCountText.text = $"{athleteCount}명";
        

        retiredAthleteCountText.text = $"{save.achievementRecord.athleteRetireCount}명";
        retiredCoachCountText.text = $"{save.achievementRecord.coachRetireCount}명";
        matchEntryCountText.text = $"{save.achievementRecord.matchEntryCount}회";
        fameText.text = $"{resourceController.Fame.Value}";

        achievementCountText.text = $"{save.achievements.Count(a => a.state == AchievementState.Completed)}개";
    }
}
