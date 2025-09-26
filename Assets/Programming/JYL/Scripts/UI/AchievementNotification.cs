using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JYL
{
    [RequireComponent(typeof(Animator))]
    public class AchievementNotification : MonoBehaviour
    {
        [Header("Set Achievement UI")]
        [SerializeField] private TextMeshProUGUI achievementTitleLabel;
        [SerializeField] private Image trophyImage;
        [SerializeField] private Sprite bronze;
        [SerializeField] private Sprite silver;
        [SerializeField] private Sprite gold;
        [SerializeField] private Sprite platinum;
        
        [Header("Set Trophies")]
        
        private static readonly int Appear = Animator.StringToHash("Appear");
        
        private Animator animator;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public void ShowNotification(Achievement achievement)
        {
            achievementTitleLabel.text = achievement.AchName;
            switch (achievement.Reward)
            {
                case AchievementReward.브론즈:
                    trophyImage.sprite = bronze;
                    break;
                case AchievementReward.실버:
                    trophyImage.sprite = silver;
                    break;
                case AchievementReward.골드:
                    trophyImage.sprite = gold;
                    break;
                case AchievementReward.플래티넘:
                    trophyImage.sprite = platinum;
                    break;
            }
            animator.SetTrigger(Appear);
        }
    }
}

