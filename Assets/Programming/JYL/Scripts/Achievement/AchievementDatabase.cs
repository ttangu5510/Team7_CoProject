using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    [CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Achievement/Database")]
    [Serializable]
    public class AchievementDatabase : ScriptableObject
    {
        public List<Achievement> achievements = new();

        [ContextMenu("Set Achievements")]
        public void GetAchievements()
        {
            achievements.Clear();
            var list = CsvReader.ReadAchievements("AchievementDataTable");
            foreach (var item in list)
            {
                achievements.Add(new Achievement(item));
            }
        }
    }
}

