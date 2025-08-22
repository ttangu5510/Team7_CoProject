using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public interface ICoachRepository
    {
        CoachEntity FindByName(string entityName);
        List<CoachEntity> FindAllCanRecruit();
        List<CoachEntity> FindAllRecruited();
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
            
            for (int i = 0; i < 25; i++)
            {
                CoachEntity entity = Coach
            }
        }
        public CoachEntity FindByName(string entityName)
        {
            throw new System.NotImplementedException();
        }

        public List<CoachEntity> FindAllCanRecruit()
        {
            throw new System.NotImplementedException();
        }

        public List<CoachEntity> FindAllRecruited()
        {
            throw new System.NotImplementedException();
        }

        public void Save(CoachEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(CoachEntity entity)
        {
            throw new System.NotImplementedException();
        }

        public void Update(CoachEntity entity)
        {
            throw new System.NotImplementedException();
        }
    }
}
