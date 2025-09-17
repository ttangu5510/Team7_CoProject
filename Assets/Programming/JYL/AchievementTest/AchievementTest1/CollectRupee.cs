using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JYL;

namespace JYL.AchievementTest01
{
    public class CollectRupee : MonoBehaviour
    {
        public AudioSource collectSound;

        private void OnTriggerEnter(Collider other)
        {
            GlobalAchievement.Ach01Count += 1;
            collectSound.Play();
            Destroy(gameObject);
        }
    }
}

