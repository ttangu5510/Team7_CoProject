using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using JWS;

namespace JYL
{
    public static class DomAthFactory
    {
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

