using JYL;
using SHG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


public class SpecialCoinUsagePopupWindowVariant : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI SpecialTrainingCoins;
    [SerializeField] private TextMeshProUGUI deployedPlayers;
    [SerializeField] private TextMeshProUGUI requiredSpecialTrainingCoins;
    [SerializeField] private TextMeshProUGUI remainingCoins;
    [Header("Step Selection Components")]
    [SerializeField] private TextMeshProUGUI centerText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [Header("Action Buttons")]
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    [Inject] private IResourceController resourceController;    // 자원 컨트롤러

    private int currentStep = 1;
    private int minStep = 1;
    private int maxStep = 10; // 최대 단계를 원하는 값으로

    private void Awake()
    {
        cancelButton.onClick.AddListener(() => gameObject.SetActive(false));
        confirmButton.onClick.AddListener(() => {
            Debug.Log($"특훈 코인 {currentStep}회 사용 확인됨");
            gameObject.SetActive(false);
        });
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

    private void UpdateUI() // UI 업데이트 메서드
    {
        centerText.text = $"{currentStep}회 진행";
        // 필요시 버튼 활성/비활성도 처리
        leftButton.interactable = (currentStep > minStep);
        rightButton.interactable = (currentStep < maxStep);

        // 실제 데이터로 UI 업데이트 필요
        SpecialTrainingCoins.text = resourceController.Coin.Value.ToString();
        deployedPlayers.text = "2명"; // 예시 값, 실제로는 배치된 선수 수로 변경 필요
        requiredSpecialTrainingCoins.text = (currentStep * 2).ToString(); // 예시 계산식
        remainingCoins.text = (resourceController.Coin.Value - (currentStep * 2)).ToString(); // 예시 계산식
    }
}
