using System;
using JYL;
using SHG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace JYL
{
    
}
public class SpecialTrainingCoinPUI : MonoBehaviour
{
    [Header("UI Components")] 
    [SerializeField] private TextMeshProUGUI increaseAmountText;
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

    private int currentStep = 1;
    private int minStep = 1;
    private int maxStep = 10; // 최대 단계를 원하는 값으로
    private int assignedPlayers = 0;
    private bool canTrain;
    private int trainingAmount = 5;

    // 외부 주입. 현재 코인 갯수
    private int coin;
    
    private void Awake()
    {
        cancelButton.OnClickAsObservable()
            .Subscribe(_ => OnClickCancelButton())
            .AddTo(this);
        
        confirmButton.OnClickAsObservable()
            .Subscribe(_ => OnClickConfirmButton())
            .AddTo(this);
        
        leftButton.OnClickAsObservable()
            .Subscribe(_=>OnLeftButtonClicked())
            .AddTo(this);

        rightButton.OnClickAsObservable()
            .Subscribe(_ => OnRightButtonClicked())
            .AddTo(this);
    }
    public void Init(int assignedNum, int coin)
    {
        canTrain = false;
        this.coin = coin;
        assignedPlayers = assignedNum;
        UpdateUI();
        SetConfirmButton();
    }

    private void OnLeftButtonClicked()
    {
        if (currentStep > minStep)
        {
            currentStep--;
            UpdateUI();
            SetConfirmButton();
        }
    }

    private void OnRightButtonClicked()
    {
        if (currentStep < maxStep)
        {
            currentStep++;
            UpdateUI();
            SetConfirmButton();
        }
    }

    // 특훈을 수행하는 버튼. 특훈할 수 있는 조건을 만족해야 버튼이 활성화 된다.
    private void OnClickConfirmButton()
    {
        // 이벤트 발행
        MessageBroker.Default.Publish(new SpecialTrainingEvent(canTrain, currentStep));
        Destroy(gameObject);
    }

    private void OnClickCancelButton()
    {
        Destroy(gameObject);
    }
    

    private void SetConfirmButton()
    {
        confirmButton.interactable = canTrain;
    }

    private void UpdateUI() // UI 업데이트 메서드
    {
        increaseAmountText.text = $"모든 능력치 <color=#FF3333>{currentStep * trainingAmount}</color> 증가";
        centerText.text = $"{currentStep}회 진행";
        // 필요시 버튼 활성/비활성도 처리
        leftButton.interactable = (currentStep > minStep);
        rightButton.interactable = (currentStep < maxStep);

        // 실제 데이터로 UI 업데이트 필요
        SpecialTrainingCoins.text = $"{coin}개";
        deployedPlayers.text = $"{assignedPlayers} 명";
        requiredSpecialTrainingCoins.text = $"{currentStep * assignedPlayers} 개";
        
        int remainCoin = coin - currentStep * assignedPlayers;
        if (remainCoin >= 0)
        {
            remainingCoins.text = $"{remainCoin} 개";
            canTrain = true;
        }
        else
        {
            remainingCoins.text = $"<color=red>{remainCoin}</color> 개";
            canTrain = false;
        }
    }
}
