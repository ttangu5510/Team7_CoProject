using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JYL
{
    public interface ICoachRepository
    {
        CoachEntity FindByName(string entityName);
        List<CoachEntity> FindAllCanRecruit();
        List<CoachEntity> FindAllRecruited();
        List<CoachEntity> FindAllRetired();
        public void Save(CoachEntity entity);
        public void Delete(CoachEntity entity);
        public void Update(CoachEntity entity);
    }
    public class CoachRepository : ICoachRepository
    {
        private Dictionary<string, CoachEntity> coachDict { get; set; } = new();
        private Test_JYL_SaveManager saveM;

        // 레포지토리 생성 시 사용되는 생성자. 세이브 매니저의 종속성을 주입한다.
        public CoachRepository(Test_JYL_SaveManager saveManager)
        {
            saveM = saveManager;
            // TODO : CSV에서 전체 코치 목록 불러와서 생성
            // var csvData = ;
            // foreach(var row in csvData)
            // {
            //      var entity = CoachFactory.CreateFromCsv(row);
            //      coachDict.Add(entity.name,entity);
            // }
            
            // TODO : 코치 테스트 생산
            for (int i = 0; i < 25; i++)
            {
                CoachEntity entity = CoachFactory.CreateCoachEntity(i); // 팩토리로 객체 생산
                saveM.UpdateCoachEntity(entity); // 세이브 객체가 있으면 그걸로 업데이트
                coachDict[entity.entityName] = entity; // 딕셔너리에 추가
            }
        }
        public CoachEntity FindByName(string entityName) // 이름으로 코치 객체 찾기
        {
            return  coachDict.GetValueOrDefault(entityName);
        }

        public List<CoachEntity> FindAllCanRecruit() // 현재 기준 영입 가능한 코치 리스트
        {
            return coachDict.Values.Where(x => x.curState == CoachState.Unrecruited).ToList();
        }

        public List<CoachEntity> FindAllRecruited() // 현재 기준 영입 된 코치 리스트(은퇴 제외)
        {
            return coachDict.Values.Where(x => x.curState == CoachState.Recruited && x.curState != CoachState.Retired).ToList();
        }

        public List<CoachEntity> FindAllRetired() // 은퇴한 코치들 리스트
        {
            return coachDict.Values.Where(x => x.curState == CoachState.Retired).ToList();
        }

        public void Save(CoachEntity entity) // Service 코치 영입에서 호출
        {

            saveM.RecruitCoach(entity); // 코치 세이브 객체 생성
        }

        public void Delete(CoachEntity entity) // Service 코치 방출에서 호출
        {
            saveM.OutCoach(entity); // 코치의 등급에 따라 로직 달라짐
        }

        public void Update(CoachEntity entity) // Service 코치 업데이트. 세이브 객체를 최신화 함.
        {
            saveM.curSave.FindCoach(entity).UpdateStatus(entity); // 세이브 객체도 업데이트함.
        }
    }
}
