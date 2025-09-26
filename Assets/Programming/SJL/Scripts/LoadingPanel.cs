using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SJL
{
    public class LoadingPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private Image illustrationImage;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Button actionButton;
        [SerializeField] private TextMeshProUGUI actionButtonText;

        // 팝업에 상황별 정보와 액션을 세팅
        public void Setup(string title, Sprite illustration, string message, string buttonLabel, UnityAction buttonAction)
        {  
            titleText.text = title; // 타이틀
            illustrationImage.sprite = illustration;    // 일러스트
            messageText.text = message; // 본문 메시지
            actionButtonText.text = buttonLabel;    // 버튼 라벨
            // 버튼 이벤트 초기화 및 재등록
            actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(buttonAction);
        }

        public void Show() { gameObject.SetActive(true); }  // 팝업 보이기
        public void Hide() { gameObject.SetActive(false); } // 팝업 숨기기
    }
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
            illustration: errorSprite,  // 오류 일러스트
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
            illustration: failSprite,   // 실패 일러스트
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