using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class TrainingBox : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button circuitTrainingPositioningPlayers;
        [SerializeField] private Button ladderDrillTrainingPositioningPlayers;
        [SerializeField] private Button sprintsPositioningPlayers;
        [SerializeField] private Button burpeeTestsPositioningPlayers;

        [SerializeField] private Button startTrainingButton;
        [SerializeField] private Button ResetPlayerPlacementButton;
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI circuitTrainingText;
        [SerializeField] private TextMeshProUGUI ladderDrillTrainingText;
        [SerializeField] private TextMeshProUGUI sprintsText;
        [SerializeField] private TextMeshProUGUI burpeeTestsText;
        [Header("Player & Coach")]
        [SerializeField] private Player currentPlayer; // 현재 훈련 중인 선수
        [SerializeField] private bool coachAssigned = false; // 코치 배치 여부

        // 각 훈련 슬롯별로 4명까지 선수 보관
        private readonly List<Player> circuitPlayers = new List<Player>();
        private readonly List<Player> ladderDrillPlayers = new List<Player>();
        private readonly List<Player> sprintsPlayers = new List<Player>();
        private readonly List<Player> burpeePlayers = new List<Player>();

        private void Awake()
        {
            // 실제 프로젝트에서는 배치 UI에서 선택한 선수 객체를 받아서 배치합니다.
            // 아래는 예시로 더미 선수 할당
            circuitTrainingPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(circuitPlayers, circuitTrainingText, "서킷 트레이닝"))
                .AddTo(this);

            ladderDrillTrainingPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(ladderDrillPlayers, ladderDrillTrainingText, "사다리 드릴 트레이닝"))
                .AddTo(this);

            sprintsPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(sprintsPlayers, sprintsText, "단거리 전력 질주"))
                .AddTo(this);

            burpeeTestsPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(burpeePlayers, burpeeTestsText, "버피 테스트"))
                .AddTo(this);

            ResetPlayerPlacementButton.OnClickAsObservable()
                .Subscribe(_ => {
                    circuitPlayers.Clear(); ladderDrillPlayers.Clear();
                    sprintsPlayers.Clear(); burpeePlayers.Clear();
                    UpdateAllAssignmentTexts();
                }).AddTo(this);

            startTrainingButton.OnClickAsObservable()
                .Subscribe(_ => StartAllTrainings())
                .AddTo(this);

            UpdateAllAssignmentTexts();
        }

        private void PositioningPlayers(List<Player> targetList, TextMeshProUGUI uiText, string routine)
        {
            if (targetList.Count >= 4)
                return;

            // 예시: 임시 플에이어 생성. 실제로는 선수 선택 UI 연동 필요
            Player dummy = new Player { name = $"Player{Random.Range(1, 100)}" };
            targetList.Add(dummy);
            UpdateAssignmentText(targetList, uiText);
        }

        private void UpdateAssignmentText(List<Player> list, TextMeshProUGUI uiText)
        {
            uiText.text = $"배치된 선수 : {list.Count}/4";
        }
        private void UpdateAllAssignmentTexts()
        {
            UpdateAssignmentText(circuitPlayers, circuitTrainingText);
            UpdateAssignmentText(ladderDrillPlayers, ladderDrillTrainingText);
            UpdateAssignmentText(sprintsPlayers, sprintsText);
            UpdateAssignmentText(burpeePlayers, burpeeTestsText);
        }

        // 모든 루틴에 배치된 선수 훈련 실행
        private void StartAllTrainings()
        {
            TrainPlayers(circuitPlayers, "CircuitTraining");
            TrainPlayers(ladderDrillPlayers, "LadderDrillTraining");
            TrainPlayers(sprintsPlayers, "SprintsTraining");
            TrainPlayers(burpeePlayers, "BurpeeTestsTraining");
        }

        // 훈련 메서드
        private void TrainPlayers(List<Player> players, string routine)
        {
            foreach (var player in players)
            {
                if (IsTrainingFailed(player))
                {
                    ApplyInjury(player);
                    Debug.Log($"{player.name} 선수가 피로 누적으로 부상당했습니다! ({routine})");
                    continue;
                }

                // 훈련별 능력치 및 피로 상승
                switch (routine)
                {
                    case "CircuitTraining":
                        player.stamina += 1;
                        player.agility += 1;
                        player.flexibility += 1;
                        IncreaseFatigue(player);
                        break;
                    case "LadderDrillTraining":
                        player.speed += 1;
                        player.agility += 1;
                        IncreaseFatigue(player);
                        break;
                    case "SprintsTraining":
                        player.stamina += 2;
                        player.speed += 1;
                        IncreaseFatigue(player);
                        break;
                    case "BurpeeTestsTraining":
                        player.stamina += 1;
                        player.mental += 1;
                        IncreaseFatigue(player);
                        break;
                }

                // 피로도 5이상시 부상 가능성
                if (player.fatigue >= 5)
                {
                    float injuryChance = 0.1f * (player.fatigue - 4);
                    if (Random.value < injuryChance)
                    {
                        player.isInjured = true;
                        Debug.Log($"{player.name} 선수 추가 부상 획득! ({routine})");
                    }
                }
                Debug.Log($"{player.name} 선수가 {routine} 훈련을 완료했습니다.");
            }
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