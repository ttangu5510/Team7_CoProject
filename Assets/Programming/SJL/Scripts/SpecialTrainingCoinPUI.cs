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

    // 외부 주입. 현재 코인 갯수
    private int coin = 0;
    
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
    
    public void Init(int assignedNum, int coin)
    {
        canTrain = false;
        this.coin = coin;
        assignedPlayers = assignedNum;
        UpdateUI();
        SetConfirmButton();
    }

    private void SetConfirmButton()
    {
        confirmButton.interactable = canTrain;
    }

    private void UpdateUI() // UI 업데이트 메서드
    {
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
