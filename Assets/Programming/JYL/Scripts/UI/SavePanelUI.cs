using System.Collections;
using System.Collections.Generic;
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

        private List<SaveData> allSave = new(11);
        private SaveData autoSave;
        private Dictionary<int, SaveFileItem> items = new();

        private SaveMode mode;

        private RectTransform rectTransform;
        
        private void Awake()
        {
            // 생성되는 세이브 파일 아이템의 부모 설정
            rectTransform = GetComponent<RectTransform>();
            
            // 현재 세이브 모드 세팅
            mode = SaveMode.Load;
            
            // 세이브 매니저에서 리스트 받아옴
            allSave = saveManager.GetAllSave();
            autoSave = saveManager.GetAutoSaveData();
            
            // 오토 세이브 있을 경우 세팅
            if (autoSave != null)
            {
                allSave.Remove(autoSave);
                SaveFileItem tmp = Instantiate(item, autoSaveContent);
                items[0] = tmp;
                tmp.Init(autoSave,0, rectTransform);
                tmp.loadButton.interactable = true;
                tmp.deleteButton.interactable = true;
            }
            // 없을 경우 세팅
            else
            {
                SaveFileItem tmp = Instantiate(item, autoSaveContent);
                items[0] = tmp;
                tmp.Init(0, rectTransform);
            }
            
            // 슬롯 인덱스 순으로 정렬
            allSave.Sort((s1,s2) => s1.saveSlotIndex.CompareTo(s2.saveSlotIndex));

            for (int i = 1; i <= 10; i++)
            {
                SaveFileItem tmp = Instantiate(item,uiContent);
                items[i] = tmp;
                
                if (!allSave.HasIndex(i)) // 세이브파일이 비어있을 경우
                {
                    tmp.Init(i,rectTransform);
                    continue;
                }
                
                tmp.Init(allSave[i],i, rectTransform);
            }
            
            
            // 토글 이벤트 연결
            changeTogle.OnClickAsObservable()
                .Subscribe(_ => OnClickTogle())
                .AddTo(this);
            
            // X 버튼 이벤트 연결
            returnButton.OnClickAsObservable()
                .Subscribe(_=>OnClickExit())
                .AddTo(this);
        }

        
        private void OnClickExit() // X 버튼 누를 시
        {
            titleScreen.SetActive(true);
            gameObject.SetActive(false);
        }

        private void OnClickTogle() // 불러오기/삭제하기 전환 버튼 누를 시
        {
            deleteImage.gameObject.SetActive(!deleteImage.gameObject.activeSelf);
            if (mode == SaveMode.Load)
            {
                mode = SaveMode.Delete;
                foreach (var item in items.Values)
                {
                    item.SetDeleteButton();
                }
            }
            else if (mode == SaveMode.Delete)
            {
                mode =  SaveMode.Load;
                foreach (var item in items.Values)
                {
                    item.SetLoadButton();
                }
            }
        }
    }
}

