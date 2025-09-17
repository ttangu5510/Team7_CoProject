using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PanelAnimator : MonoBehaviour
{
    [Header("Preset")]
    public PanelAnimPreset preset = PanelAnimPreset.SmoothFade;

    [Header("In/Out (자동 세팅됨)")]
    public float inDuration = 0.25f;
    public float outDuration = 0.2f;
    public Ease inEase = Ease.OutCubic;
    public Ease outEase = Ease.InCubic;

    [Header("Effects")]
    public bool useScale = true;
    public float inStartScale = 0.95f;
    public bool useAlpha = true;
    public float inStartAlpha = 0f;

    CanvasGroup cg;
    RectTransform rt;
    Sequence currentSeq;

    void Awake()
    {
        rt = transform as RectTransform;
        cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>();
        ApplyPreset(); // 실행 시 자동으로 프리셋 반영
    }

#if UNITY_EDITOR
    // 인스펙터에서 값 바뀌면 즉시 반영 (에디터 전용)
    void OnValidate()
    {
        ApplyPreset();
    }
#endif

    public void ApplyPreset()
    {
        switch (preset)
        {
            case PanelAnimPreset.SmoothFade: // 1
                inDuration = 0.25f;
                outDuration = 0.2f;
                inEase = Ease.OutCubic;
                outEase = Ease.InCubic;
                useScale = true; inStartScale = 0.95f;
                useAlpha = true; inStartAlpha = 0f;
                break;

            case PanelAnimPreset.Pop: // 2
                inDuration = 0.3f;
                outDuration = 0.2f;
                inEase = Ease.OutBack;
                outEase = Ease.InBack;
                useScale = true; inStartScale = 0.7f;
                useAlpha = true; inStartAlpha = 0f;
                break;

            case PanelAnimPreset.FastFade: // 3
                inDuration = 0.15f;
                outDuration = 0.15f;
                inEase = Ease.Linear;
                outEase = Ease.Linear;
                useScale = false;
                useAlpha = true; inStartAlpha = 0f;
                break;

            case PanelAnimPreset.ElasticPop: // 4
                inDuration = 0.25f;
                outDuration = 0.2f;
                inEase = Ease.OutElastic;
                outEase = Ease.InBack;
                useScale = true; inStartScale = 1.1f;
                useAlpha = true; inStartAlpha = 0f;
                break;
        }
    }

    public Tween PlayIn()
    {
        KillSeq();
        if (useAlpha) cg.alpha = inStartAlpha;
        if (useScale) rt.localScale = Vector3.one * inStartScale;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        currentSeq = DOTween.Sequence();
        if (useAlpha) currentSeq.Join(cg.DOFade(1f, inDuration));
        if (useScale) currentSeq.Join(rt.DOScale(1f, inDuration).SetEase(inEase));

        currentSeq.OnComplete(() =>
        {
            cg.interactable = true;
            cg.blocksRaycasts = true;
        });

        return currentSeq;
    }

    public Tween PlayOut()
    {
        KillSeq();
        cg.interactable = false;
        cg.blocksRaycasts = false;

        currentSeq = DOTween.Sequence();

        if (useAlpha) currentSeq.Join(cg.DOFade(0f, outDuration).SetEase(outEase));
        if (useScale) currentSeq.Join(rt.DOScale(inStartScale, outDuration).SetEase(outEase));

        return currentSeq;
    }

    void KillSeq()
    {
        if (currentSeq != null && currentSeq.IsActive()) currentSeq.Kill();
        currentSeq = null;
    }
}

public enum PanelAnimPreset
{
    SmoothFade,   // 1번: 부드럽게 페이드+살짝 확대
    Pop,          // 2번: 퐁 튀어나오는 팝업
    FastFade,     // 3번: 빠른 페이드
    ElasticPop    // 4번: 확대 후 흔들리며 정착
}
