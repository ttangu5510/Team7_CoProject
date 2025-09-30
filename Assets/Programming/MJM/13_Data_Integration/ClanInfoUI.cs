using TMPro;
using UnityEngine;
using Zenject;
using System.Linq;
using JWS;
using JYL;
using SHG;
using UniRx;

public class ClanInfoUI : MonoBehaviour
{
   int goldMedal;
   int silverMedal;
   int bronzeMedal;
  
   [Inject] private SaveManager saveManager;
   [Inject] private DomAthService domAthService;
   [Inject] private CoachService coachService;
   [Inject] private IResourceController resourceController;
   private MatchController matchController;
  
  
   [SerializeField] private TextMeshProUGUI clanNameText;
   [SerializeField] private TextMeshProUGUI foundedDateText;
   [SerializeField] private TextMeshProUGUI ownedAthleteCountText;
   [SerializeField] private TextMeshProUGUI retiredAthleteCountText;
   [SerializeField] private TextMeshProUGUI retiredCoachCountText;
   [SerializeField] private TextMeshProUGUI matchEntryCountText;
   [SerializeField] private TextMeshProUGUI fameText;
   [SerializeField] private TextMeshProUGUI achievementCountText;
  
   [SerializeField] private TextMeshProUGUI medalCountText;
  
   private void OnEnable()
   {
       Refresh(); // 창이 열릴 때 갱신
   }
  
   private void Start()
   {
       // 경기가 끝날 때 구독
       matchController.CurrentMatch
      .Where(x => x != null && x.CurrentState.Value == Match.State.Ended)
      .Subscribe(MedalCount).AddTo(this);
   }
  
   private void Refresh()
   {
       var save = saveManager.GetCurrentSave();
       if (save == null) return;
  
       // 선수단 이름
       clanNameText.text = save.clanName;
  
       // 선수단 창단일
       if (!string.IsNullOrEmpty(save.foundedUtcIso))
       {
           foundedDateText.text = $"창단일: {save.foundedUtcIso}";
       }
       else
       {
           foundedDateText.text = "창단일 기록 없음";
       }
  
       // 현재 보유 선수 수
       int athleteCount = domAthService.GetAllRecruitedAthleteList().Count(ath => ath.curState != AthleteState.Retired);
       ownedAthleteCountText.text = $"{athleteCount}명";
  
       // 은퇴 선수 수
       int retiredAthleteCount = domAthService.GetAllCanRecruitAthleteList().Count(ath => ath.curState == AthleteState.Retired);
       retiredAthleteCountText.text = $"{save.achievementRecord.athleteRetireCount}명";
  
       // 은퇴 코치 수
       int retiredCoachCount = coachService.GetRetiredCoaches().Count();
       retiredCoachCountText.text = $"{retiredCoachCount}명";
  
       // 경기 참여 수
       matchEntryCountText.text = $"{save.achievementRecord.matchEntryCount}회";
  
       // 명성~~ 근데 명성이 뭐야?
       fameText.text = $"{resourceController.Fame.Value}";
  
       // 달성한 업적 수~ 
       achievementCountText.text = $"{save.achievements.Count(a => a.state == AchievementState.Completed)}개";
  
       // 획득한 메달 수 (금, 은, 동 각각 모두 합산한 수치)
       medalCountText.text = $"금 : {goldMedal}, 은 : {silverMedal}, 동 : {bronzeMedal}";
   }
  
   private void MedalCount(Match match)
   {
       // 메달 갯수
       int[] madalCount = match.UserResult.GetMedalCounts();
   
       goldMedal = madalCount[0];
       silverMedal = madalCount[1];
       bronzeMedal = madalCount[2];
   }

}