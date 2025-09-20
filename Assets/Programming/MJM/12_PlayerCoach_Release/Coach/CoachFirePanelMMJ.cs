using System;
using JYL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoachFirePanelMMJ : MonoBehaviour
{
    [SerializeField] private Image coachImage;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI messageText;

    public event Action OnCanceled;
    public event Action<CoachEntity> OnConfirmed;

    private CoachEntity current;
    private Sprite currentPortrait;

    private void Start()
    {
        cancelButton.onClick.AddListener(() =>
        {
            OnCanceled?.Invoke();
            gameObject.SetActive(false);
        });

        confirmButton.onClick.AddListener(() =>
        {
            OnConfirmed?.Invoke(current);
            gameObject.SetActive(false);
        });

        gameObject.SetActive(false); // 기본 비활성
    }

    /// <summary>
    /// 코치 해지 패널 열기
    /// </summary>
    public void Open(CoachEntity coach, Sprite portrait = null)
    {
        current = coach;
        currentPortrait = portrait;

        // 이미지 표시
        if (coachImage && portrait != null)
            coachImage.sprite = portrait;

        // 안내 문구 갱신  ----------  현재는 쓸 필요 없음
        // if (messageText)
        // {
        //     messageText.text =
        //         $"정말 계약을 취소하시겠습니까?\n이후 '스카우트 센터'에서 다시 영입할 수 있습니다.";
        // }

        gameObject.SetActive(true);
    }
}
