using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image illustration;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button actionButton;

    public void Setup(string title, Sprite illust, string message, string buttonLabel, UnityEngine.Events.UnityAction action)
    {
        titleText.text = title;
        illustration.sprite = illust;
        messageText.text = message;
        actionButton.GetComponentInChildren<TextMeshProUGUI>().text = buttonLabel;
        actionButton.onClick.RemoveAllListeners();
        actionButton.onClick.AddListener(action);
    }

    public void Show() { gameObject.SetActive(true); }
    public void Hide() { gameObject.SetActive(false); }
}

/*
  사용 예시
    [SerializeField] private LoadingPopup loadingPopup;
    [SerializeField] private Sprite errorSprite;
    [SerializeField] private Sprite failSprite;

    private void ShowErrorPopup()
    {
        loadingPopup.Setup(
            title: "로딩 오류",
            illustration: errorSprite,
            message: "로딩 중 오류가 발생했습니다.",
            buttonLabel: "재시도",
            buttonAction: OnRetryClicked
        );
        loadingPopup.Show();
    }

    private void ShowFailPopup()
    {
        loadingPopup.Setup(
            title: "로딩 실패",
            illustration: failSprite,
            message: "로딩이 실패하여 타이틀로 이동합니다.",
            buttonLabel: "타이틀로",
            buttonAction: OnMoveToTitleClicked
        );
        loadingPopup.Show();
    }

    private void OnRetryClicked()
    {
        loadingPopup.Hide();
        // 로딩 재시도 로직 호출
    }

    private void OnMoveToTitleClicked()
    {
        loadingPopup.Hide();
        // 타이틀 화면 이동 로직 호출
    }

 */