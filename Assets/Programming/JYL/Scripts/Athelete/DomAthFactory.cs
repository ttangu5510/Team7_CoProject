using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        // public static DomAthEntity CreateFromCsv(CsvData row)
        // {
        //     var entity = new DomAthEntity();
        //     entity.Init(
        //         row.id, row.name, row.affiliation, row.maxGrade,
        //         row.recruitAge, row.health, row.quickness, row.flexibility,
        //         row.technic,row.speed,row.balance
        //     );
        //     return entity;
        // }
        
    }
}

