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

        public AchievementItemController achievementItemPrefab;
        public RectTransform content;

        [SerializeField][HideInInspector] private List<AchievementItemController> itemList = new();
        
        public Achievements achievementToShow;
        
        private void Start()
        {
            dropdownController.onValueChanged += HandleAchievementDropdownValueChanged;
            LoadAchievementsList();
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

        [ContextMenu("Load Achievements")]
        private void LoadAchievementsList()
        {
            foreach (AchievementItemController item in itemList)
            {
                if (item)
                {
                    if (item.gameObject != null) DestroyImmediate(item.gameObject);
                }
            }
            itemList.Clear();
            
            foreach (Achievement achievement in database.achievements)
            {
                AchievementItemController achievementItem = Instantiate(achievementItemPrefab, content);
                itemList.Add(achievementItem);
                
                // 언락 정보 최신화
                bool unlocked = PlayerPrefs.GetInt(achievement.id, 0) == 1;
                achievementItem.unlocked = unlocked;
                
                achievementItem.achievement = achievement;
                achievementItem.RefreshView();
            }
        }
        
        public void UnlockAchievement() // 팝업되는 UI 재생
        {
            UnlockAchievement(achievementToShow);
        }
        
        public void UnlockAchievement(Achievements achievement)
        {
            AchievementItemController item = itemList[(int)achievement];
    
            if(item.unlocked) return; // 이미 언락됐다면 return
    
            ShowNotification();
            item.unlocked = true;
            item.RefreshView(); // UI 최신화
            // TODO : 여기서 세이브데이터 객체에 최신 상황 저장해야함.
            PlayerPrefs.SetInt(item.achievement.id, 1);
        }
        
        public void LockAllAchievement()
        {
            foreach (Achievement achievement in database.achievements)
            {
                PlayerPrefs.DeleteKey(achievement.id);
            }
            foreach (AchievementItemController item in itemList)
            {
                item.unlocked = false;
                item.RefreshView();
            }
        }
    }
}
