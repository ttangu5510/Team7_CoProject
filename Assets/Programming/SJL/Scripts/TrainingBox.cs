using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JYL;
using SHG;
using TMPro;
using UniRx;
using UnityEditor.ShaderGraph.Internal;
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
        // 부모 칸바스
        private Canvas trainingCenter;
        
        [Header("Button")] 
        [SerializeField] private Button circuitPlayers;
        [SerializeField] private Button ladderPlayers;
        [SerializeField] private Button sprintsPlayers;
        [SerializeField] private Button burpeePlayers;

        [SerializeField] private Button trainingCloseButton;
        [SerializeField] private Button startTrainingButton;
        [SerializeField] private Button resetButton;
        
        // [Header("Text")] 
        // [SerializeField] private TextMeshProUGUI circuitTrainingText;
        // [SerializeField] private TextMeshProUGUI ladderDrillTrainingText;
        // [SerializeField] private TextMeshProUGUI sprintsText;
        // [SerializeField] private TextMeshProUGUI burpeeTestsText;
        
        [Header("Assigned AthleteImage")] // 편성된 선수들의 이미지 및 이름 관리
        [SerializeField] private AssignedAthPanel circuitImage;
        [SerializeField] private AssignedAthPanel ladderImage;
        [SerializeField] private AssignedAthPanel sprintsImage;
        [SerializeField] private AssignedAthPanel burpeeImage;
        
        [Header("Set Refs")]
        [SerializeField] private AthleteListPanel assignmentPanel;
        [SerializeField] private ConfirmPUI confirmPui;

        
        // 플레이어 서비스 의존성 주입
        [Inject] private DomAthService athleteService;
        // 시간 컨트롤러 의존성 주입
        [Inject] private ITimeFlowController flowController;

        public List<DomAthEntity> athleteList = new();
        private Dictionary<DomAthEntity,TrainingType> assignDict = new();

        private int trainingVolume = 1;
        private CoachGrade coach;
        private TrainingType cachedType; // 이벤트 연결을 위해 들고 있음.
        
        // 팝업 생성 시 사용되는 텍스트
        private string[] puiString;

        private void Awake()
        {
            trainingCenter = GetComponentInParent<Canvas>();
            
            assignDict.Clear();
            
            EnableInit();
            
            circuitPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, TrainingType.Circuit))
                .AddTo(this);

            ladderPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, TrainingType.LadderDrill))
                .AddTo(this);

            sprintsPlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, TrainingType.Sprint))
                .AddTo(this);

            burpeePlayers.OnClickAsObservable()
                .Subscribe(_ => PositioningPlayers(athleteList, TrainingType.BurpeeTest))
                .AddTo(this);

            resetButton.OnClickAsObservable()
                .Subscribe(_ => EnableInit()).AddTo(this);

            startTrainingButton.OnClickAsObservable()
                .Subscribe(_ => CheckAllAssign()).AddTo(this);
            
            trainingCloseButton.OnClickAsObservable()
                .Subscribe(_ => UpdateAllAssignment()).AddTo(this);
           
            // 텍스트 세팅
            puiString = new string[2] { "아직 배치되지 않은 슬롯이 있습니다.\n훈련을 진행하시겠습니까?",""};
            // circuitTrainingText.text = $"순발력, 기술 상승";
            // ladderDrillTrainingText.text = $"체력, 기술 상승";
            // sprintsText.text = $"체력, 유연성 상승";
            // burpeeTestsText.text = $"속도, 균형감각 상승";
            
            // 배치된 선수들 정보 최신화 이벤트 구독
            assignmentPanel.CloseSubject.Subscribe(isClosed =>
            {
                if (isClosed)
                {
                    UpdateAssignment(cachedType);
                }
            }).AddTo(this);
        }

        private void OnEnable()
        {
            EnableInit();
        }

        private void EnableInit()
        {
            // 훈련 가능한 선수, 부상당한 선수들을 리스트로 가져옴
            athleteList = athleteService.GetAllRecruitedAthleteList()
                .Where(ath => ath.curState == AthleteState.Active || ath.curState == AthleteState.Injured)
                .ToList();
            
            // 모든 훈련가능한 선수들을 현재 배치상황 None으로 설정
            assignDict.Clear();
            foreach (var ath in athleteList)
            {
                assignDict[ath] = TrainingType.None;
            }
            
            // UI 초기화
            UpdateAllAssignment();
        }

        // 선수 배치. 선수 선택 UI 팝업 띄움
        private void PositioningPlayers(List<DomAthEntity> targetList, TrainingType type)
        {
            assignmentPanel.gameObject.SetActive(true);
            assignmentPanel.SelectTrainingAthlete(targetList, type, assignDict);
            cachedType = type;
        }

        // 각 종목의 배치 현황 UI 업데이트
        private void UpdateAssignment(TrainingType type)
        {
            switch (type)
            {
                case TrainingType.Circuit:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        circuitImage.gameObject.SetActive(true);
                        circuitImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        circuitImage.gameObject.SetActive(false);
                    }
                    break;
                case TrainingType.LadderDrill:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        ladderImage.gameObject.SetActive(true);
                        ladderImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        ladderImage.gameObject.SetActive(false);
                    }
                    break;
                case TrainingType.Sprint:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        sprintsImage.gameObject.SetActive(true);
                        sprintsImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        sprintsImage.gameObject.SetActive(false);
                    }
                    break;
                case TrainingType.BurpeeTest:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        burpeeImage.gameObject.SetActive(true);
                        burpeeImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        burpeeImage.gameObject.SetActive(false);
                    }
                    break;
            }
        }

        // 전 종목의 배치 현황 텍스트 업데이트
        private void UpdateAllAssignment()
        {
            int assignedCircuitCount = assignDict.Values.Count(t => t == TrainingType.Circuit);
            if (assignedCircuitCount > 0)
            { 
                circuitImage.gameObject.SetActive(true);
                circuitImage.UpdateUI(assignDict, TrainingType.Circuit);
            }
            else
            {
                circuitImage.gameObject.SetActive(false);
            }
            int assignedLadderCount = assignDict.Values.Count(t => t == TrainingType.LadderDrill);
            if (assignedLadderCount > 0)
            {
                ladderImage.gameObject.SetActive(true);
                ladderImage.UpdateUI(assignDict, TrainingType.LadderDrill);
            }
            else
            {
                ladderImage.gameObject.SetActive(false);
            }
            int assignedSprintsCount = assignDict.Values.Count(t => t == TrainingType.Sprint);
            if (assignedSprintsCount > 0)
            {
                sprintsImage.gameObject.SetActive(true);
                sprintsImage.UpdateUI(assignDict, TrainingType.Sprint);
            }
            else
            {
                sprintsImage.gameObject.SetActive(false);
            }
            int assignedBurpeeCount = assignDict.Values.Count(t => t == TrainingType.BurpeeTest);
            if (assignedBurpeeCount > 0)
            {
                burpeeImage.gameObject.SetActive(true);
                burpeeImage.UpdateUI(assignDict, TrainingType.BurpeeTest);
            }
            else
            {
                burpeeImage.gameObject.SetActive(false);
            }
                
        }

        // 훈련 전에 모든 슬롯에 배치가 완료 되었는지 확인
        private void CheckAllAssign()
        {
            // 현재 배치된 선수들의 숫자 카운트
            int assignedCount = assignDict.Values.Count(t => t != TrainingType.None);
            int allAthleteCount = athleteList.Count(ath => ath.curState != AthleteState.Injured);
            
            // 전부 배치된 경우
            if ( assignedCount == 16 || allAthleteCount == assignedCount )
            {
                TrainPlayers();
            }
            
            // 아닌 경우, 확인 창 생성
            else
            {
                ConfirmPUI pui = Instantiate(confirmPui, trainingCenter.transform);
                pui.Init(puiString, ConfirmState.OnlyYesOrNo);
                pui.ConfirmSubject.Subscribe(confirm =>
                {
                    if (confirm) TrainPlayers(); // "예" 누르면 훈련 시작
                }).AddTo(pui);
            }
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
                        //player.TrainAthlete(Ability.Health);
                        athleteService.TrainAthlete(player.entityName, Ability.Health, trainingVolume, (int)coach); // 피로도 1 증가
                        break;
                    case TrainingType.LadderDrill:
                        //player.TrainAthlete(Ability.Quickness);
                        athleteService.TrainAthlete(player.entityName, Ability.Quickness, trainingVolume, (int)coach); // 피로도 1 증가
                        break;
                    case TrainingType.Sprint:
                        player.TrainAthlete(Ability.Flexibility);
                        athleteService.TrainAthlete(player.entityName, Ability.Flexibility, trainingVolume, (int)coach); // 피로도 1 증가
                        break;
                    case TrainingType.BurpeeTest:
                        //player.TrainAthlete(Ability.Balance);
                        athleteService.TrainAthlete(player.entityName, Ability.Balance, trainingVolume, (int)coach); // 피로도 1 증가
                        break;
                }
                Debug.Log($"{player.entityName} 선수가 {assignDict[player].ToString()} 훈련을 완료했습니다.");
            }
            
            // 훈련이 끝났으니 초기화
            assignDict.Clear();
            // 시간 보내기
            flowController.ProgressWeek();
            // 패널 종료
            trainingCenter.gameObject.SetActive(false);
        }
    }
}