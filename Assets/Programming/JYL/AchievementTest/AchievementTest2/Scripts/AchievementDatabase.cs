using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL.AchievementTest02
{
    [CreateAssetMenu(fileName = "AchievementDB", menuName = "Achievement/Database")]
    [Serializable]
    public class AchievementDatabase : ScriptableObject
    {
        public List<Achievement> achievements = new();
    }
}

