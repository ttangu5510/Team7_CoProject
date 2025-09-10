using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JYL;
using SHG;
using SJL;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SpecialTrainingPanel : MonoBehaviour
{
    [Header("Set UIs")] 
    [SerializeField] private Image[] athleteIcon;
    [SerializeField] private TextMeshProUGUI[] nameText;
    [SerializeField] private Button[] athleteIconButton;
    [SerializeField] private TextMeshProUGUI assignedText;
    [SerializeField] private Button confirmButton;
    
    [Header("Set References")]
    [SerializeField] private AthleteListPanel athleteListPui;
    [SerializeField] private SpecialTrainingCoinPUI sTCoinPui;
    [SerializeField] private SpecialTrainingResultPUI sTResultPui;
    [SerializeField] private TrainingProgressPUI tProgressPui;
    [SerializeField] private TrainingDonePUI tDonePui;
    
    [Header("Set Values")] 
    [SerializeField] private int applyAmount = 5;
    
    private Canvas trainingCenter;
    
    [Inject] private DomAthService athleteService;
    [Inject] private IResourceController resourceController;
    
    private List<DomAthEntity> athleteList;
    private Dictionary<DomAthEntity, TrainingType> assignDict = new();
    private IDisposable subscription;

    private string iconPath = "AthleteIcon";

    private int trainingTimes;
    
    
    private void Awake()
    {
        // 팝업 부모 설정
        trainingCenter = GetComponentInParent<Canvas>();
        
        // 선수 아이콘 버튼에 선수 리스트 띄우는 기능 할당
        foreach (var b in athleteIconButton)
        {
            b.OnClickAsObservable()
                .Subscribe(_=>OnClickAssignButton())
                .AddTo(this);
        }
        
        // 특훈 시작 버튼 기능 할당
        confirmButton.OnClickAsObservable()
            .Subscribe(_=>OnClickSpecialTrain())
            .AddTo(this);
        
        // 선수 등록 패널 열고 닫는 것 구독
        athleteListPui.CloseSubject
            .Subscribe(closed =>
            {
                if (closed)
                {
                    UpdateUI();
                }
            }).AddTo(this);
    }

    private void OnEnable()
    {
        EnableInit();
        
        // 선수 훈련 시 이벤트 발행되는 것을 수신. 코인 창에서 발행함
        subscription = MessageBroker.Default
            .Receive<SpecialTrainingEvent>()
            .TakeUntilDisable(this)
            .Subscribe(StartTraining);
        
    }
    
    // 패널 켜질 때마다 수행. 초기화 작업
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
            
        // UI 최신화
        UpdateUI();
            
    }

    // 선수 아이콘 클릭 시
    private void OnClickAssignButton()
    {
        athleteListPui.gameObject.SetActive(true);
        athleteListPui.SelectTrainingAthlete(athleteList, TrainingType.Special, assignDict);
    }

    // UI 최신화
    private void UpdateUI()
    {
        int count = 0;
        foreach (var pair in assignDict)
        {
            if (pair.Value == TrainingType.Special)
            {
                // TODO : 아이콘 로드
                //athleteIcon[count].sprite = Resources.Load<Sprite>($"{iconPath}{pair.Key.id}");
                nameText[count].text = pair.Key.entityName;
                count++;
            }
        }
        for(int i = count ; i < nameText.Length ; i++)
        {
            // athleteIcon[i].sprite = null;
            nameText[i].text = "";
        }

        assignedText.text = $"특훈 진행 선수 : {assignDict.Values.Count(t => t == TrainingType.Special)} / 4";
    }

    // 패널에서 선수 배치완료 후, "특훈 시작"을 눌렀을 때 수행
    private void OnClickSpecialTrain()
    {
        int assignedNumber = assignDict.Values.Count(t => t == TrainingType.Special);
        Debug.Log($"현재 배정된 선수의 숫자{assignedNumber}");
        if ( assignedNumber == 0) return; // 아무도 배치되어 있지 않다면 return
        
        SpecialTrainingCoinPUI pui = Instantiate(sTCoinPui, trainingCenter.transform);
        pui.gameObject.SetActive(true);
        pui.Init(assignedNumber, resourceController.Coin.Value); // 현재 배치되어 있는 인원, 가지고 있는 코인을 가지고 팝업을 초기화 함.
    }

    // 트레이닝 시작 함수. SpecialTrainingCoinPUI 팝업 내부의 이벤트 발행에 의해 수행됨. 
    private void StartTraining(SpecialTrainingEvent tEvent) // 1. 트레이닝 결과를 적용함. 2. UI의 진행 %가 전부 차면, 자동으로 ProgressDonePopUp으로 넘어감. 
    {
        int assignedCount = assignDict.Values.Count(t=>t==TrainingType.Special);
        
        if (tEvent.startTraining) // 트레이닝 시작 시
        {
            trainingTimes = tEvent.trainingStage;
            foreach (var pair in assignDict)
            {
                if (pair.Value == TrainingType.Special)
                {
                    athleteService.ApplySpecialTraining(pair.Key, trainingTimes, applyAmount); // 각 선수들을 특훈 수행 적용.
                }
            }

            int spendCoin = assignedCount * trainingTimes;
            resourceController.SpendCoin(spendCoin);
            Debug.Log($"이만큼 코인 소비함{spendCoin}__ 남은 코인{resourceController.Coin.Value}__배정된 선수 숫자{assignedCount}__훈련 횟수{trainingTimes}");
        
            // 특훈 수행 로직을 먼저 처리한 다음, 특훈 수행 프로그레스 표현 UI 팝업
            TrainingProgressPUI pui = Instantiate(tProgressPui, trainingCenter.transform);
            pui.gameObject.SetActive(true);
            _ = pui.Init();
            pui.Confirmed.Subscribe(done =>
            {
                if (done) ProgressDonePopUp();
            });
        }
    }

    // 특훈 수행 프로그레스 바가 꽉 차면 수행. "훈련 완료" 팝업.
    private void ProgressDonePopUp()
    {
        TrainingDonePUI pui = Instantiate(tDonePui, trainingCenter.transform);
        pui.gameObject.SetActive(true);
        pui.Init(true);
        pui.ConfirmSubject.Subscribe(confirm =>
        {
            if (confirm) ShowConfirmPopUp();
        });
    }

    // "훈련 완료" 팝업을 확인 클릭 시 수행. 특훈 결과를 보여준다.
    private void ShowConfirmPopUp()
    {
        SpecialTrainingResultPUI pui = Instantiate(sTResultPui, trainingCenter.transform);
        pui.gameObject.SetActive(true);
        pui.SetParameters(trainingTimes, assignDict); // 특훈 수행 횟수만큼 결과에 반영.
        pui.ConfirmSubject
            .Subscribe(confirm =>
            {
                if(confirm) trainingCenter.gameObject.SetActive(false);
            });
    }
}
