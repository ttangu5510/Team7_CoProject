using System;
using JYL;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class PlayerFirePanelMMJ : MonoBehaviour
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    public event Action OnCanceled;
    public event Action<DomAthEntity> OnConfirmed;

    private DomAthEntity current;
    private Sprite currentPortrait;
   //  private bool _wired;

    private void Awake()
    {
        WireButtonsOnce();
    }

    private void WireButtonsOnce()
    {
       // if (_wired) return;
       // _wired = true;

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

    public void Open(DomAthEntity athlete, Sprite portrait = null)
    {
        current = athlete;
        currentPortrait = portrait;
        if (playerImage && portrait) playerImage.sprite = portrait;

        // 혹시 CanvasGroup이 있다면 첫 프레임 인터랙션 보장
        var cg = GetComponent<CanvasGroup>();
        if (cg) { cg.alpha = 1f; cg.interactable = true; cg.blocksRaycasts = true; }

        gameObject.SetActive(true);
    }
}
