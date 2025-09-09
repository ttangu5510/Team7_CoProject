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
    
    [Header("Set Parent")]
    [SerializeField] private Canvas trainingCenter;

    [Inject] private DomAthService athleteService;
    [Inject] private IResourceController resourceController;
    
    private List<DomAthEntity> athleteList;
    private Dictionary<DomAthEntity, TrainingType> assignDict = new();
    private IDisposable subscription;

    private string iconPath = "AthleteIcon";

    private int trainingTimes;
    
    private void Awake()
    {
        trainingCenter = GetComponentInParent<Canvas>();
        foreach (var b in athleteIconButton)
        {
            b.OnClickAsObservable()
                .Subscribe(_=>OnClickAssignButton())
                .AddTo(this);
        }
        
        confirmButton.OnClickAsObservable()
            .Subscribe(_=>OnClickSpecialTrain())
            .AddTo(this);
    }

    private void OnEnable()
    {
        EnableInit();
        
        subscription = MessageBroker.Default
            .Receive<SpecialTrainingEvent>()
            .TakeUntilDisable(this)
            .Subscribe(StartTraining);
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
            
        // UI 최신화
        UpdateUI();
            
    }

    private void OnClickAssignButton()
    {
        athleteListPui.gameObject.SetActive(true);
        athleteListPui.SelectTrainingAthlete(athleteList, TrainingType.Special, assignDict);
    }

    private void UpdateUI()
    {
        int count = 0;
        foreach (var pair in assignDict)
        {
            // TODO : 아이콘 로드
            //athleteIcon[count].sprite = Resources.Load<Sprite>($"{iconPath}{pair.Key.id}");
            nameText[count].text = pair.Key.entityName;
            count++;
        }
        for(int i = count ; i < nameText.Length ; i++)
        {
            // athleteIcon[i].sprite = null;
            nameText[i].text = "";
        }
    }

    // 패널에서 선수 배치완료 후, "특훈 시작"을 눌렀을 때 수행
    private void OnClickSpecialTrain()
    {
        int assignedNumber = assignDict.Values.Count(t => t == TrainingType.Special);
        
        if ( assignedNumber == 0) return; // 아무도 배치되어 있지 않다면 return
        
        SpecialTrainingCoinPUI pui = Instantiate(sTCoinPui, trainingCenter.transform);
        pui.Init(assignedNumber, resourceController.Coin.Value); // 현재 배치되어 있는 인원, 가지고 있는 코인을 가지고 팝업을 초기화 함.
    }

    private void StartTraining(SpecialTrainingEvent tEvent) // 트레이닝 시작 함수. 팝업 내부의 이벤트로 수행됨.
                                                            // 1. 트레이닝 결과를 적용함. 2. UI의 진행 %가 전부 차면, 자동으로 ProgressDonePopUp으로 넘어감. 
    {
        if (tEvent.startTraining)
        {
            trainingTimes = tEvent.trainingStage;
            foreach (var pair in assignDict)
            {
                if (pair.Value == TrainingType.Special)
                {
                    athleteService.ApplySpecialTraining(pair.Key, trainingTimes);
                }
            }
        
            TrainingProgressPUI pui = Instantiate(tProgressPui, trainingCenter.transform);
            _ = pui.Init();
            pui.Confirmed.Subscribe(done =>
            {
                if (done) ProgressDonePopUp();
            });
        }
    }

    private void ProgressDonePopUp()
    {
        TrainingDonePUI pui = Instantiate(tDonePui, trainingCenter.transform);
        pui.Init(true);
        pui.ConfirmSubject.Subscribe(confirm =>
        {
            if (confirm) ShowConfirmPopUp();
        });
    }

    private void ShowConfirmPopUp()
    {
        SpecialTrainingResultPUI pui = Instantiate(sTResultPui, trainingCenter.transform);
        pui.SetParameters(trainingTimes,assignDict);
        pui.ConfirmSubject
            .Subscribe(confirm =>
            {
                if(confirm) trainingCenter.gameObject.SetActive(false);
            });
    }
}
