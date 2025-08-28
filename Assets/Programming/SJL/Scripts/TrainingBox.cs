using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JYL;
using SHG;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SJL
{
    public enum TrainingType
    {
        None, Circuit, LadderDrill, Sprint, BurpeeTest
    }
    
    public class TrainingBox : MonoBehaviour
    {
        [Header("Training Center")] 
        [SerializeField] private GameObject trainingCenter;
        
        [Header("Button")] 
        [SerializeField] private Button circuitTrainingPositioningPlayers;
        [SerializeField] private Button ladderDrillTrainingPositioningPlayers;
        [SerializeField] private Button sprintsPositioningPlayers;
        [SerializeField] private Button burpeeTestsPositioningPlayers;
        [SerializeField] private Button popupCloseButton;

        [SerializeField] private Button startTrainingButton;
        [SerializeField] private Button ResetPlayerPlacementButton;
        
        [Header("Text")] 
        [SerializeField] private TextMeshProUGUI circuitTrainingText;
        [SerializeField] private TextMeshProUGUI ladderDrillTrainingText;
        [SerializeField] private TextMeshProUGUI sprintsText;
        [SerializeField] private TextMeshProUGUI burpeeTestsText;
        
        [Header("Assignment panel")]
        [SerializeField] private PlayerListInformation assignmentPanel;

        // 플레이어 서비스 의존성 주입
        [Inject] private DomAthService athleteService;
        // 시간 컨트롤러 의존성 주입
        [Inject] private ITimeFlowController flowController;

        public List<DomAthEntity> athleteList = new();
        public Dictionary<DomAthEntity,TrainingType> assignDict = new();
        
        
        
        private void Awake()
        {
            // 실제 프로젝트에서는 배치 UI에서 선택한 선수 객체를 받아서 배치합니다.
            // 아래는 예시로 더미 선수 할당
            athleteList.Clear();
            assignDict.Clear();
            
            circuitTrainingPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, circuitTrainingText, TrainingType.Circuit))
                .AddTo(this);

            ladderDrillTrainingPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, ladderDrillTrainingText, TrainingType.LadderDrill))
                .AddTo(this);

            sprintsPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, sprintsText, TrainingType.Sprint))
                .AddTo(this);

            burpeeTestsPositioningPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, burpeeTestsText, TrainingType.BurpeeTest))
                .AddTo(this);

            ResetPlayerPlacementButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    assignDict.Clear();
                    foreach (var ath in athleteList)
                    {
                        assignDict[ath] = TrainingType.None;
                    }
                    UpdateAllAssignmentTexts();
                }).AddTo(this);

            startTrainingButton.OnClickAsObservable()
                .Subscribe(_ => TrainPlayers())
                .AddTo(this);
            
            popupCloseButton.OnClickAsObservable()
                .Subscribe(_ => UpdateAllAssignmentTexts())
                .AddTo(this);

            UpdateAllAssignmentTexts();
        }

        private void OnEnable()
        {
            // 훈련 가능한 선수만 리스트로 가져옴
            athleteList = athleteService.GetAllRecruitedAthleteList()
                .Where(ath => ath.curState == AthleteState.Active)
                .ToList();
            
            // 모든 훈련가능한 선수들을 현재 배치상황 None으로 설정
            foreach (var ath in athleteList)
            {
                assignDict[ath] = TrainingType.None;
            }
        }

        // 선수 배치. 선수 선택 UI 팝업 띄움
        private void PositioningPlayers(List<DomAthEntity> targetList, TextMeshProUGUI uiText, TrainingType type)
        {
            if (assignDict.Values.Count(t => t == type) >= 4) // 해당 훈련에 배치된 선수가 이미 4명 이상임
                return;
            
            assignmentPanel.gameObject.SetActive(true);
            assignmentPanel.SelectTrainingAthlete(targetList, type, assignDict);
            
            UpdateAssignmentText(uiText, type);
        }

        // 각 종목의 배치 현황 텍스트 업데이트
        private void UpdateAssignmentText(TextMeshProUGUI uiText, TrainingType type)
        {
            uiText.text = $"$배치된 선수 : {assignDict.Values.Count(t => t == type)}/4";
        }

        // 전종먹의 배치 현황 텍스트 업데이트
        private void UpdateAllAssignmentTexts()
        {
            circuitTrainingText.text = $"배치된 선수 : {assignDict.Values.Count(t => t == TrainingType.Circuit)}/4";
            ladderDrillTrainingText.text = $"배치된 선수 : {assignDict.Values.Count(t => t == TrainingType.LadderDrill)}/4";
            sprintsText.text = $"배치된 선수 : {assignDict.Values.Count(t => t == TrainingType.Sprint)}/4";
            burpeeTestsText.text = $"배치된 선수 : {assignDict.Values.Count(t => t == TrainingType.BurpeeTest)}/4";
        }

        // 훈련 메서드
        private void TrainPlayers()
        {
            foreach (var player in assignDict.Keys)
            {
                // 훈련별 능력치 및 피로 상승
                switch (assignDict[player])
                {
                    case TrainingType.Circuit:
                        player.TrainAthlete(Ability.Health);
                        break;
                    case TrainingType.LadderDrill:
                        player.TrainAthlete(Ability.Quickness);
                        break;
                    case TrainingType.Sprint:
                        player.TrainAthlete(Ability.Flexibility);
                        break;
                    case TrainingType.BurpeeTest:
                        player.TrainAthlete(Ability.Balance);
                        break;
                }

                Debug.Log($"{player.entityName} 선수가 {assignDict[player].ToString()} 훈련을 완료했습니다.");
            }
            athleteList.Clear(); // 훈련이 끝났으니 초기화
            assignDict.Clear();
            // 시간 보내기
            flowController.ProgressWeek();
            // 패널 종료
            trainingCenter.SetActive(false);
        }
    }
}