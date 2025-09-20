using System;
using JYL;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFirePanelMMJ : MonoBehaviour
{
    [SerializeField] private Image playerImage;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    public event Action OnCanceled;
    public event Action<DomAthEntity> OnConfirmed;

    private DomAthEntity current;
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

    public void Open(DomAthEntity athlete, Sprite portrait = null)
    {
        current = athlete;
        currentPortrait = portrait;
        if (playerImage && portrait) playerImage.sprite = portrait;
        gameObject.SetActive(true);
    }
}
