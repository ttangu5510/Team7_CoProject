using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace JYL
{
    public interface IDomAthRepository
    {
        DomAthEntity FindByName(string name);
        List<DomAthEntity> FindAll();
        List<DomAthEntity> FindAllRecruited();
        void Save(DomAthEntity entity);
        void Delete(DomAthEntity entity);
    }
    public class DomAthRepository : IDomAthRepository // 선수들의 정보를 보관하는 레포지토리 
    {
        private Dictionary<string,DomAthEntity> athleteDict { get; set; } = new();

        // 테스트 세이브 매니저. 전역 접근 필요함(Zenject)
        private Test_JYL_SaveManager saveM;

        // 국내 선수 전체 숫자만큼(CSV의 캐릭터 숫자만큼) 만들어서 딕셔너리에 저장. 세이브가 있을 경우, 가져와서 최신화
        public DomAthRepository(Test_JYL_SaveManager saveManager)
        {
            saveM = saveManager;
            // CSV 로드
            // var csvData = ;
            // foreach(var row in csvData)
            // {
            //     var athlete = DomAthFactory.CreateFromCsv(row);
            //     athleteDict.Add(athlete.name,athlete);
            // }
            
            for (int i = 0; i < 25; i++) //csv의 행 수 만큼 반복
            {
                DomAthEntity entity = DomAthFactory.CreateFromCsv(i); // CSV에서 읽어와서 초기화

                if (!athleteDict.TryAdd(entity.name, entity))
                {
                    Debug.LogWarning($"이미 추가된 선수임{entity.name}");
                }

                saveM.UpdateAthleteEntity(entity); // 선수 세이브 객체를 통해 최신화
            }
            
        }

        public DomAthEntity FindByName(string name) // 이름으로 선수 찾기 (id로 찾는게 더 나을 수도)
        {
            return  athleteDict.GetValueOrDefault(name);
        }
        
        public List<DomAthEntity> FindAll() // 전체 선수들 리스트로 내보내기
        {
            return  athleteDict.Values.ToList();
        }

        public List<DomAthEntity> FindAllRecruited() // 현재 영입된 선수들 리스트로 내보내기. 은퇴 선수도 포함이라 알아서 걸러써야 함
        {
            return  athleteDict.Values.Where(x => x.curState != AthleteState.Unrecruited).ToList();
        }

        public void Save(DomAthEntity entity) // 선수 영입
        {
            // TODO: 작성완료해야함
        }

        public void Delete(DomAthEntity entity) // 선수 방출
        {
            // TODO: 작성완료해야함
        }
    }
}