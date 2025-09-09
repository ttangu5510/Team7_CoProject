using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JWS;

namespace JYL
{
    public class CoachFactory // 코치 동적 객체 생성에 사용. 초기 데이터를 기준으로 생성함. Repository에서 호출
    {
        // public static CoachEntity CreateCoachEntity(int i) // TODO : 코치 테스트 생성 함수
        // {
        //     CoachEntity entity = new(10222, $"coach_{i}", CoachGrade.선수출신, 32);
        //     return entity;
        // }
        public static CoachEntity CreateCoachEntityFromCSV(CoachCsvData row)
        {
            CoachEntity entity = new CoachEntity(
                row.ID, row.Name, row.Grade, row.Age
            );
            return entity;
        }

    }    
}

