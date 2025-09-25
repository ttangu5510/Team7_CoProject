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
        None, SpeedSkating, FigureSkating, Skeleton, SkiJump, Special
    }
    
    public class TrainingBox : MonoBehaviour
    {
        // 부모 칸바스
        private Canvas trainingCenter;
        
        [Header("Button")] 
        [SerializeField] private Button speedSkatingButton;
        [SerializeField] private Button figureSkatingButton;
        [SerializeField] private Button skeletonButton;
        [SerializeField] private Button skiJumpButton;

        [SerializeField] private Button trainingCloseButton;
        [SerializeField] private Button startTrainingButton;
        [SerializeField] private Button resetButton;
        
        // [Header("Text")] 
        // [SerializeField] private TextMeshProUGUI circuitTrainingText;
        // [SerializeField] private TextMeshProUGUI ladderDrillTrainingText;
        // [SerializeField] private TextMeshProUGUI sprintsText;
        // [SerializeField] private TextMeshProUGUI burpeeTestsText;
        
        [Header("Assigned AthleteImage")] // 편성된 선수들의 이미지 및 이름 관리
        [SerializeField] private AssignedAthPanel speedSkatingImage;
        [SerializeField] private AssignedAthPanel figureSkatingImage;
        [SerializeField] private AssignedAthPanel skeletonImage;
        [SerializeField] private AssignedAthPanel skiJumpImage;
        
        [Header("Set Refs")]
        [SerializeField] private AthleteListPanel assignmentPanel;
        [SerializeField] private ConfirmPUI confirmPui;
        [SerializeField] private TrainingProgressPUI progressPui;
        [SerializeField] private TrainingDonePUI donePui;
        [SerializeField] private FacilityPresenter facilityPresenter;

        
        // 플레이어 서비스 의존성 주입
        [Inject] private DomAthService athleteService;
        // 코치 서비스 의존성 주입
        [Inject] private CoachService coachService;
        // 시간 컨트롤러 의존성 주입
        [Inject] private ITimeFlowController flowController;
        // 시설의 업그레이드 정도 적용
        [Inject] private IFacilitiesController facilitiesController;
        // 업적 적용
        [Inject] private AchievementManager achievementManager;

        public List<DomAthEntity> athleteList = new();
        private Dictionary<DomAthEntity,TrainingType> assignDict = new();

        private int trainAmount = 1;
        
        private TrainingType cachedType; // 이벤트 연결을 위해 들고 있음.
        
        // 팝업 생성 시 사용되는 텍스트
        private string[] puiString;

        private void Awake()
        {
            trainingCenter = GetComponentInParent<Canvas>();
            
            assignDict.Clear();
            
            EnableInit();
            
            speedSkatingButton.OnClickAsObservable()
                .Subscribe(_ => AssignAthletes(TrainingType.SpeedSkating))
                .AddTo(this);

            figureSkatingButton.OnClickAsObservable()
                .Subscribe(_ => AssignAthletes(TrainingType.FigureSkating))
                .AddTo(this);

            skeletonButton.OnClickAsObservable()
                .Subscribe(_ => AssignAthletes(TrainingType.Skeleton))
                .AddTo(this);

            skiJumpButton.OnClickAsObservable()
                .Subscribe(_ => AssignAthletes(TrainingType.SkiJump))
                .AddTo(this);

            resetButton.OnClickAsObservable()
                .Subscribe(_ => EnableInit()).AddTo(this);

            startTrainingButton.OnClickAsObservable()
                .Subscribe(_ => CheckAllAssign()).AddTo(this);
            
            //trainingCloseButton.OnClickAsObservable()
                //.Subscribe(_ => UpdateAllAssignment()).AddTo(this);
           
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
            
            // 시설의 업그레이드 정도 최신화
            trainAmount = facilitiesController.TrainingCenter.BonusStat.Value;
        }

        // 선수 배치. 선수 선택 UI 팝업 띄움
        private void AssignAthletes(TrainingType type)
        {
            assignmentPanel.gameObject.SetActive(true);
            assignmentPanel.SelectTrainingAthlete(athleteList, type, assignDict);
            cachedType = type;
        }

        // 각 종목의 배치 현황 UI 업데이트
        private void UpdateAssignment(TrainingType type)
        {
            switch (type)
            {
                case TrainingType.SpeedSkating:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        speedSkatingImage.gameObject.SetActive(true);
                        speedSkatingImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        speedSkatingImage.gameObject.SetActive(false);
                    }
                    break;
                case TrainingType.FigureSkating:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        figureSkatingImage.gameObject.SetActive(true);
                        figureSkatingImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        figureSkatingImage.gameObject.SetActive(false);
                    }
                    break;
                case TrainingType.Skeleton:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        skeletonImage.gameObject.SetActive(true);
                        skeletonImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        skeletonImage.gameObject.SetActive(false);
                    }
                    break;
                case TrainingType.SkiJump:
                    if (assignDict.Values.Count(t => t == type) > 0)
                    {
                        skiJumpImage.gameObject.SetActive(true);
                        skiJumpImage.UpdateUI(assignDict, type);
                    }
                    else
                    {
                        skiJumpImage.gameObject.SetActive(false);
                    }
                    break;
            }
        }

        // 전 종목의 배치 현황 텍스트 업데이트
        private void UpdateAllAssignment()
        {
            int assignedCircuitCount = assignDict.Values.Count(t => t == TrainingType.SpeedSkating);
            if (assignedCircuitCount > 0)
            { 
                speedSkatingImage.gameObject.SetActive(true);
                speedSkatingImage.UpdateUI(assignDict, TrainingType.SpeedSkating);
            }
            else
            {
                speedSkatingImage.gameObject.SetActive(false);
            }
            int assignedLadderCount = assignDict.Values.Count(t => t == TrainingType.FigureSkating);
            if (assignedLadderCount > 0)
            {
                figureSkatingImage.gameObject.SetActive(true);
                figureSkatingImage.UpdateUI(assignDict, TrainingType.FigureSkating);
            }
            else
            {
                figureSkatingImage.gameObject.SetActive(false);
            }
            int assignedSprintsCount = assignDict.Values.Count(t => t == TrainingType.Skeleton);
            if (assignedSprintsCount > 0)
            {
                skeletonImage.gameObject.SetActive(true);
                skeletonImage.UpdateUI(assignDict, TrainingType.Skeleton);
            }
            else
            {
                skeletonImage.gameObject.SetActive(false);
            }
            int assignedBurpeeCount = assignDict.Values.Count(t => t == TrainingType.SkiJump);
            if (assignedBurpeeCount > 0)
            {
                skiJumpImage.gameObject.SetActive(true);
                skiJumpImage.UpdateUI(assignDict, TrainingType.SkiJump);
            }
            else
            {
                skiJumpImage.gameObject.SetActive(false);
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
            bool success = true;
            bool result = true;
            
            // 코치 정보 가져오기
            int[] coaches = coachService.GetAssignedCoachesArray();
            foreach (var entity in assignDict.Keys)
            {
                // 훈련별 능력치 및 피로 상승
                switch (assignDict[entity])
                {
                    case TrainingType.SpeedSkating: // 순발력, 기술
                        success = athleteService.TrainAthlete(entity, TrainingType.SpeedSkating, trainAmount,coaches[0]);
                        break;
                    case TrainingType.FigureSkating: // 기술, 체력
                        success = athleteService.TrainAthlete(entity, TrainingType.FigureSkating, trainAmount, coaches[1]);
                        break;
                    case TrainingType.Skeleton: // 유연성, 체력
                        success = athleteService.TrainAthlete(entity, TrainingType.Skeleton, trainAmount, coaches[2]);
                        break;
                    case TrainingType.SkiJump: // 균형감각, 속도
                        success = athleteService.TrainAthlete(entity, TrainingType.SkiJump, trainAmount, coaches[3]);
                        break;
                }

                if (!success)
                {
                    result = false;
                    Debug.Log($"훈련 실패{entity.entityName}_{assignDict[entity]}");
                }
                else
                {
                    Debug.Log($"{entity.entityName} 선수가 {assignDict[entity].ToString()} 훈련을 완료했습니다.");
                }
            }
            
            // 훈련이 끝났으니 초기화
            assignDict.Clear();
            
            // 훈련 진행 팝업 표시
            TrainingProgressPUI progPui = Instantiate(progressPui, trainingCenter.transform);
            progPui.gameObject.SetActive(true);
            
            _ = progPui.Init();
            progPui.Confirmed.Subscribe(progress =>
            {
                if (progress) OnTrainingDone(result);
            });
            

        }
        
        // 이벤트. 훈련 결과
        private void OnTrainingDone(bool success)
        {
            TrainingDonePUI pui = Instantiate(donePui, trainingCenter.transform);
            pui.gameObject.SetActive(true);
            pui.Init(success);
            pui.ConfirmSubject.Subscribe(clicked =>
            {
                if (clicked) OnPopUpOkClick();
            });
        }
        
        // 이벤트. 훈련 후, 시간 보내기
        private void OnPopUpOkClick()
        {
            // 시간 보내기
            flowController.ProgressWeek();
            facilityPresenter.Hide();
            achievementManager.wrapper.TrainCount.Value++; // 업적 카운트 적용
        }
    }
}