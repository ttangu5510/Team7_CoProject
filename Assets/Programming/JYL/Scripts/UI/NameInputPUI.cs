using System.Collections;
using System.Collections.Generic;
using JYL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace JYL
{
    public class NameInputPUI : MonoBehaviour
    {
        [Header("Set UI Manager")] 
        [SerializeField] private UIManager uiManager;
        
        [Header("Panels")]
        [SerializeField] private GameObject errorPopup;   // 에러 팝업 (비활성 시작 권장)
    
        [Header("Inputs")]
        [SerializeField] private TMP_InputField nameField;
        [SerializeField] private TMP_InputField teamNameField;
    
        [Header("Error UI")]
        [SerializeField] private TextMeshProUGUI errorText;
        [SerializeField] private Button errorOkButton;
    
        [Header("Actions")]
        [SerializeField] private Button confirmButton;
    
        [Inject] public ISaveManager saveManager;
    
        void Awake()
        {
            errorOkButton.OnClickAsObservable()
                .Subscribe(_ => OnErrorOk())
                .AddTo(this);
            
            confirmButton.OnClickAsObservable()
                .Subscribe(_ => OnConfirm())
                .AddTo(this);
            
            errorPopup.SetActive(false);
        }
    
        void OnConfirm()
        {
            // 주인공 이름 검사. 안되면 넘김
            if (!NameRules.TryValidate(nameField.text, out var reason1))
            {
                ShowError($"<color=#ff3b30>{reason1}</color>\n이름 조건 : 2~8자, 공백/특수문자 불가");
                nameField.ActivateInputField();
                return;
            }
    
            // 팀 이름 검사. 안되면 넘김
            if (!NameRules.TryValidate(teamNameField.text, out var reason2))
            {
                ShowError($"<color=#ff3b30>{reason2}</color>\n이름 조건 : 2~8자, 공백/특수문자 불가");
                teamNameField.ActivateInputField();
                return;
            }
    
            // 저장(JSON)
            string protagonistName = nameField.text.Trim();
            string teamName = teamNameField.text.Trim();
            
            // TODO : UID 들어오면 여기서 추가
            saveManager.CreateAutoSaveData(protagonistName,teamName,"testUid123"); 
            
            uiManager.OpenPanel("loading");
        }
    
        void ShowError(string message)
        {
            if (errorText) errorText.text = message;
            if (errorPopup) errorPopup.SetActive(true);   // 에러 팝업 표시
        }
    
        void OnErrorOk()
        {
            if (errorPopup) errorPopup.SetActive(false);  // 팝업 닫기
        }
    }
}
