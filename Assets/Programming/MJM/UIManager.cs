using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BasePanelType // 안전장치용 열거형
{
    Info,
    Character,
    QuestAchievement,
    Competition,
    CompetitionSchedule
}

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }


    [Header("하단 UI(Header for SHG = Bottom UI)")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject questAchievementPanel;
    [SerializeField] private GameObject competitionPanel;
    [SerializeField] private GameObject competitionSchedulePanel;

    // 팝업시 다른 곳 상호작용을 막는 용도
    [Header("팝업 (Header for SHG = Popup)")]
    [SerializeField] private GameObject popupBlocker;

    // 베이스 패널 관리용
    private Dictionary<BasePanelType, GameObject> basePanels;
    private BasePanelType? currentBasePanel; // 현재 열린 베이스 패널 (현재 열린 패널이 하나도 없을 수 있으니 null 값을 가지기 위해 ? 를 붙여 나타냄)

    // 팝업 관리용 (겹칠 수 있으므로 스택으로 관리)
    private Stack<GameObject> popupStack = new Stack<GameObject>();




    private void Awake()
    {
        // 싱글 톤
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);

        // 베이스 패널 맵 구성
        basePanels = new Dictionary<BasePanelType, GameObject>
        {
            { BasePanelType.Info, infoPanel },
            { BasePanelType.Character, characterPanel },
            { BasePanelType.QuestAchievement, questAchievementPanel },
            { BasePanelType.Competition, competitionPanel },
            { BasePanelType.CompetitionSchedule, competitionSchedulePanel },
        };

        // 시작 시 모두 끄기
        foreach (var kv in basePanels) kv.Value?.SetActive(false);
        popupBlocker?.SetActive(false);
    }

    private void Update()
    {
        // 안드로이드 뒤로가기 대응
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleBack();
    }




    // 오픈 베이스 패널
    public void OpenBasePanel(BasePanelType type)
    {
        // 이미 같은 패널이면 토글로 닫기 (원하면 유지하도록 바꿔도 됨)
        if (currentBasePanel.HasValue && currentBasePanel.Value.Equals(type))
        {
            CloseAllBasePanels();
            return;
        }

        // 하나만 켜고 나머지 끄기
        foreach (var kv in basePanels)
            kv.Value?.SetActive(kv.Key == type);

        currentBasePanel = type;

        // 베이스 패널은 블로커 불필요 (탭 전환만 하니까)
        if (popupStack.Count == 0)
            popupBlocker?.SetActive(false);

        // 입력/카메라 잠금이 필요하면 여기서 이벤트 쏘기
        // e.g., PlayerInputBus.RaiseUIOpened();
    }

    // 모든 패널 닫기
    public void CloseAllBasePanels()
    {
        foreach (var kv in basePanels) kv.Value?.SetActive(false);
        currentBasePanel = null;
        if (popupStack.Count == 0)
            popupBlocker?.SetActive(false);

        // e.g., PlayerInputBus.RaiseUIClosed();
    }

    // 버튼에서 직접 연결하고 싶으면 이 래퍼 써도 됨
    public void OnClick_OpenInfo() => OpenBasePanel(BasePanelType.Info);
    public void OnClick_OpenCharacter() => OpenBasePanel(BasePanelType.Character);
    public void OnClick_OpenQuestAchievement() => OpenBasePanel(BasePanelType.QuestAchievement);
    public void OnClick_OpenCompetition() => OpenBasePanel(BasePanelType.Competition);
    public void OnClick_OpenCompetitionSchedule() => OpenBasePanel(BasePanelType.CompetitionSchedule);



    public void ShowPopup(GameObject popup)
    {
        // 전달된 팝업 없으면 그냥 종료
        if (popup == null) return;

        // 전달받은 팝업 게임 오브젝트 활성화
        popup.SetActive(true);

        // 스택에 쌓기 -> 마지막으로 연 팝업부터 닫기 위해
        popupStack.Push(popup);
        
        // 블로커 활성화 -> 팝업 외 다른 UI 영역 선택 방지
        popupBlocker?.SetActive(true);
    }

    public void CloseTopPopup()
    {
        // 팝업 없으면 종료
        if (popupStack.Count == 0) return;

        // 스택에서 꺼내기
        var top = popupStack.Pop();

        // 팝업 비활성화
        if (top) top.SetActive(false);

        // 팝업이 남아있으면 블로커 유지 / 없으면 블로커 비활성화
        popupBlocker?.SetActive(popupStack.Count > 0);
    }


    // 특정 팝업을 지정해서 삭제 할 수 있는 함수
    public void CloseSpecificPopup(GameObject popup)
    {
        // 팝업이 없거나 스택이 비어있으면 그냥 종료
        if (popup == null || popupStack.Count == 0) return;
        
        var temp = new Stack<GameObject>();
        bool closed = false;


        // popupStack을 하나씩 꺼내면서 원하는 팝업을 찾음
        while (popupStack.Count > 0)
        {
            var p = popupStack.Pop();
            if (!closed && p == popup)          // 찾은 경우
            {
                if (p) p.SetActive(false);      // 화면에서 비활성화
                closed = true;                  // 닫았다는 표시
                continue;                       // 스택에 추가 X
            }
            temp.Push(p);                       // 찾는 게 아니면 임시 스택에 보관
        }

        // temp에 있던 나머지를 다시 원래 popupStack에 되돌림
        while (temp.Count > 0) popupStack.Push(temp.Pop());

        // 스택에 팝업이 남아있으면 블로커 유지, 아니면 끔
        popupBlocker?.SetActive(popupStack.Count > 0);
    }


    public void ShowMessage(string text)
    {
        // 토스트/알림 팝업 열기 → ShowPopup(알림팝업 프리팹) 사용
        Debug.Log($"[Toast] {text}");
    }

    
    private void HandleBack()
    {
        if (popupStack.Count > 0) { CloseTopPopup(); return; }
        if (currentBasePanel.HasValue) { CloseAllBasePanels(); return; }
        // 더 이상 닫을 UI가 없다면: 게임 일시정지/종료 확인 팝업 등을 띄울지 결정
        // ShowPopup(pausePopup);
    }

}
