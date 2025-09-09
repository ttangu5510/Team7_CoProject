using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using JWS;
using StatefulUI.Runtime.Core;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace JYL
{
    public enum SaveMode { Load, Delete }
    public class SavePanelUI : MonoBehaviour
    {
        [Header("Set References")] 
        [SerializeField] private GameObject titleScreen;
        [SerializeField] private SaveFileItem item;
        
        [Header("Set Parent")]
        [SerializeField] private RectTransform uiContent;
        [SerializeField] private RectTransform autoSaveContent;
        
        [Header("Set UI")]
        [SerializeField] private Button returnButton;
        [SerializeField] private Button changeTogle;
        [SerializeField] private Image deleteImage;

        [Inject] private IUiManager uiManager;
        [Inject] private ISaveManager saveManager;
        [Inject] readonly DiContainer container;

        private List<SaveData> allSave = new(11);
        private SaveData autoSave;
        private Dictionary<int, SaveFileItem> items = new();
        private Dictionary<int, SaveData> slotNumberDict = new();
        
        private RectTransform rectTransform;
        private SaveMode mode;

        private void Awake()
        {
            Subscribe();
        }

        private void OnEnable()
        {
            Init();
            // if(mode ==  SaveMode.Delete) OnClickTogle(); // Delete모드로 닫았었으면 Load 모드로 다시 돌림
        }

        private void Subscribe()
        {
            // 토글 이벤트 연결
            changeTogle.OnClickAsObservable()
                .Subscribe(_ => OnClickTogle())
                .AddTo(this);
        }

        private void Init()
        {
            // 생성된 아이템들 파괴
            foreach (var item in items.Values)
            {
                Destroy(item.gameObject);
            }
            
            // 리스트/딕셔너리 초기화
            items.Clear();
            slotNumberDict.Clear();

            
            // 생성되는 세이브 파일 아이템의 부모 설정
            rectTransform = GetComponent<RectTransform>();
            
            // 현재 세이브 모드 세팅
            mode = SaveMode.Load;
            deleteImage.gameObject.SetActive(true);
            
            // 세이브 매니저에서 리스트 받아옴
            allSave = saveManager.GetAllSave();
            autoSave = saveManager.GetAutoSaveData();
            
            // 오토 세이브 아이템 생성
            SetAutoSaveItem();
            
            // 수동 세이브들 딕셔너리화
            foreach (var save in allSave)
            {
                slotNumberDict[save.saveSlotIndex] = save;
            }
            
            // 수동 세이브 아이템들로 생성
            SetSaveItems();
            
            // X 버튼 이벤트 연결
            // returnButton.OnClickAsObservable()
            //     .Subscribe(_=>OnClickExit())
            //     .AddTo(this);
        }
        
        // private void OnClickExit() // X 버튼 누를 시
        // {
        //     titleScreen.SetActive(true);
        //     gameObject.SetActive(false);
        // }
        
        private void OnClickTogle() // 불러오기/삭제하기 전환 버튼 누를 시
        {
            deleteImage.gameObject.SetActive(!deleteImage.gameObject.activeSelf);
            if (mode == SaveMode.Load)
            {
                mode = SaveMode.Delete;
                foreach (var item in items.Values)
                {
                    if(item) item.SetDeleteButton();
                }
            }
            else if (mode == SaveMode.Delete)
            {
                mode =  SaveMode.Load;
                foreach (var item in items.Values)
                {
                    if(item) item.SetLoadButton();
                }
            }
        }

        // 오토 세이브 아이템 생성
        private void SetAutoSaveItem()
        {
            SaveFileItem autoTmp = container.InstantiatePrefabForComponent<SaveFileItem>(item, autoSaveContent);
            // 오토 세이브 있을 경우 세팅
            if (autoSave != null)
            {
                allSave.Remove(autoSave);
                items[0] = autoTmp;
                autoTmp.Init(autoSave,0, rectTransform);
                autoTmp.loadButton.interactable = true;
                autoTmp.deleteButton.interactable = true;
            }
            // 없을 경우 세팅
            else
            {
                items[0] = autoTmp;
                autoTmp.Init(0, rectTransform);
            }
        }

        // 수동 세이브 아이템들 생성
        private void SetSaveItems()
        {
            // 수동 세이브 파일 10개 생성
            for (int i = 1; i <= 10; i++)
            {
                SaveFileItem tmp = container.InstantiatePrefabForComponent<SaveFileItem>(item,uiContent);
                items[i] = tmp;
                
                if (slotNumberDict.TryGetValue(i,out SaveData save)) // 세이브파일이 비어있을 경우
                {
                    tmp.Init(save, i, rectTransform);
                }
                else
                {
                    tmp.Init(i,rectTransform);
                }
            }
        }
    }
}

