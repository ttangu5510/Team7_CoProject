using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL.AchievementTest02
{
    public class AchievementManager : MonoBehaviour
    {
        public AchievementDatabase database;
        public AchievementNotificationController controller;

        public AchievementDropdownController dropdownController;
        
        public Achievements achievementToShow;

        private void Start()
        {
            dropdownController.onValueChanged += HandleAchievementDropdownValueChanged;
        }
        public void ShowNotification()
        {
            Achievement achievement = database.achievements[(int)achievementToShow];
            controller.ShowNotification(achievement);
        }

        private void HandleAchievementDropdownValueChanged(Achievements achievement)
        {
            achievementToShow = achievement;
        }
    }
}
