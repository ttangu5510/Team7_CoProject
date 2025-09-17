using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JYL.AchievementTest02
{
    [RequireComponent(typeof(Animator))]
    public class AchievementNotificationController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI achievementTitleLabel;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void ShowNotification(Achievement achievement)
        {
            achievementTitleLabel.text = achievement.title;
            animator.SetTrigger("Appear");
        }
    } 
}

