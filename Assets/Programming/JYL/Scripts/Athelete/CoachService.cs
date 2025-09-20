using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks.Triggers;
using UnityEngine;
using UniRx;
using Zenject;

namespace JYL
{
    public class CoachService : MonoBehaviour
    {
        [Inject] private readonly ICoachRepository repository;
        private IDisposable subscription; // 구독 해제를 위한 객체

        [SerializeField] public int[] coachesTest = new int[4];
        private void Awake()
        {
            Init();
        }

        private void Init()
        {
            // 선수 은퇴 시 이벤트 수행을 위한 구독
            subscription = MessageBroker.Default //선수 은퇴 이벤트가 발행되면, 구독해뒀다가 수행
                .Receive<AthleteRetiredEvent>()
                .Where(e => e.affiliation != AthleteAffiliation.일반선수) // 후보급 이상 선수만
                .Subscribe(OnAthleteRetiredEvent); // 코치로 전환 작업

            // 코치 은퇴 구독
            List<CoachEntity> coachList = GetRecruitedCoaches();
            foreach (CoachEntity coach in coachList)
            {
                var age = coach.curAge.ToReadOnlyReactiveProperty();
                age.Where(a => a >= coach.retireAge) // 은퇴 나이일 때 이벤트 수행
                    .TakeWhile(_ =>
                        coach.curState != CoachState.Retired && // 은퇴 전
                        coach.curState != CoachState.Unrecruited) // 방출 전까지 구독
                    .Subscribe(_ => RetireCoach(coach))
                    .AddTo(this);
            }

            coachesTest = repository.FindAllAssigned();
        }

        // TODO: 코치 배치 테스트
        public void RefreshCoaches()
        {
            coachesTest = repository.FindAllAssigned();
        }

        #region 코치 객체, 리스트

        public CoachEntity FindCoachById(int id)
        {
            return repository.FindById(id);
        }
        public List<CoachEntity> GetAllCoaches() // CSV에서 만들어진 모든 코치 리스트
        {
            return repository.FindAllCoaches();
        }
        public List<CoachEntity> GetCanRecruitCoaches() // 플레이어가 영입 가능한 코치 리스트. 영입된 애들, 은퇴한 애들 제외
        {
            return repository.FindAllCanRecruit();
        }

        public List<CoachEntity> GetRecruitedCoaches() // 플레이어가 영입한 코치 리스트. 은퇴 제외
        {
            return repository.FindAllRecruited();
        }

        public List<CoachEntity> GetRetiredCoaches() // 은퇴한 코치 리스트
        {
            return repository.FindAllRetired();
        }

        public int[] GetAssignedCoachesArray() // 현재 배치된 코치들의 entity를 index 기준으로 가져옴
        {
            int[] returnArray = new int[4];
            int[] assignedCoaches = repository.FindAllAssigned();
            for (int i = 0; i < returnArray.Length; i++)
            {
                if (assignedCoaches[i] != -1)
                {
                    returnArray[i] = (int)repository.FindById(assignedCoaches[i]).grade;
                }
                else
                {
                    returnArray[i] = 0;
                }
            }
            return returnArray;
        }

        public CoachEntity[] GetAssignedCoachesEntity()
        {
            CoachEntity[] returnList = new CoachEntity[4];
            int[] assignedCoaches = repository.FindAllAssigned();
            for (int i = 0; i < returnList.Length; i++)
            {
                if (assignedCoaches[i] != -1)
                {
                    returnList[i] = repository.FindById(assignedCoaches[i]);
                }
            }
            return returnList;
        }
        #endregion

        
        #region 코치 영입, 방출, 은퇴
        public void RecruitCoach(CoachEntity entity) // 코치를 영입.
        {
            // 코치의 동적 객체 최신화와 세이브 객체 최신화 진행
            entity.Recruit(); // 도메인 로직 수행
            repository.Save(entity); // 레포지토리를 통해 변경 사항 저장
            
            // 은퇴 이벤트 추가
            var age = entity.curAge.ToReadOnlyReactiveProperty();
            age.Where(a => a >= entity.retireAge) // 은퇴 나이일 때 이벤트 수행
                .TakeWhile(_ =>
                    entity.curState != CoachState.Retired && // 은퇴 전
                    entity.curState != CoachState.Unrecruited) // 방출 전까지 구독
                .Subscribe(_ => RetireCoach(entity))
                .AddTo(this);
        }

        public void OutCoach(CoachEntity entity) // 코치 방출
        {
            // 코치의 동적 객체 최신화와 세이브 객체 최신화 진행
            entity.OutCoach(); // 도메인 로직 수행. 상태만 변경함.Unrecruited
            repository.Delete(entity); // 레포지토리를 통해 변경 사항 저장. 일반급과 후보급 이상이 서로 다른 로직 수행
        }

        private void RetireCoach(CoachEntity entity) // 코치 은퇴
        {
            entity.Retire(); // 객체를 은퇴 상태로 변경
            repository.Update(entity); // 세이브 객체도 변동사항 저장
        }
        #endregion
        
        #region 선수 =>코치 전환
        private void OnAthleteRetiredEvent(AthleteRetiredEvent retiredEvent) // DomAthService에서 발행된 은퇴 이벤트로 인해 수행됨.
        {
            CoachEntity coach = repository.FindByName(retiredEvent.athleteName); // 레포지토리에서 코치 세이브 객체를 이름으로 찾음
            coach.AthleteToCoach(); // 상태 Hidden을 Unrecruited로 변경
            repository.Update(coach); // 코치의 동적 객체를 통해 세이브 객체 최신화
        }
        #endregion
        
    }
}
