using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

namespace JYL
{
    public class DomAthService : MonoBehaviour
    {
        private readonly IDomAthRepository repository;
        private readonly IDisposable subscription;
         
        // 생성자를 통해 Repository를 주입받는다 (Dependency Injection)
        public DomAthService(IDomAthRepository repository)
        {
            this.repository = repository;
            // 초기 작업 이후, 등록된 선수들은 능력치 및 나이에 이벤트 구독 필요
            // subscription = TODO: 선수의 나이에 이벤트 걸기. UniRx R&D 후
        }

        #region 선수 목록
        public List<DomAthEntity> GetAllAthleteList() // 국내 전체 선수 목록 뽑아가기
        {
            return repository.FindAll();
        }

        public List<DomAthEntity> GetRecruitedAthleteList() // 영입된 선수 목록 뽑아가기
        {
            return repository.FindAllRecruited();
        }
        #endregion
        
        #region 선수 영입, 은퇴, 방출
        public void RecruitAthlete(string athleteName) // 새로운 선수를 영입할 때 사용하는 함수
        {
            // 레포지토리에서 Entity를 찾음
            DomAthEntity entity = repository.FindByName(athleteName);
            // Entity의 도메인 로직 실행
            entity.Recruit(); // isRecruited true로 변경
            // Repository를 통해서 변경 사항을 저장한다.
            repository.Save(entity);
        }

        public void RetireAthlete(DomAthEntity entity) // 일반 선수면 그냥 Retired 상태.
                                                       // 후보 이상이면 추가적으로 CoachService에서
                                                       // 코치 동적, 세이브 객체의 상태를 Hidden -> Unrecruited로 변경
        {
            entity.Retire(); // 도메인 로직 수행
            MessageBroker.Default.Publish(new AthleteRetiredEvent(entity.entityName, entity.affiliation)); // 이벤트 발행
        }

        public void OutAthlete(string athleteName) // 선수 방출할 때 쓰는 함수
        {
            // 레포지토리에서 Entity 찾음
            DomAthEntity athlete = repository.FindByName(athleteName);
            // Entity에서 도메인 로직 실행
            athlete.OutAthlete();
            // 레포지토리를 통해서 변경사항을 저장
            repository.Delete(athlete);
        }
        #endregion

        #region 선수 강화
        public void TrainAthlete(in string athleteName, in Ability status, int amount = 1, int coach = 0)
        { //선수 훈련 함수. 정해진 파라매터만 수행 가능 (기획안의 루틴에 따름). 부상이면 선수 강화 함수 수행하면 안됨
            switch (status)
            {
                case Ability.Health :
                case Ability.Quickness :
                case Ability.Flexibility :
                case Ability.Balance :
                    // 선수를 딕셔너리에서 찾고 훈련
                    DomAthEntity athlete = repository.FindByName(athleteName);
                    if (athlete != null || athlete.curState == AthleteState.Injured) return; 
                        
                    athlete.TrainAthlete(status, amount, coach);
                    // 선수 세이브 객체 최신화
                    repository.Update(athlete);
                    break;
                default:
                    Debug.LogWarning($"잘못된 파라매터 입력{status}");
                    break;
            }
        }
        #endregion
        
        
        #region 선수 회복
        // 선수가 회복하는 함수. 파라매터만 변경 하는 것이기 때문에, 결과 처리는 UI에서 필요함. 마찬가지로, 부상 상태가 아니면 수행 못하게 해야함
        public void RecoverAthlete(in string athleteName, int  amount = 1)
        {
            DomAthEntity athlete = repository.FindByName(athleteName);
            if (athlete.curState == AthleteState.Injured && athlete.leftInjury > 0)
            {
                athlete.RecoverAthlete(amount); // 리커버리. 부상을 한 턴 감소.
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
