using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using EditorAttributes;
using JWS;
using SHG;
using StatefulUI.Runtime.Core;
using UniRx;
using UnityEngine;
using Zenject;

namespace JYL
{
    public class AchievementManager : MonoBehaviour
    {
        [Header("Set References")]
        [SerializeField] private AchievementNotification notification;
        [SerializeField] private AchievementDatabase databaseSo;
        
        [Inject] private ISaveManager saveManager;
        [Inject] private IFacilitiesController  facilitiesController;
        [Inject] private IMatchController matchController;
        
        public List<AchievementController> achievements = new();

        public AchievementWrapper wrapper;
        //
        // // 테스트
        // [Button]
        // void TestNotification()
        // {
        //     notification.gameObject.SetActive(true);
        //     notification.ShowNotification(achievements[0].achieve);
        // }
        
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
            
            foreach (var data in databaseSo.achievements)
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
                    .Where(s => s is not AchievementState.Locked)
                    .Skip(1)
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
                    }).AddTo(this);
                    break;
                case AchievementCondition.스카우트센터업그레이드:
                    facilitiesController.ScoutCenter.CurrentStage.Subscribe(stage =>
                    {
                        if(stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    }).AddTo(this);
                    break;
                case AchievementCondition.훈련센터업그레이드:
                    facilitiesController.TrainingCenter.CurrentStage.Subscribe(stage =>
                    {
                        if (stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    }).AddTo(this);
                    break;
                case AchievementCondition.의료센터업그레이드:
                    facilitiesController.MedicalCenter.CurrentStage.Subscribe(stage =>
                    {
                        if(stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    }).AddTo(this);
                    break;
                case AchievementCondition.숙소업그레이드:
                    facilitiesController.Accomodation.CurrentStage.Subscribe(stage =>
                    {
                        if (stage < 0) throw new ArgumentOutOfRangeException(nameof(stage));
                        controller.progress.Value = stage;
                    }).AddTo(this);
                    break;
                default:
                    Debug.LogWarning($"제대로 된 컨디션이들어오지 못했음.{controller.achieve.Condition}");
                    break;
            }
            // 경기 프로퍼티 구독
            MatchSubscribe();
        }
        
        // 경기 프로퍼티 구독
        void MatchSubscribe()
        {
            matchController.CurrentMatch
                .Where(x => x != null && x.CurrentState.Value == Match.State.Ended)
                .Subscribe(MatchResult).AddTo(this);
        }
        
        // 경기 결과 로직 처리
        void MatchResult(Match match)
        {
            wrapper.MatchEntryCount.Value++; // 경기 참여 카운트 +
                    
            if (match.Data.IsSingleSport) // 단일 경기일 경우
            {
                bool isWin = match.UserResult.GetHighestRank() == 0; // 승리 판단
                if (isWin)
                {
                    wrapper.MatchWinCount.Value++; // 우승 카운트 ++
                }
            }
            else // 종합 경기일 경우
            {
                bool isWin = match.Results.IndexOf(match.UserResult) == 0; // 최종 우승 판단
                if (isWin)
                {
                    wrapper.MatchWinCount.Value++; // 우승 카운트 ++
                }
            }
        }
        
        // 업적의 상태 변동 시 
        void OnStateChanged(AchievementController controller, AchievementState state)
        {
            switch (state)
            {
                // 업적 완료 가능 시, 팝업 UI 재생
                case AchievementState.CanComplete:
                    notification.gameObject.SetActive(true);
                    notification.ShowNotification(controller.achieve);
                    break;
                
                // 업적 완료 시 후행 업적이 있을 경우 해당 업적을 언락한다.
                case AchievementState.Completed:
                    achievements.TryFindValue(cont => cont.achieve.PrevAchievement == controller.achieve.ID,out AchievementController nextAchieve);
                    if (nextAchieve != null)
                    {
                        Debug.Log($"후행 업적 찾음{nextAchieve.achieve.AchName}");
                        nextAchieve.UnlockAchievement();
                    }
                    else
                    {
                        Debug.Log($"해당하는 선행 업적이 없음{controller.achieve.ID}");
                    }
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

