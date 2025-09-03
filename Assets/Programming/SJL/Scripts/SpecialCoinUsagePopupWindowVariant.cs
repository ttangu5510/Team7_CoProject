using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JYL;


public class SpecialCoinUsagePopupWindowVariant : MonoBehaviour
{
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI centerText;

    private int currentStep = 1;
    private int minStep = 1;
    private int maxStep = 10; // 최대 단계를 원하는 값으로

    private void Awake()
    {
        leftButton.onClick.AddListener(OnLeftButtonClicked);
        rightButton.onClick.AddListener(OnRightButtonClicked);

        UpdateUI();
    }

    private void OnLeftButtonClicked()
    {
        if (currentStep > minStep)
        {
            currentStep--;
            UpdateUI();
        }
    }

    private void OnRightButtonClicked()
    {
        if (currentStep < maxStep)
        {
            currentStep++;
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        centerText.text = $"{currentStep}회 진행";
        // 필요시 버튼 활성/비활성도 처리
        leftButton.interactable = (currentStep > minStep);
        rightButton.interactable = (currentStep < maxStep);
    }
}
