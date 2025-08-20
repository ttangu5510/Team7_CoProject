using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace JYL
{
    public class DomAthRP : MonoBehaviour // 선수들의 정보를 보관하는 레포지토리 
    {
        public List<DomAthEntity> athleteList = new();
        public List<DomAthEntity> recruitList = new();

        // 테스트 세이브 매니저. 전역 접근 필요함(Zenject)
        private Test_JYL_SaveManager saveManager = new();

        public void Init()
        {
            CreateAthleteList();
            UpdateRecruitList();
        }
        
        public void CreateAthleteList() // 국내 선수 전체 숫자만큼(CSV의 캐릭터 숫자만큼) 만들어서 리스트에 저장. 세이브가 있을 경우, 가져와서 최신화
        {
            //for(int i = 0; i < csv.Length;i++)
            DomAthEntity entity = new DomAthEntity();
            AthleteSave save = saveManager.curSave.athleteSaves.Find(save => save.id == entity.id);
            entity.Init(1001, "athlete1", AthleteAffiliation.Regular, AthleteGrade.A,
                19, 5, 4, 6, 9, 10, 1);
            entity.UpdateFromSave(save);
            athleteList.Add(entity);
        }

        public void UpdateRecruitList() // 전체 선수 리스트에서, 보유중인 것만 리스트화. CreateAthleteList를 먼저 수행해야 함.
        {
            recruitList = athleteList.Where(s => s.isRecruited).ToList();
        }
    }
}