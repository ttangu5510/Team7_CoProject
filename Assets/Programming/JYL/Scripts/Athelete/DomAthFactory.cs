using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JWS;

namespace JYL
{
    public static class DomAthFactory
    {
        // public static DomAthEntity CreateFromCsv(int i) // TODO :테스트로 사용
        // {
        //     DomAthEntity entity = new();
        //     entity.Init(1001, $"athlete_{i}", AthleteAffiliation.일반선수, AthleteGrade.A,
        //         19, 5, 4, 6, 9, 10, 1);
        //     return entity;
        // }
        public static DomAthEntity CreateAthEntityFromCSV(DomAthleteCsvData data)
        {
            var entity = new DomAthEntity();
            entity.Init(
                data.ID, data.Name, data.Affiliation, data.Grade,
                data.RecruitAge, data.Health, data.Quickness, data.Flexibility,
                data.Technic,data.Speed,data.Balance
            );
            return entity;
        }
    }
}

