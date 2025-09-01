using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    [Serializable]
    public class ForAthEntity : BaseAthEntity
    {
        public AthleteStats stats { get; private set; }
        public AthleteAffiliation affiliation { get; private set; }
        public AthleteNation nation { get; private set; }

        
        public void Init(int id, string athName, AthleteAffiliation affiliation, AthleteStats athleteStats)
        {
            this.id = id;
            entityName = athName;
            this.affiliation = affiliation;
            
            stats = athleteStats;
            nation = (AthleteNation)(id / 100);
        }
    }
}

