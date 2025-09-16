using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PanelAnimator : MonoBehaviour
{
    [Header("In/Out")]
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

        currentSeq.OnComplete(() => {
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
