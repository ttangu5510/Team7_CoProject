using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;
using JYL;
using UnityEngine.SceneManagement;
using Zenject;

public class NameEntryController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject nameInputPopup;   // 이름 입력 팝업
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
        if (errorOkButton) errorOkButton.onClick.AddListener(OnErrorOk);
        if (confirmButton) confirmButton.onClick.AddListener(OnConfirm);
        if (errorPopup) errorPopup.SetActive(false);
    }

    void OnConfirm()
    {
        // 주인공 이름 검사
        if (!NameRules.TryValidate(nameField.text, out var reason1))
        {
            ShowError($"<color=#ff3b30>{reason1}</color>\n이름 조건 : 2~8자, 공백/특수문자 불가");
            nameField.ActivateInputField();
            return;
        }

        // 팀 이름 검사
        if (!NameRules.TryValidate(teamNameField.text, out var reason2))
        {
            ShowError($"<color=#ff3b30>{reason2}</color>\n이름 조건 : 2~8자, 공백/특수문자 불가");
            teamNameField.ActivateInputField();
            return;
        }

        // 저장(JSON)
        string protagonistName = nameField.text.Trim();
        string teamName = teamNameField.text.Trim();
        if (saveManager == null) Debug.Log("세이브매니저 널");
        saveManager.CreateSaveData(protagonistName,teamName);

        nameInputPopup.SetActive(false);
        SceneManager.LoadSceneAsync("JYL_MainScene");
    }

    void ShowError(string message)
    {
        if (errorText) errorText.text = message;
        if (errorPopup) errorPopup.SetActive(true);   // 에러 팝업 표시
    }

    void OnErrorOk()
    {
        if (errorPopup) errorPopup.SetActive(false);  // 팝업 닫고
        if (nameInputPopup) nameInputPopup.SetActive(true);   // 입력 팝업으로 복귀(이미 켜져 있으면 그대로)
    }

    // TODO : 세이브 매니저에서 데이터 불러오기, 세이브 패널 열기
    
}
