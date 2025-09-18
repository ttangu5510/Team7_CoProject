using DG.Tweening;
using UnityEngine;

[DisallowMultipleComponent]
public class UIAnimator : MonoBehaviour
{
    [Header("Preset")]
    public UIAnimPreset preset = UIAnimPreset.SmoothFade;

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

    [Header("Dim Overlay (선택)")]
    public CanvasGroup blocker;          // 팝업 뒤 배경 딤(옵션)
    [Range(0f, 1f)] public float dimAlpha = 0.6f;
    public bool dimBlocksRaycasts = true;

    CanvasGroup cg;
    RectTransform rt;
    RectTransform parentRt;
    Sequence currentSeq;
    Vector2 initialAnchoredPos;
    bool initialized;

    void Awake()
    {
        Init();
        ApplyPreset(); // 실행 시 자동 프리셋 반영
    }

    void Init()
    {
        if (initialized) return;
        rt = transform as RectTransform;
        if (!rt)
        {
            Debug.LogWarning("[UIAnimator] RectTransform이 아닌 객체입니다.");
            return;
        }
        parentRt = rt.parent as RectTransform;
        cg = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        initialAnchoredPos = rt.anchoredPosition;
        initialized = true;
    }

#if UNITY_EDITOR
    // 인스펙터 값 변경 시 프리셋 재적용
    void OnValidate()
    {
        Init();
        ApplyPreset();
    }
#endif

    public void ApplyPreset()
    {
        switch (preset)
        {
            case UIAnimPreset.SmoothFade:
                inDuration = 0.25f; outDuration = 0.2f;
                inEase = Ease.OutCubic; outEase = Ease.InCubic;
                useScale = true; inStartScale = 0.95f;
                useAlpha = true; inStartAlpha = 0f;
                break;

            case UIAnimPreset.Pop:
                inDuration = 0.3f; outDuration = 0.2f;
                inEase = Ease.OutBack; outEase = Ease.InBack;
                useScale = true; inStartScale = 0.7f;
                useAlpha = true; inStartAlpha = 0f;
                break;

            case UIAnimPreset.FastFade:
                inDuration = 0.15f; outDuration = 0.15f;
                inEase = Ease.Linear; outEase = Ease.Linear;
                useScale = false; useAlpha = true; inStartAlpha = 0f;
                break;

            case UIAnimPreset.ElasticPop:
                inDuration = 0.25f; outDuration = 0.2f;
                inEase = Ease.OutElastic; outEase = Ease.InBack;
                useScale = true; inStartScale = 1.1f;
                useAlpha = true; inStartAlpha = 0f;
                break;

            case UIAnimPreset.DimFade:
                inDuration = 0.25f; outDuration = 0.2f;
                inEase = Ease.OutCubic; outEase = Ease.InCubic;
                useScale = false; useAlpha = true; inStartAlpha = 0f;
                break;

            case UIAnimPreset.SlideUp:
                inDuration = 0.5f; outDuration = 0.4f;
                inEase = Ease.OutCubic; outEase = Ease.InCubic;
                useScale = false; useAlpha = true; inStartAlpha = 1f; // 알파 고정
                break;

            case UIAnimPreset.SlideDown:
                inDuration = 0.3f; outDuration = 0.25f;
                inEase = Ease.OutCubic; outEase = Ease.InCubic;
                useScale = false; useAlpha = true; inStartAlpha = 1f; // 알파 고정
                break;
        }
    }

    float TravelY()
    {
        // 부모 Rect 기준 이동거리. 부모 없으면 Screen.height 사용
        if (parentRt) return Mathf.Max(parentRt.rect.height, rt.rect.height);
        return Screen.height;
    }

    public Tween PlayIn()
    {
        KillSeq();
        if (!rt) return null;

        if (useAlpha) cg.alpha = inStartAlpha;
        if (useScale) rt.localScale = Vector3.one * inStartScale;

        cg.interactable = false;
        cg.blocksRaycasts = false;

        currentSeq = DOTween.Sequence();

        switch (preset)
        {
            case UIAnimPreset.DimFade:
                if (blocker)
                {
                    blocker.alpha = 0f;
                    blocker.blocksRaycasts = dimBlocksRaycasts; // 배경 클릭 막기
                    currentSeq.Join(blocker.DOFade(dimAlpha, inDuration));
                }
                currentSeq.Join(cg.DOFade(1f, inDuration).SetEase(inEase));
                break;

            case UIAnimPreset.SlideUp:
                {
                    var d = TravelY();
                    rt.anchoredPosition = initialAnchoredPos + new Vector2(0f, -d);
                    currentSeq.Join(rt.DOAnchorPosY(initialAnchoredPos.y, inDuration).SetEase(inEase));
                    break;
                }
            case UIAnimPreset.SlideDown:
                {
                    var d = TravelY();
                    rt.anchoredPosition = initialAnchoredPos + new Vector2(0f, +d);
                    currentSeq.Join(rt.DOAnchorPosY(initialAnchoredPos.y, inDuration).SetEase(inEase));
                    break;
                }
            default:
                if (useAlpha) currentSeq.Join(cg.DOFade(1f, inDuration));
                if (useScale) currentSeq.Join(rt.DOScale(1f, inDuration).SetEase(inEase));
                break;
        }

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
        if (!rt) return null;

        cg.interactable = false;
        cg.blocksRaycasts = false;

        currentSeq = DOTween.Sequence();

        switch (preset)
        {
            case UIAnimPreset.DimFade:
                if (blocker) currentSeq.Join(blocker.DOFade(0f, outDuration));
                currentSeq.Join(cg.DOFade(0f, outDuration).SetEase(outEase));
                break;

            case UIAnimPreset.SlideUp:
                {
                    var d = TravelY();
                    currentSeq.Join(rt.DOAnchorPosY(initialAnchoredPos.y - d, outDuration).SetEase(outEase));
                    break;
                }
            case UIAnimPreset.SlideDown:
                {
                    var d = TravelY();
                    currentSeq.Join(rt.DOAnchorPosY(initialAnchoredPos.y + d, outDuration).SetEase(outEase));
                    break;
                }
            default:
                if (useAlpha) currentSeq.Join(cg.DOFade(0f, outDuration).SetEase(outEase));
                if (useScale) currentSeq.Join(rt.DOScale(inStartScale, outDuration).SetEase(outEase));
                break;
        }

        currentSeq.OnComplete(() =>
        {
            // Dim은 닫힌 뒤 클릭 허용
            if (blocker) blocker.blocksRaycasts = false;
            // 원래 자리로 복귀(슬라이드 계열에서 깔끔하게 초기화)
            rt.anchoredPosition = initialAnchoredPos;
        });

        return currentSeq;
    }

    void KillSeq()
    {
        if (currentSeq != null && currentSeq.IsActive()) currentSeq.Kill();
        currentSeq = null;
    }

    void OnDisable() => KillSeq();
    void OnDestroy() => KillSeq();
}

public enum UIAnimPreset
{
    SmoothFade,   // 1: 부드러운 페이드 + 살짝 확대
    Pop,          // 2: 퐁 튀어나오는 팝업
    FastFade,     // 3: 빠른 페이드
    ElasticPop,   // 4: 확대 후 탄성
    DimFade,      // 팝업 전용: 배경 딤 + 페이드
    SlideUp,      // 팝업 전용: 아래→위
    SlideDown     // 팝업 전용: 위→아래
}
