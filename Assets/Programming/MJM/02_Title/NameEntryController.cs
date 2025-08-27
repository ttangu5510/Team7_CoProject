using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

[System.Serializable]
public class PlayerProfile
{
    public string protagonistName;
    public string teamName;
}

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

    string SavePath => Path.Combine(Application.persistentDataPath, "player_profile.json");

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
        var profile = new PlayerProfile
        {
            protagonistName = nameField.text.Trim(),
            teamName = teamNameField.text.Trim()
        };

        var json = JsonUtility.ToJson(profile, true);
        File.WriteAllText(SavePath, json);

        // 필요하면 PlayerPrefs 백업도 가능:
        // PlayerPrefs.SetString("player_profile_json", json);

        // 다음 로직으로 진행 (입력 팝업 닫기 등)
        nameInputPopup.SetActive(false);
        Debug.Log($"Saved: {SavePath}\n{json}");
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

    // 필요 시 다른 씬에서 불러쓰기
    public PlayerProfile LoadProfile()
    {
        if (!File.Exists(SavePath)) return null;
        var json = File.ReadAllText(SavePath);
        return JsonUtility.FromJson<PlayerProfile>(json);
    }
}
