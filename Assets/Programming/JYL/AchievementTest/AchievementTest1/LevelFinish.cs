using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL.AchievementTest01
{
    public class LevelFinish : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            GlobalAchievement.ach02Trigger = true;
            Destroy(gameObject);
        }
    }
}

