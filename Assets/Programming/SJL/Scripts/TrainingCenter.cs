using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class TrainingCenter : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button FacilityInformationButton;
        [SerializeField] private Button trainingButton;
        [SerializeField] private Button specialTrainingButton;

        [SerializeField] private Button circuitTrainingPositioningPlayers;
        [SerializeField] private Button ladderDrillTrainingPositioningPlayers;
        [SerializeField] private Button sprintsPositioningPlayers;
        [SerializeField] private Button burpeeTestsPositioningPlayers;

        [SerializeField] private Button startTrainingButton;
        [SerializeField] private Button ResetPlayerPlacementButton;
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI explanatoryText;
        [Header("panel")]
        [SerializeField] private GameObject FacilityInformationBox;
        [SerializeField] private GameObject TrainingBox;
        [SerializeField] private GameObject specialTrainingBox;
        [Header("Player & Coach")]
        [SerializeField] private Player currentPlayer; // 현재 훈련 중인 선수
        [SerializeField] private bool coachAssigned = false; // 코치 배치 여부

        private void Awake()
        {
            // UniRx로 버튼 클릭 처리
            FacilityInformationButton.OnClickAsObservable()
                .Subscribe(_ => ShowPanel(PanelType.FacilityInformation)).AddTo(this);

            trainingButton.OnClickAsObservable()
                .Subscribe(_ => ShowPanel(PanelType.Training)).AddTo(this);

            specialTrainingButton.OnClickAsObservable()
                .Subscribe(_ => ShowPanel(PanelType.SpecialTraining)).AddTo(this);

            // 훈련 버튼 클릭 시 예시: 루틴 "CircuitTraining"으로 훈련 진행
            trainingButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    if (currentPlayer != null)
                    {
                        TrainPlayer(currentPlayer, "CircuitTraining");
                        UpdateExplanatoryText();
                    }
                }).AddTo(this);
        }

        private enum PanelType { FacilityInformation, Training, SpecialTraining }

        private void ShowPanel(PanelType type)
        {
            switch (type)
            {
                case PanelType.FacilityInformation:
                    explanatoryText.text = "시설 : 0 단계\n훈련 스탯 추가 수치 : + 0\n훈련과 특훈 진행 가능.";
                    FacilityInformationBox.SetActive(true);
                    TrainingBox.SetActive(false);
                    specialTrainingBox.SetActive(false);
                    break;
                case PanelType.Training:
                    explanatoryText.text = "선수틀을 배치하여 훈련시킬 수 있습니다.\n루틴에 따라 상승하는 능력치가 달라집니다.";
                    FacilityInformationBox.SetActive(false);
                    TrainingBox.SetActive(true);
                    specialTrainingBox.SetActive(false);
                    break;
                case PanelType.SpecialTraining:
                    explanatoryText.text = "선수틀을 배치하여 훈련시킬 수 있습니다.\n루틴에 따라 상승하는 능력치가 달라집니다.";
                    FacilityInformationBox.SetActive(false);
                    TrainingBox.SetActive(false);
                    specialTrainingBox.SetActive(true);
                    break;
            }
        }

        private void UpdateExplanatoryText()
        {
            explanatoryText.text = $"훈련 중: {currentPlayer.name}\n" +
                                   $"피로도: {currentPlayer.fatigue}\n" +
                                   $"부상: {(currentPlayer.isInjured ? "예" : "아니오")}";
        }

        // 훈련 메서드
        private void TrainPlayer(Player player, string routine)
        {
            if (!coachAssigned)
            {
                Debug.Log("코치가 배치되어 있지 않습니다. 훈련을 시작할 수 없습니다.");
                return;
            }
            // 예시: 루틴에 따른 능력치 상승
            switch (routine)
            {
                case "CircuitTraining":
                    player.stamina += 1;
                    player.agility += 1;
                    player.flexibility += 1;
                    player.fatigue += 2; // 피로도 증가
                    break;
                case "StrengthTraining":
                    player.stamina += 2;
                    player.mental += 1;
                    player.fatigue += 3; // 피로도 증가
                    break;
                case "EnduranceTraining":
                    player.stamina += 1;
                    player.speed += 1;
                    player.balance += 1;
                    player.fatigue += 2; // 피로도 증가
                    break;
                default:
                    Debug.Log("알 수 없는 훈련 루틴입니다.");
                    break;
            }
            // 부상 확률 계산 (예시: 피로도가 5 이상일 때 부상 확률 증가)
            if (player.fatigue >= 5)
            {
                float injuryChance = 0.1f * (player.fatigue - 4); // 피로도가 높을수록 부상 확률 증가
                if (Random.value < injuryChance)
                {
                    player.isInjured = true;
                    Debug.Log($"{player.name} 선수가 부상을 입었습니다!");
                }
            }
            Debug.Log($"{player.name} 선수가 {routine} 훈련을 완료했습니다.");
        }

        // 피로도 무작위 상승, 코치 있을 때 감소
        private void IncreaseFatigue(Player player)
        {
            int fatigueIncrease = UnityEngine.Random.Range(7, 13); // 7~12

            if (coachAssigned)
            {
                fatigueIncrease = Mathf.Max(0, fatigueIncrease - 5); // 코치가 피로도 증가 5 감소 시킴
            }

            player.fatigue += fatigueIncrease;
        }

        // 훈련 실패 확률 계산 (피로도 70 이상부터 확률 상승)
        private bool IsTrainingFailed(Player player)
        {
            if (player.fatigue < 70) return false;

            float failChance = (player.fatigue - 70) * 0.05f;
            return UnityEngine.Random.value < failChance;
        }

        // 부상 처리
        private void ApplyInjury(Player player)
        {
            player.isInjured = true;
        }
    }


}

