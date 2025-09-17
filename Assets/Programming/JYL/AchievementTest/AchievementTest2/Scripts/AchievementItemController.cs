using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JYL.AchievementTest02
{
    public class AchievementItemController : MonoBehaviour
    {
        [SerializeField] private Image unlockedIcon;
        [SerializeField] private Image lockedIcon;
    
        [SerializeField] private TextMeshProUGUI titleLabelText;
        [SerializeField] private TextMeshProUGUI descriptionLabelText;

        public bool unlocked;
        public Achievement achievement;

        public void RefreshView()
        {
            titleLabelText.text = achievement.title;
            descriptionLabelText.text = achievement.description;
            unlockedIcon.enabled = unlocked;
            lockedIcon.enabled = !unlocked;
        }

        private void OnValidate() // 인스펙터 창에서 값이 변할 때마다 최신화
        {
            RefreshView();
        }
    }
}

