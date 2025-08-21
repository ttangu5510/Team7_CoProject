using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class CoachService : MonoBehaviour
    {
        private readonly ICoachRepository repository;
        
        // 생성자를 통해 Repository를 주입함 (Dependency Injection)
        public CoachService(ICoachRepository repository)
        {
            this.repository = repository;
        }
        
        #region 코치 리스트
        public List<CoachEntity> GetCanRecruitCoaches() // 영입 가능한 코치 리스트
        {
            return repository.FindAllCanRecruit();
        }

        public List<CoachEntity> GetRecruitedCoaches() // 영입한 코치 리스트
        {
            return repository.FindAllRecruited();
        }

        public List<CoachEntity> GetRetiredCoaches() // 은퇴한 코치 리스트
        {
            return repository.FindAllRetired();
        }
        #endregion

        
        #region 코치 영입, 방출, 은퇴
        public void RecruitCoach(string coachName) // 코치를 영입.
        {
            CoachEntity entity = repository.FindByName(coachName); // 레포지토리에서 객체 찾음
            entity.Recruit(); // 도메인 로직 수행
            repository.Save(entity); // 레포지토리를 통해 변경 사항 저장
        }

        public void OutCoach(string coachName) // 코치 방출
        {
            CoachEntity entity = repository.FindByName(coachName);
            entity.OutCoach(); // 도메인 로직 수행. 상태만 변경함.Unrecruited
            repository.Delete(entity); // 레포지토리를 통해 변경 사항 저장. 일반급과 후보급 이상이 서로 다른 로직 수행
        }

        public void RetireCoach(string coachName) // 코치 은퇴
        {
            CoachEntity entity = repository.FindByName(coachName);
            entity.Retire(); // 객체를 은퇴 상태로 변경
            repository.Update(entity); // 세이브 객체도 변동사항 저장
        }
        #endregion
    }
}
