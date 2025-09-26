using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using JWS;
using Zenject;
using UniRx;

namespace JYL
{
    public class AchievementPanel : MonoBehaviour
    {
        [Header("Set References")]
        [SerializeField] private AchievementItem itemPrefab;
        [SerializeField] private RectTransform parentContent;
        [SerializeField] private AchievementRewardPUI rewardUI;
        [SerializeField] private AchievementManager manager;

        [Header("Set UI")] 
        [SerializeField] private TextMeshProUGUI achievementProgress;


        private RectTransform parentCanvas;
        
        public List<AchievementItem> items = new();
        [SerializeField] public List<AchievementController> achievements = new();

        private void Start()
        {
            parentCanvas = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        }
        
        private void OnEnable()
        {
            achievements.Clear();
            // AddRange: 리스트의 주소값들을 깊은복사로 가져온다. 원본 리스트에는 영향을 주지 않음.
            achievements.AddRange(manager.GetAchievements()); // 깊은 복사.
            achievements.Sort((c1,c2) => ((int)c1.state.Value).CompareTo((int)c2.state.Value));
            CreateItems();
            UpdateUI();
        }

        private void CreateItems()
        {
            Debug.Log("이거 들어오는 지 확인");
            if (items.Count > 0)
            {
                Debug.Log("카운트 0 이상임");
                foreach (var item in items)
                {
                    if (item != null && item.gameObject != null)
                    {
                        Destroy(item.gameObject);
                    }
                }
            }

            items.Clear();
            
            Debug.Log($"이거 카운트 확인 필요함. 업적 리스트. 갯수: {achievements.Count}");
            foreach (var a in achievements)
            {
                Debug.Log("생성 들어오는 지 확인");
                AchievementItem item = Instantiate(itemPrefab, parentContent);
                items.Add(item);
                item.Init(a, rewardUI);
                a.state
                    .Skip(1)
                    .DistinctUntilChanged()
                    .Subscribe(_=> UpdateUI())
                    .AddTo(item.gameObject);
            }
        }

        private void UpdateUI()
        {
            int completeCount = achievements.Count(achieve => achieve.state.Value == AchievementState.Completed);
            achievementProgress.text = $"{completeCount}/{achievements.Count}";    
        }
    }  
}

