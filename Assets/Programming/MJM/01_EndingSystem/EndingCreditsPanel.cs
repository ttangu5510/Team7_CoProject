// EndingCreditsPanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingCreditsPanel : MonoBehaviour
{
    [Header("Scroll Root")]
    [SerializeField] RectTransform content;
    [SerializeField] float scrollSpeed = 80f; // 스크롤 속도 (초당 픽셀 단위)

    [Header("Controls")]
    [SerializeField] Button btnSkip;

    // 콜백을 둘로 분리
    public System.Action onCreditFinished; // 자연 종료(끝까지 스크롤)
    public System.Action onSkip;           // 스킵 버튼 눌림

    float startY, endY;  // 시작 위치와 끝 위치의 Y 좌표
    bool playing;        // 현재 크레딧이 진행 중인지 여부

    void Awake()
    {
        if (btnSkip) btnSkip.onClick.AddListener(SkipNow);
    }

    void OnEnable()
    {
        var parent = content.parent as RectTransform;       // 컨텐츠 부모 RectTransform
        startY = -parent.rect.height * 0.5f - content.rect.height * 0.5f; // 화면 아래쪽 시작 위치
        endY = parent.rect.height * 0.5f + content.rect.height * 0.5f;    // 화면 위쪽 끝 위치

        var pos = content.anchoredPosition;     // 현재 위치 불러오기
        pos.y = startY;                         // 시작 위치로 이동
        content.anchoredPosition = pos;

        playing = true;
    }

    void Update()
    {
        if (!playing) return;                   // 진행 중이 아니면 업데이트 X
        var pos = content.anchoredPosition;     // 현재 위치 가져오기
        pos.y += scrollSpeed * Time.deltaTime;  // 스크롤 속도에 맞춰 위로 이동
        content.anchoredPosition = pos;         // 새로운 위치 적용

        if (pos.y >= endY) FinishNow();  // 자연 종료
    }

    // ===== 종료 조건 둘 =====


    void FinishNow()
    {
        playing = false;
        onCreditFinished?.Invoke();
    }

    void SkipNow()
    {
        playing = false;
        onSkip?.Invoke();                // 스킵 전용 콜백
    }
}
