using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JWS;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace JYL
{
    public class IngameSaveItem : MonoBehaviour
    {
        [Header("Set Text")]
        [SerializeField] private TextMeshProUGUI progressedYearText;
        [SerializeField] private TextMeshProUGUI savedTimeText;

        [Header("Set PopUp")]
        [SerializeField] private ConfirmPUI pui;
        
        [Inject] private ISaveManager saveManager;
        
        // Awake에서 세팅
        private Button saveButton;
        private RectTransform parent;
        
        // 팝업창에 넣을 문구
        private string[] overrideText;
        private string[] newWriteText;
        
        // 외부에서 주입받는 데이터
        private SaveData save;
        private int slotNumber;

        private void Awake()
        {
            Subscribe();
        }

        private void Subscribe()
        {
            saveButton = GetComponent<Button>();
            
            saveButton.OnClickAsObservable()
                .Subscribe(_ => OnClickSaveButton())
                .AddTo(this);
            
            overrideText = new string [3] { "데이터를 덮어쓰시겠습니까?", "기존의 데이터는 삭제됩니다.", "저장되었습니다." };
            newWriteText = new string [2] { "저장되었습니다.", "" };
        }
        
        // 세이브데이터가 슬롯에 있을 경우
        public void Init(SaveData save, int slotNumber,RectTransform rectTransform)
        {
            this.slotNumber = slotNumber;
            parent = rectTransform;
            this.save = save;

            UpdateUI();
        }
        
        // 세이브 데이터가 슬롯에 없을 경우
        public void Init(int slotNumber,RectTransform rectTransform)
        {
            this.slotNumber = slotNumber;
            parent = rectTransform;
            save = null;

            UpdateUI();
        }

        private void UpdateUI()
        {
            if (save != null)
            {
                // 인게임 시간 플레이 타임
                string year = (save.time.week / 40).ToString();
                string season = save.time.season.ToString();
                string week = (save.time.week % 40).ToString();

                progressedYearText.text = $"{year}년차 {season} {week}주차";
            
                // 마지막 저장시간
                savedTimeText.text = Util.UtcToKst(save.time.lastSaveUtcIso);
            }
            else
            {
                progressedYearText.text = "빈 데이터";
                savedTimeText.text = "";
            }
        }


        // 각 세이브 아이템을 클릭했을 때, 수행되는 함수. 덮어쓰기를 전제로 로직이 수행된다.
        private void OnClickSaveButton()
        {
            // 확인 팝업 생성
            ConfirmPUI popUp = Instantiate(pui,parent);
            // 현재 플레이중인 세이브 파일 가져옴
            SaveData currentSaveData = saveManager.GetCurrentSave();
            
            if (currentSaveData != null) // 세이브 파일이 있을 경우, 덮어씌워야 함.
            {
                popUp.Init(overrideText, ConfirmState.YesOrNoAndConfirm); // 팝업 내용 채우기
                popUp.ConfirmSubject.Subscribe(confirm => // 팝업의 클릭 결과 이벤트 구독
                {
                    if (confirm)
                    {
                        saveManager.SaveProgress(currentSaveData, slotNumber);
                        Init(currentSaveData, slotNumber, parent); // UI 정보 최신화
                    }
                }).AddTo(this);
            }
            
            else // 세이브 파일이 없을 경우, 저장하고 확인만 함.
            {
                saveManager.SaveProgress(currentSaveData, slotNumber); // 저장
                Init(currentSaveData, slotNumber, parent); // UI 정보 최신화
                popUp.Init(newWriteText,ConfirmState.OnlyConfirm);
            }
        }
    }
}

