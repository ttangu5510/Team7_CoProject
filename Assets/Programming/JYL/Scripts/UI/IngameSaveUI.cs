using System.Collections;
using System.Collections.Generic;
using JWS;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace JYL
{
    public class IngameSaveUI : MonoBehaviour
    {
        [Header("Set Parent")]
        [SerializeField] private RectTransform itemParent;

        [Header("Set Prefab")]
        [SerializeField] private IngameSaveItem itemPrefab;

        [Header("Set Button")] 
        [SerializeField] private Button backButton;
        
        [Inject] private ISaveManager saveManager;

        private List<SaveData> allSave = new();
        private Dictionary<int, IngameSaveItem> items = new();

        private void Awake()
        {
            allSave = saveManager.GetAllSave();
            allSave.Sort((s1,s2)=>s1.saveSlotIndex.CompareTo(s2.saveSlotIndex));
        }
        void OnEnable()
        {
            
        }

        void Update()
        {
        
        }
    }
}

