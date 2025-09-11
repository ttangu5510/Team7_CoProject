using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Triggers;
using JYL;
using ModestTree;
using SJL;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace JYL
{
public class CoachListPanel : MonoBehaviour
{
    [Header("Set References")]
    [SerializeField] private CoachItem coachItem;
    [SerializeField] private RectTransform itemParentContent;

    [Header("Set UIs")] 
    [SerializeField] private Button closeButton;

    // 의존성 주입
    [Inject] private CoachService coachService; 
    
    // 코치 리스트
    private List<CoachEntity> recruitedCoaches = new();
    // 코치 아이템 딕셔너리
    private Dictionary<CoachItem, bool> itemDict = new();
    // 현재 배치되어 있는 코치가 있는지 확인
    private bool isAssigned;
    private int routineNumber = -1;
    private int[] assignedCoaches = new int[4];

    private void Awake()
    {
        closeButton.OnClickAsObservable()
            .Subscribe(_=>gameObject.SetActive(false))
            .AddTo(this);
    }
    // 초기화 함수
    public void Init(int routineNumber, int[] assignedCoaches)
    {
        foreach (CoachItem item in itemDict.Keys)
        {
            Destroy(item.gameObject);
        }
        itemDict.Clear();
        recruitedCoaches = coachService.GetRecruitedCoaches(); // 현재 영입된 코치들 불러오기
        this.routineNumber  = routineNumber; // 현재 훈련 배치 번호
        this.assignedCoaches = assignedCoaches; // 훈련 배치된 코치들 배열
        Debug.LogWarning($"여기에 들여온 배열 길이{assignedCoaches.Length}__들여온루틴넘버{routineNumber}");
        isAssigned = assignedCoaches[routineNumber] != -1; // 현재 배치에 id가 있다면, 배치 중인것.
        
        CreateItems();
    }

    // 코치 리스트로 아이템을 생성한다.
    private void CreateItems()
    {
        foreach (CoachEntity entity in recruitedCoaches)
        {
            // 만약, 다른 곳에 배치된 코치면 
            if (assignedCoaches.Contains(entity.id) && assignedCoaches[routineNumber] != entity.id)
            {
                continue;
            }
            CoachItem item = Instantiate(coachItem, itemParentContent);
            bool entityAssigned = assignedCoaches[routineNumber] == entity.id;
            item.Init(entity, entityAssigned);
            itemDict[item] = entityAssigned;
            item.assignButton.OnClickAsObservable()
                .TakeUntilDisable(this)
                .Subscribe(_ => OnClickAssignCoach(entity, item));
        }
    }
    

    // 코치를 배치하면 수행되는 함수.
    private void OnClickAssignCoach(CoachEntity coach, CoachItem item)
    {
        if (itemDict[item]) // 이미 배치되어 있었다면,
        {
            int index = assignedCoaches.IndexOf(coach.id);
            Debug.Log($"인덱스{index}");
            assignedCoaches[index] = -1; // "배치 중" 인 아이템은 "배치 하기"로 변경
            isAssigned = false; // 배치 판별을 false로 돌림
            item.UpdateButton(isAssigned);
            itemDict[item] = false;
            //item.Init();
        }
        else if (!isAssigned) // 배치되어 있지 않았다면
        {
            MessageBroker.Default.Publish(new AssignCoachEvent(routineNumber, coach.id));
            // 새로이 배치하고, 이벤트를 발행한 다음 패널을 닫음
            gameObject.SetActive(false);
        }
    }
}
}