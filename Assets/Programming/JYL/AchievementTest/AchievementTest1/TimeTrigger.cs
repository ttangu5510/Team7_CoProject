using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL.AchievementTest01
{
    public class TimeTrigger : MonoBehaviour
    {
        public GameObject finalTimeTrigger;
        void OnTriggerEnter(Collider other)
        {
            finalTimeTrigger.SetActive(true);
        }
    }
}

