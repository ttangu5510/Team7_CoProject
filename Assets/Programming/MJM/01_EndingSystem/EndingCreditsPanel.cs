using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingCreditsPanel : MonoBehaviour
{
    [Header("Scroll Root")]
    [SerializeField] RectTransform content; // 크레딧 전체 텍스트의 RectTransform
    [SerializeField] float scrollSpeed = 80f; // px/sec
    [Header("Controls")]
    [SerializeField] Button btnSkip;

    public System.Action onCreditFinished;   // ★ 끝나면 타이틀로

    float startY, endY;
    bool playing;

    void Awake()
    {
        if (btnSkip) btnSkip.onClick.AddListener(FinishNow);
    }

    void OnEnable()
    {
        // 시작/종료 위치 계산(아래에서 위로 올림)
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

        if (pos.y >= endY) FinishNow();
    }

    public void FinishNow()
    {
        if (!gameObject.activeInHierarchy) return;
        playing = false;
        onCreditFinished?.Invoke(); // 컨트롤러에서 타이틀 로드
    }
}
