using System;
using JYL;
using UnityEngine;
using UnityEngine.UI;

public class CoachFirePanelMMJ : MonoBehaviour
{
    [SerializeField] private Image coachImage;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    public event Action OnCanceled;
    public event Action<CoachEntity> OnConfirmed;

    private CoachEntity current;
    private Sprite currentPortrait;
    private bool _wired;

    private void Awake()
    {
        WireButtonsOnce();
    }

    private void WireButtonsOnce()
    {
        if (_wired) return;
        _wired = true;

        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            OnCanceled?.Invoke();
            gameObject.SetActive(false);
        });

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            if (current != null) OnConfirmed?.Invoke(current);
            gameObject.SetActive(false);
        });
    }

    public void Open(CoachEntity coach, Sprite portrait = null)
    {
        current = coach;
        currentPortrait = portrait;
        if (coachImage && portrait) coachImage.sprite = portrait;

        var cg = GetComponent<CanvasGroup>();
        if (cg) { cg.alpha = 1f; cg.interactable = true; cg.blocksRaycasts = true; }

        gameObject.SetActive(true);
    }
}
