using System;
using JYL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace SJL
{
    public class CoachBox : MonoBehaviour
    {
        [Header("Set Slots")]
        [SerializeField] private Button[] routineButton;
        [SerializeField] private Image[] coachImage;
        [SerializeField] private TextMeshProUGUI[] coachName;
        [SerializeField] private TextMeshProUGUI[] coachFatigue;

        [Header("Set References")] 
        [SerializeField] private CoachListPanel coachListPanel;
        
        // 의존성 주입
        [Inject] private ISaveManager saveManager;
        [Inject] private CoachService coachService;

        // 코치 배치 상황을 보여주는 배열. 세이브 객체에서 복사해옴.
        private int[] assignedCoaches;
        
        // 이벤트 구독
        private IDisposable subscription;

        // 배치 배열을 index를 키로, 코치 객체를 값으로 딕셔너리화.
        private Dictionary<int, CoachEntity> assignedCoachDict = new();

        private void Awake()
        {
            for (int i = 0; i< routineButton.Length ; i++)
            {
                var i1 = i;
                routineButton[i].OnClickAsObservable()
                    .Subscribe(_=>OnClickOpenList(i1))
                    .AddTo(this);
            }
        }
        
        private void OnEnable()
        {
            // 현재 코치 배치 상황 업데이트
            assignedCoaches = saveManager.GetAssignedCoaches();
            assignedCoachDict.Clear();

            for (int i = 0; i < assignedCoaches.Length; i++)
            {
                if (assignedCoaches[i] > 0)
                {
                    assignedCoachDict[i] = coachService.FindCoachById(assignedCoaches[i]); // id로 객체 찾은다음 딕셔너리에 저장.
                }
            }
            UpdateUI();

            subscription = MessageBroker.Default
                .Receive<AssignCoachEvent>()
                .TakeUntilDisable(this)
                .Subscribe(OnSetCoaches);
        }

        private void UpdateUI() // 버튼의 UI들을 업데이트
        {
            for (int i = 0; i < coachImage.Length; i++)
            {
                if (assignedCoachDict.TryGetValue(i, out CoachEntity entity))
                {
                    // coachImage[i].sprite = entity.coachIcon;
                    coachName[i].text = entity.entityName;
                    coachFatigue[i].text = $"-{(int)entity.grade}";

                }
                else
                {
                    // coachImage[i].sprite = null;
                    coachName[i].text = "배치된 코치가 없습니다.";
                    coachFatigue[i].text = "";
                }
            }
        }

        private void OnClickOpenList(int routineNumber) // 코치 배치를 위한 리스트 UI 팝업 수행
        {
            Debug.Log($"리스트 팝업 루틴{routineNumber}__{assignedCoaches[0]}__{assignedCoaches[1]}__{assignedCoaches[2]}__{assignedCoaches[3]}");
            coachListPanel.gameObject.SetActive(true);
            coachListPanel.Init(routineNumber, assignedCoaches);
            //coachListPanel.Init();

        }

        private void OnSetCoaches(AssignCoachEvent assignEvent) // 코치 배치가 되면, 이벤트 수행
        {
            if (assignEvent.CoachId != -1) // 코치 배치를 새로 했다면
            {
                assignedCoachDict[assignEvent.SlotNumber] = coachService.FindCoachById(assignEvent.CoachId); // 딕셔너리 최신화
            }
            else // 코치 배치를 해제했다면
            {
                assignedCoachDict.Remove(assignEvent.SlotNumber); // 딕셔너리에서 삭제
            }
            assignedCoaches[assignEvent.SlotNumber] = assignEvent.CoachId; // 코치 배치 배열 최신화
            saveManager.SetAssignedCoaches(assignedCoaches); // 세이브 객체의 코치 배치 배열 또한 최신화
            UpdateUI(); // UI 최신화
            
            //TODO:Test 코치영입,배치
            coachService.RefreshCoaches();
        }

    }
}
