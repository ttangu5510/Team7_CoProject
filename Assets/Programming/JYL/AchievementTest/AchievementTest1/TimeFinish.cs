using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JYL.AchievementTest01
{
    public class TimeFinish : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private float achieveTime = 7f;

        private float timer;
        void Start()
        {
            StartCoroutine(TimeAchieve());
            timerText.gameObject.SetActive(true);
        }

        void OnTriggerEnter(Collider other)
        {
            GlobalAchievement.ach03Trigger = true;
            Destroy(timerText.gameObject);
            Destroy(gameObject);
        }
    
        IEnumerator TimeAchieve()
        {
            timer = achieveTime;
            while (timer > 0)
            {
                timerText.text = timer.ToString("F");
                timer -= Time.deltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
        }
    }
}

