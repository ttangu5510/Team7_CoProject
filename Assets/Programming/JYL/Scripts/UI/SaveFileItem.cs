using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using JWS;
using Newtonsoft.Json.Converters;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace JYL
{
    public class SaveFileItem : MonoBehaviour
    {
        [Header("Set Buttons")]
        [SerializeField] public Button loadButton;
        [SerializeField] public Button deleteButton;
    
        [Header("Set Texts")]
        [SerializeField] private TextMeshProUGUI progressedYearText;
        [SerializeField] private TextMeshProUGUI savedTimeText;

        [Header("Set References")] 
        [SerializeField] private DeleteSavePUI deletePopUp;
        
        [Inject] private ISaveManager saveManager;
        [Inject] private IUiManager uiManager;
    
        private int saveSlotIndex = -1;

        private RectTransform parent; 
        private IDisposable subscription;

        
        
        public void Init(SaveData save, int saveSlotIndex, RectTransform parent) // 세이브 있는 슬롯
        {
            this.parent = parent;
            this.saveSlotIndex = saveSlotIndex;
            string year = (save.time.week / 40).ToString();
            string season = save.time.season.ToString();
            string week = (save.time.week % 40).ToString();

            // 현재 플레이 중인 인게임 시간
            progressedYearText.text = $"{year}년차 {season} {week}주차";

            // 마지막 저장 시간
            savedTimeText.text = Util.UtcToKst(save.time.lastSaveUtcIso);
        
            // 버튼 이벤트 연결
            loadButton.OnClickAsObservable()
                .Subscribe(_ => OnClickLoadButton(save))
                .AddTo(this);
            deleteButton.OnClickAsObservable()
                .Subscribe(_ => OnClickDeleteButton(save))
                .AddTo(this);
            
            // 처음에는 불러오는 상태이니 deleteButton 꺼둠
            deleteButton.gameObject.SetActive(false);
        }

        public void Init(int saveSlotIndex, RectTransform parent) // 빈 슬롯
        {
            this.parent = parent;
            this.saveSlotIndex = saveSlotIndex;
            progressedYearText.text = "빈 데이터";
            savedTimeText.text = "";
            loadButton.interactable = false;
            deleteButton.interactable = false;
            deleteButton.gameObject.SetActive(false);
        }

        public void SetLoadButton()
        {
            loadButton.gameObject.SetActive(true);
            deleteButton.gameObject.SetActive(false);
        }

        public void SetDeleteButton()
        {
            loadButton.gameObject.SetActive(false);
            deleteButton.gameObject.SetActive(true);
        }

        private void OnClickLoadButton(SaveData save)
        {
            if (saveManager == null)
            {
                Debug.Log("이거 널임");
            }
            saveManager.LoadProgress(save);
            SceneManager.LoadSceneAsync("JYL_MainScene");
        }

        private void OnClickDeleteButton(SaveData save)
        {
            // 팝업창에서 생성되는 이벤트 발행을 구독
            DeleteSavePUI del = Instantiate(deletePopUp, parent);
            del.ConfirmSubject
                .Subscribe(confirmed =>
                {
                    if (confirmed)
                    { 
                        OnConfirmDelete(save);
                    }
                })
                .AddTo(del);
        }
        
        // 팝업창에서 발행된 이벤트에 의해 수행됨
        private void OnConfirmDelete(SaveData save)
        {
            saveManager.DeleteSaveFile(save, saveSlotIndex);
            Init(saveSlotIndex, parent); // 빈 데이터로 처리
            SetDeleteButton(); // 삭제 버튼 활성화로 변경
        }
    }
}

