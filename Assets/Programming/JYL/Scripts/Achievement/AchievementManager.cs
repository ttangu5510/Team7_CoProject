using System.Collections;
using System.Collections.Generic;
using JWS;
using UnityEngine;
using Zenject;

namespace JYL
{
    public class AchievementManager : MonoBehaviour
    {
        [Inject] private ISaveManager saveManager;
        [SerializeField] private AchievementDatabase database;
        private List<AchievementController> achievements = new();
        void Start()
        {
            achievements = GetControllerList();
        }

        void Update()
        {
        
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
            return result;
        }
    }
}

