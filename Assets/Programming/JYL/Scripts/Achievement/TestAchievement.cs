using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace JYL
{
    public class TestAchievement : MonoBehaviour
    {
        [Inject] private ISaveManager saveManager;
        [Inject] private AchievementManager achievementManager;
        
        
        [Header("Set Dropdown")]
        [SerializeField] private TMP_Dropdown dropdown;
        
        [Header("Set Buttons")]
        [SerializeField] private Button entryMatch;
        [SerializeField] private Button winMatch;
        [SerializeField] private Button upTrainCount;
        [SerializeField] private Button upRecoverCount;
        [SerializeField] private Button upSpecialTrainCount;
        [SerializeField] private Button athleteRecruitCount;
        [SerializeField] private Button coachRecruitCount;
        [SerializeField] private Button athleteRetireCount;
        
        [Header("Set Texts")]
        [SerializeField] private TextMeshProUGUI textEntryMatch;
        [SerializeField] private TextMeshProUGUI textWinMatch;
        [SerializeField] private TextMeshProUGUI textUpTrainCount;
        [SerializeField] private TextMeshProUGUI textUpRecoverCount;
        [SerializeField] private TextMeshProUGUI textUpSpecialTrainCount;
        [SerializeField] private TextMeshProUGUI textAthleteRecruitCount;
        [SerializeField] private TextMeshProUGUI textCoachRecruitCount;
        [SerializeField] private TextMeshProUGUI textAthleteRetireCount;
        
        private List<AchievementController> controllers = new();
        private AchievementWrapper wrapper;
        
        void Start()
        {
            controllers = achievementManager.GetAchievements();
            wrapper = saveManager.GetAchievementWrapper();
            dropdown.options.Clear();
            dropdown.AddOptions(controllers.Select(x=>x.achieve.AchName).ToList());
            Subscribe();
        }

        void Subscribe()
        {
            entryMatch.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.MatchEntryCount.Value++;
                    textEntryMatch.text = wrapper.MatchEntryCount.Value.ToString();
                })
                .AddTo(this);
            
            winMatch.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.MatchWinCount.Value++;
                    textWinMatch.text = wrapper.MatchWinCount.Value.ToString();
                })
                .AddTo(this);
            
            upTrainCount.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.TrainCount.Value++;
                    textUpTrainCount.text = wrapper.TrainCount.Value.ToString();
                })
                .AddTo(this);
            
            upRecoverCount.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.RecoverCount.Value++;
                    textUpRecoverCount.text = wrapper.RecoverCount.Value.ToString();
                })
                .AddTo(this);
            
            upSpecialTrainCount.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.SpecialTrainCount.Value++;
                    textUpSpecialTrainCount.text = wrapper.SpecialTrainCount.Value.ToString();
                })
                .AddTo(this);
            
            athleteRecruitCount.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.AthleteRecruitCount.Value++;
                    textAthleteRecruitCount.text = wrapper.AthleteRecruitCount.Value.ToString();
                })
                .AddTo(this);
            
            coachRecruitCount.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.CoachRecruitCount.Value++;
                    textCoachRecruitCount.text = wrapper.CoachRecruitCount.Value.ToString();
                })
                .AddTo(this);
            
            athleteRetireCount.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    wrapper.AthleteRetireCount.Value++;
                    textAthleteRetireCount.text = wrapper.AthleteRetireCount.Value.ToString();
                })
                .AddTo(this);
        }
    }
}

