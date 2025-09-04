using System.Collections;
using System.Collections.Generic;
using JWS;
using StatefulUI.Runtime.Core;
using UniRx;
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
        [Inject] private readonly DiContainer container;

        private RectTransform rectTransform;
        private List<SaveData> allSave = new();
        private Dictionary<int, IngameSaveItem> items = new();
        private Dictionary<int, SaveData> slotNumberDict = new();

        private void Awake()
        {
            // 돌아가기 버튼 이벤트 구독
            backButton.OnClickAsObservable()
                .Subscribe(_=> gameObject.SetActive(false))
                .AddTo(this);
            
            // 생성되는 팝업창의 부모 설정
            rectTransform = GetComponent<RectTransform>();
        }
        
        void OnEnable()
        {
            // 초기화 작업
            Init();

            // 수동 세이브 아이템 10개 생성
            SetSaveItem();
        }
        
        private void Init()
        {
            // 리스트/딕셔너리 클리어
            items.Clear();
            slotNumberDict.Clear();
            allSave.Clear();
            
            // 현재 세이브 리스트 가져오기
            allSave = saveManager.GetAllSave();
            // 슬롯 인덱스 별로 딕셔너리화
            foreach (var save in allSave)
            {
                slotNumberDict[save.saveSlotIndex] = save;
                Debug.Log($"현재 세이브 슬롯 인덱스{save.saveSlotIndex}");
            }
        }

        // 수동 세이브 아이템들 생성
        private void SetSaveItem()
        {
            // 슬롯 인덱스마다 세이브 데이터가 있을 경우와 없을 경우에 맞게 생성
            for (int i = 1; i <= 10; i++)
            {
                IngameSaveItem item = container.InstantiatePrefabForComponent<IngameSaveItem>(itemPrefab, itemParent);
                items[i] = item;

                if (slotNumberDict.TryGetValue(i, out SaveData save))
                {
                    item.Init(save,i,rectTransform); 
                }
                else
                {
                    item.Init(i, rectTransform); // Init에서 이벤트 구독 처리함
                }
                
            }
        }
    }
}

