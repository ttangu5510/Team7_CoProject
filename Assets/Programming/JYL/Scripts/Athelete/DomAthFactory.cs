using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JWS;

namespace JYL
{
    public static class DomAthFactory
    {
        public static DomAthEntity CreateFromCsv(int i) // TODO :테스트로 사용
        {
            DomAthEntity entity = new();
            entity.Init(1001, $"athlete_{i}", AthleteAffiliation.Regular, AthleteGrade.A,
                19, 5, 4, 6, 9, 10, 1);
            return entity;
        }
        // TODO : CsvReader 만들면 부활
        public static DomAthEntity CreateFromCsv(DomAthleteCsvData row)
        {
            var entity = new DomAthEntity();
            entity.Init(
                row.ID, row.Name, row.Affiliation, row.Grade,
                row.RecruitAge, row.Health, row.Quickness, row.Flexibility,
                row.Technic,row.Speed,row.Balance
            );
            return entity;
        }
    }
}

