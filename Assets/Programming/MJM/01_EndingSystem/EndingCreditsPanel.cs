// EndingCreditsPanel.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingCreditsPanel : MonoBehaviour
{
    [Header("Scroll Root")]
    [SerializeField] RectTransform content;
    [SerializeField] float scrollSpeed = 80f;

    [Header("Controls")]
    [SerializeField] Button btnSkip;

    // 콜백을 둘로 분리
    public System.Action onCreditFinished; // 자연 종료(끝까지 스크롤)
    public System.Action onSkip;           // 스킵 버튼 눌림

    float startY, endY;
    bool playing;

    void Awake()
    {
        if (btnSkip) btnSkip.onClick.AddListener(SkipNow);
    }

    void OnEnable()
    {
        var parent = content.parent as RectTransform;
        startY = -parent.rect.height * 0.5f - content.rect.height * 0.5f;
        endY = parent.rect.height * 0.5f + content.rect.height * 0.5f;

        var pos = content.anchoredPosition;
        pos.y = startY;
        content.anchoredPosition = pos;

        playing = true;
    }

    void Update()
    {
        if (!playing) return;
        var pos = content.anchoredPosition;
        pos.y += scrollSpeed * Time.deltaTime;
        content.anchoredPosition = pos;

        if (pos.y >= endY) FinishNow();  // 자연 종료
    }

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
