using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JWS;
using SHG;
using UniRx;
using UnityEngine;
using Zenject;

namespace JYL
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField] private AchievementDatabase database;
        
        [Inject] private ISaveManager saveManager;
        [Inject] private IFacilitiesController  facilitiesController;
        
        private List<AchievementController> achievements = new();

        private AchievementWrapper wrapper;
        
        #region 라이프사이클
        void OnEnable()
        {
            achievements.Clear();
            achievements = GetControllerList();
        }
        
        // 비활성화 될 때 컨트롤러들 구독해제.
        void OnDisable()
        {
            foreach (var controller in achievements)
            {
                controller.OnDestroy();
            }
        }
        #endregion

        #region 초기화
        // 초기화 작업. 업적 DB를 통해 동적 업적 객체들의 리스트 생성.
        private List<AchievementController> GetControllerList()
        {
            List<AchievementController> result = new(); // 결과로 반환할 리스트 생성
            SaveData save = saveManager.GetCurrentSave(); // 현재 세이브 파일을 불러옴.
            wrapper = saveManager.GetAchievementWrapper();
            
            foreach (var data in database.achievements)
            {
                // id를 통해 세이브 파일 객체가 있는지 찾음.
                Debug.Log($"데이터 ID{data.ID}");
                AchievementSave achSave = save.FindAchievementSaveByID(data.ID);
                // 데이터베이스의 업적 객체와 세이브데이터의 업적 객체를 통해 동적 업적 객체 생성
                result.Add(new AchievementController(data, achSave)); 
            }

            // 위에서 생성된 동적 업적 객체들을 통해 이벤트 구독 처리함.
            foreach (var controller in result)
            {
                controller.state
                    .Where(s => s is not (AchievementState.Locked or AchievementState.Completed))
                    .Subscribe(state =>OnStateChanged(controller, state))
                    .AddTo(this); // 상태 변화를 구독함.
                
                SubscribeAchievement(controller);
            }
            
            return result;
        }
        #endregion
        
        #region 이벤트 구독
        // 각 업적들의 상태에 따른 세이브데이터를 통한 progress의 최신화
        void SubscribeAchievement(AchievementController controller)
        {
            switch(controller.achieve.Condition)
            {
                case AchievementCondition.경기참가:
                    wrapper.MatchEntryCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.우승:
                    wrapper.MatchWinCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.훈련진행:
                    wrapper.TrainCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.회복진행:
                    wrapper.RecoverCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.특훈진행:
                    wrapper.SpecialTrainCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.선수영입:
                    wrapper.AthleteRecruitCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.선출코치영입:
                    wrapper.CoachRecruitCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.선수은퇴:
                    wrapper.AthleteRetireCount
                        .Subscribe(count => controller.progress.Value = count)
                        .AddTo(this);
                    break;
                case AchievementCondition.휴게실업그레이드:
                    facilitiesController.Lounge.CurrentStage.Subscribe(stage =>
                    {
                        if (stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    });
                    break;
                case AchievementCondition.스카우트센터업그레이드:
                    facilitiesController.ScoutCenter.CurrentStage.Subscribe(stage =>
                    {
                        if(stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    });
                    break;
                case AchievementCondition.훈련센터업그레이드:
                    facilitiesController.TrainingCenter.CurrentStage.Subscribe(stage =>
                    {
                        if (stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    });
                    break;
                case AchievementCondition.의료센터업그레이드:
                    facilitiesController.MedicalCenter.CurrentStage.Subscribe(stage =>
                    {
                        if(stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    });
                    break;
                case AchievementCondition.숙소업그레이드:
                    facilitiesController.Accomodation.CurrentStage.Subscribe(stage =>
                    {
                        if (stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    });
                    break;
                default:
                    Debug.LogWarning($"제대로 된 컨디션이들어오지 못했음.{controller.achieve.Condition}");
                    break;
            }
        }
        
        // 업적의 상태 변동 시 
        void OnStateChanged(AchievementController controller, AchievementState state)
        {
            switch (state)
            {
                case AchievementState.CanComplete:
                    // TODO : 업적 완료 토스트 팝업
                    break;
                case AchievementState.Completed:
                    AchievementController achievementControllers = achievements.Where(cont => cont.achieve.PrevAchievement == controller.achieve.ID) as AchievementController;
                    if (achievementControllers != null) achievementControllers.UnlockAchievement();
                    else Debug.Log($"해당하는 선행 업적이 없음{controller.achieve.ID}");
                    break;
            }
        }
        #endregion
        
        #region 외부 접근용 기능
        public List<AchievementController> GetAchievements()
        {
            return achievements;
        }
        #endregion
    }
}

