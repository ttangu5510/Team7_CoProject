using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JYL
{
    public class DomAthRP : MonoBehaviour // 
    {
        public List<DomAthEntity> athleteList = new();
        public List<DomAthEntity> recruitList = new();

        public void Init() // 국내 선수 전체 숫자만큼(CSV의 캐릭터 숫자만큼) 만들어서 리스트에 저장
        {
            //for(int i = 0; i < csv.Length;i++)
            DomAthEntity entity = new DomAthEntity();
            entity.Init(1001, "athlete1", AthleteAffiliation.Regular, AthleteGrade.A,
                19, 5, 4, 6, 9, 10, 1);

            // TODO : 세이브 데이터가 있을 경우, 값을 입력
            athleteList.Add(entity);
        }

        public void UpdateRecruitList() // 전체 선수 리스트에서, 보유중인 것만 리스트 화
        {
            recruitList = athleteList.Where(s => s.isRecruited).ToList();
        }
        
        
        
    }
}

