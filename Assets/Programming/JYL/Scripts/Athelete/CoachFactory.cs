using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class CoachFactory // 코치 동적 객체 생성에 사용. 초기 데이터를 기준으로 생성함. Repository에서 호출
    {
        public static CoachEntity CreateCoachEntity(int i) // TODO : 코치 테스트 생성 함수
        {
            CoachEntity entity = new(10222, $"coach_{i}", CoachGrade.Veteran, 32);
            return entity;
        }
        // TODO : CSVReader완성되면 사용
        // public static CoachEntity CreateCoachEntity(CsvData row)
        // {
        //     CoachEntity entity = new(...);
        //      return entity;
        // }
    }    
}

