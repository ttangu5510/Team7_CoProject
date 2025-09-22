using System.Collections;
using System.Collections.Generic;
using JWS;
using UnityEngine;
using Zenject;

namespace JYL
{
    public class AchievementManager : MonoBehaviour
    {
        [SerializeField] private AchievementDatabase database;
        
        [Inject] private ISaveManager saveManager;
        
        private List<AchievementController> achievements = new();
        void OnEnable()
        {
            achievements = GetControllerList();
        }

        private List<AchievementController> GetControllerList()
        {
            List<AchievementController> result = new();
            SaveData save = saveManager.GetCurrentSave(); 
            foreach (var data in database.achievements)
            {
                AchievementSave achSave = save.FindAchievementSaveByID(data.ID);
                result.Add(new AchievementController(data, achSave));
            }

            foreach (var controller in achievements)
            {
                //controller.state
            }
            return result;
        }
    }
}

