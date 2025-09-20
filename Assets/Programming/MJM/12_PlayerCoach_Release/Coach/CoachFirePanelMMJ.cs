using System;
using JYL;
using UnityEngine;
using UnityEngine.UI;

public class CoachFirePanelMMJ : MonoBehaviour
{
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    public event Action OnCanceled;
    public event Action<CoachEntity> OnConfirmed;

    private CoachEntity current;

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

        gameObject.SetActive(false);
    }

    public void Open(CoachEntity coach)
    {
        current = coach;
        gameObject.SetActive(true);
    }
}
