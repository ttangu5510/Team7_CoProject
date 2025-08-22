using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class Toast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [Header("Anim")]
    [SerializeField] private float fadeIn = 0.15f;
    [SerializeField] private float fadeOut = 0.20f;
    [SerializeField] private float moveUp = 30f;     // 살짝 떠오르는 연출

    private CanvasGroup cg;
    private RectTransform rt;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
    }

    public void SetText(string msg)
    {
        if (text) text.text = msg;
    }

    public IEnumerator PlayIn()
    {
        // 등장 애니메이션 (페이드 + 살짝 위로)
        float t = 0f;
        float startY = rt.anchoredPosition.y;
        cg.alpha = 0f;
        while (t < fadeIn)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeIn;
            cg.alpha = Mathf.Lerp(0f, 1f, k);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, startY + Mathf.Lerp(0f, moveUp, k));
            yield return null;
        }
        cg.alpha = 1f;
    }

    public IEnumerator PlayOut()
    {
        float t = 0f;
        float startY = rt.anchoredPosition.y;
        while (t < fadeOut)
        {
            t += Time.unscaledDeltaTime;
            float k = t / fadeOut;
            cg.alpha = Mathf.Lerp(1f, 0f, k);
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, startY + Mathf.Lerp(0f, moveUp * 0.4f, k));
            yield return null;
        }
        cg.alpha = 0f;
    }
}