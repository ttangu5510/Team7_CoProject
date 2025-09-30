using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SJL;
using UnityEngine;
using UniRx;
using Zenject;
using MMJ;

namespace JYL
{
    public class DomAthService : MonoBehaviour
    {
        [Inject] private readonly IDomAthRepository repository;
        
        private IDisposable subscription;

        
        private void Awake()
        {
            Init();
        }

        private void Init() // 국내 선수 초기화 작업 . 이벤트 구독에 사용
        {
            // 영입한 선수들 중, 은퇴하지 않은 선수들만 리스트화
            List<DomAthEntity> athleteList = GetAllRecruitedAthleteList().Where(t=>t.curState != AthleteState.Retired).ToList();
            foreach (DomAthEntity athlete in athleteList)
            {
                var age = athlete.curAge.ToReadOnlyReactiveProperty();

                // 현재 나이가 은퇴나이보다 높고, 은퇴 상태가 아닐 때
                age.Where(curAge => curAge >= athlete.retireAge)
                    .TakeWhile(_=> athlete.curState != AthleteState.Unrecruited && athlete.curState != AthleteState.Retired) // 방출 전, 은퇴 전까지 구독
                    .Subscribe(x => RetireAthlete(athlete)) // 은퇴 구독
                    .AddTo(this); // 서비스 객체 파괴 시 이벤트 구독 해제
            }
        }

        #region 선수 목록
        public List<DomAthEntity> GetAllAthleteList() // 국내 전체 선수 목록 뽑아가기
        {
            return repository.FindAll();
        }
        public List<DomAthEntity> GetAllCanRecruitAthleteList() // 영입이 가능한 선수들 목록 뽑아가기
        {
            return repository.FindAllCanRecruit();
        }
        public List<DomAthEntity> GetAllRecruitedAthleteList() // 플레이어가 영입한 선수들 목록 뽑아가기. 은퇴한 선수 포함.
        {
            return repository.FindAllRecruited();
        }
        #endregion
        
        #region 선수 영입, 은퇴, 방출, 나이 업데이트
        public void RecruitAthlete(string athleteName) // 새로운 선수를 영입할 때 사용하는 함수
        {
            // 레포지토리에서 Entity를 찾음
            DomAthEntity entity = repository.FindByName(athleteName);
            // Entity의 도메인 로직 실행
            entity.Recruit(); // isRecruited true로 변경
            // Repository를 통해서 변경 사항을 저장한다.
            repository.Save(entity);
            
            // 은퇴 구독 추가
            var age =  entity.curAge.ToReadOnlyReactiveProperty();
            age.Where(ageValue => ageValue >= entity.retireAge)
                .TakeWhile(_ => entity.curState != AthleteState.Unrecruited && entity.curState != AthleteState.Retired)// 방출 전, 은퇴 전까지 구독
                .Subscribe(sendAge => RetireAthlete(entity))
                .AddTo(this);

            // 만준 추가 코드, 영입 이벤트
            MessageBroker.Default.Publish(new AthleteRecruitedEvent(entity.id));
        }

        // 선수 은퇴 함수는 선수의 나이에 의해 자동으로 수행 됨.
        public void RetireAthlete(DomAthEntity entity) // 일반 선수면 그냥 Retired 상태.
                                                       // 후보 이상이면 추가적으로 CoachService에서
                                                       // 코치 동적, 세이브 객체의 상태를 Hidden -> Unrecruited로 변경
        {
            entity.Retire(); // 도메인 로직 수행

            MessageBroker.Default.Publish(new AthleteRetiredEvent(entity.entityName, entity.affiliation, entity.id)); // 이벤트 발행
            // 만준 추가 코드, 은퇴 이벤트
            // MessageBroker.Default.Publish(new AthleteRetiredEvent(entity.id));
        }

        public void OutAthlete(string athleteName) // 선수 방출할 때 쓰는 함수
        {
            // 레포지토리에서 Entity 찾음
            DomAthEntity athlete = repository.FindByName(athleteName);
            // Entity에서 도메인 로직 실행
            athlete.OutAthlete();
            // 레포지토리를 통해서 변경사항을 저장
            repository.Delete(athlete);

            // 만준 추가 코드, 방출 이벤트
            MessageBroker.Default.Publish(new AthleteOutEvent(athlete.id));

        }

        public void AthleteAgeUpdate(DomAthEntity entity) // 선수 나이 업데이트
        {
            entity.GetAge();
            repository.Update(entity);
        }
        #endregion

        #region 선수 훈련, 특훈
        public bool TrainAthlete(DomAthEntity entity, in TrainingType type, int amount = 1, int coach = 0)
        { //선수 훈련 함수. 정해진 파라매터만 수행 가능 (기획안의 루틴에 따름). 부상이면 선수 강화 함수 수행하면 안됨
            if (entity == null || entity.curState == AthleteState.Injured) return false;  //선수가 부상 중이거나 null이면 false
            
            bool isSuccess = false; // 반환에 사용될 boolean
            Ability firstAbility;
            Ability secondAbility;
            switch (type)
            {
                case TrainingType.SpeedSkating :
                    firstAbility = Ability.Quickness;
                    secondAbility = Ability.Technic;
                    break;
                case TrainingType.FigureSkating :
                    firstAbility = Ability.Technic;
                    secondAbility = Ability.Health;
                    break;
                case TrainingType.Skeleton :
                    firstAbility = Ability.Flexibility;
                    secondAbility = Ability.Health;
                    break;
                case TrainingType.SkiJump :
                    firstAbility = Ability.Balance;
                    secondAbility = Ability.Speed;
                    break;
                default:
                    Debug.LogWarning($"잘못된 파라매터 입력{type}");
                    return false;
            }
                
            isSuccess = entity.TrainAthlete(firstAbility, secondAbility, amount, coach);
            // 선수 세이브 객체 최신화
            repository.Update(entity);
            return isSuccess;
        }

        public void ApplySpecialTraining(DomAthEntity entity, int trainingTimes, int amountPerTime)
        {
            entity.SpecialTrain(trainingTimes, amountPerTime);
            repository.Update(entity);
        }
        #endregion
        
        
        #region 선수 회복
        // 선수가 회복하는 함수. 파라매터만 변경 하는 것이기 때문에, 결과 처리는 UI에서 필요함. 마찬가지로, 부상 상태가 아니면 수행 못하게 해야함
        public void RecoverAthlete(DomAthEntity athlete, int amount = 0)
        {
            if (athlete.curState == AthleteState.Injured && athlete.leftInjury > 0)
            {
                athlete.RecoverAthlete(); // 리커버리. 부상을 한 턴 감소.
                athlete.stats.SetFatigue(-amount);
                repository.Update(athlete); // 진행상황을 선수의 세이브 객체에 반영
                Debug.Log($"{athlete.entityName} 부상 회복");
            }
            else
            {
                Debug.LogWarning($"해당 선수는 부상당한 상태가 아님!{athlete.entityName}_isInjured={athlete.curState == AthleteState.Injured}");
            }
        }
        #endregion
    }
}
