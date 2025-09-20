using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    [CreateAssetMenu(fileName = "AchievementDatabase", menuName = "Achievement")]
    public class AchievementDatabase : ScriptableObject
    {
        public List<Achievement> achievements = new();

        [ContextMenu("Set Achievements")]
        public void GetAchievements()
        {
            var list = CsvReader.ReadAchievements("CSV/AchievementDataTable");
            foreach (var item in list)
            {
                achievements.Add(new Achievement(item));
            }
        }
    }
}

