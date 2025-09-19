using System.Collections;
using System.Collections.Generic;
using JWS;
using UnityEngine;

namespace JYL
{
    public class Achievement
    {
        public readonly string ID;
        public readonly string AchName;
        public readonly string AchDescription;
        public readonly int CompleteNumber;
        public readonly string PrevAchievement;
        public AchievementCategory Category;
        public AchievementCondition Condition;
        public AchievementReward Reward;
        public AchievementState State;

        public Achievement(AchievementCsvData data)
        {
            ID = data.ID;
            AchName = data.AchName;
            AchDescription = data.AchDesc;
            CompleteNumber = data.CompleteNumber;
            PrevAchievement = data.PrevAchievement;
            Category = data.Category;
            Condition = data.Condition;
            Reward = data.Reward;
            State = data.state;
        }
        
    }    
}

