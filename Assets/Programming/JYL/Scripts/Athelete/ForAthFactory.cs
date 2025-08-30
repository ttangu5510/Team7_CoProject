using System.Collections;
using System.Collections.Generic;
using JWS;
using UnityEngine;

namespace JYL
{
    public class ForAthFactory : MonoBehaviour
    {
        public static ForAthEntity CreateEntityFromCSV(ForAthleteCsvData data)
        {
            var entity = new ForAthEntity();
            
            var stats = new AthleteStats(
                data.Health, data.Quickness, data.Flexibility, 
                data.Technic, data.Speed, data.Balance);
            
            entity.Init(data.ID,data.Name,data.Affiliation, stats);
            
            return entity;
        }
    }
}

