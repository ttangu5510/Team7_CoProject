using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.ParticleSystem;

namespace SJL
{
    public class Tutorial : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI tutorialText;  // 대사 출력 텍스트
        [SerializeField] private GameObject[] highlightObjects; // 하이라이트 대상 UI들
        [SerializeField] private Button clickAreaButton;        // 클릭 영역(이미지 혹은 투명 버튼)
        [SerializeField] private Image nextImage;               // 다음 대사로 넘어가는 이미지(화살표 등)

        public float lineDelay = 2f; // 대사 한줄 출력 간격

        private List<string[]> tutorialSteps;   // 모든 튜토리얼 단계 배열 리스트
        private List<int[]> highlightStepIdxs;  // 각 단계별 하이라이트 대상 인덱스 배열 리스트
        private int currentStepIdx = 0; // 현재 튜토리얼 단계 인덱스
        private Coroutine currentRoutine;   // 현재 실행중인 코루틴 참조
        private bool waitForClick = false; // 대사 끝나고 클릭 대기 상태

        private void Awake()
        {
            clickAreaButton.onClick.AddListener(OnClickAreaClick);  // 클릭 이벤트 연결
        }

        private void Start()
        {
            // 모든 튜토리얼 단계 배열을 리스트로 등록
            tutorialSteps = new List<string[]>
            {
                tutorialGreetings,
                tutorialMainScreen,
                tutorialfacilities,
                tutorialStartOfTheFirstGame,
                tutorialEndOfTheFirstGame,
                tutorialFinalCompetition,
                tutorialSecondYearStartAnnouncement,
                // 필요하면 더 추가
            };

            // 문장별로 켜고 싶은 highlightObjects 인덱스를 지정 (-1은 모두 비활성)
            highlightStepIdxs = new List<int[]>
            {
                new int[] { 0, 1, 2, 3, 4 }, // tutorialGreetings
                new int[] { 0, -1, 1, -1, -1, -1, -1, 2, -1, -1, 3, 4, 5, 6, 7 }, // tutorialMainScreen
                new int[] { 8, 9, 10, 11, 12, 13 }, // tutorialfacilities
                new int[] { -1, -1, -1, 14 }, // tutorialStartOfTheFirstGame
                new int[] { -1, -1, -1, -1 }, // tutorialEndOfTheFirstGame
                new int[] { -1, -1, -1, 15 }, // tutorialFinalCompetition
                new int[] { -1, -1, -1, -1, -1, -1 }, // tutorialSecondYearStartAnnouncement
            };

            // 시작시 튜토리얼 표시
            ShowStep(currentStepIdx);
        }

        public void ShowStep(int stepIdx)   // 특정 단계의 튜토리얼 표시
        {
            if (currentRoutine != null) // 기존 코루틴이 실행중이면 중지
                StopCoroutine(currentRoutine);  // 기존 코루틴 중지
            waitForClick = false;   // 클릭 대기 상태 해제
            nextImage.gameObject.SetActive(false); // 다음 화살표는 라인 출력 중엔 숨김
            currentRoutine = StartCoroutine(RevealLines(tutorialSteps[stepIdx], highlightStepIdxs[stepIdx]));   // 해당 단계 코루틴 시작
        }

        private IEnumerator RevealLines(string[] lines, int[] highlightIndices)    // 각 줄을 순차적으로 표시
        {
            //tutorialText.text = ""; // 텍스트 초기화
            //for (int i = 0; i < lines.Length; i++)  // 각 줄 표시
            //{
            //    tutorialText.text += lines[i] + "\n";   // 줄 추가
            //    HighlightUIObject(i);   // 해당 줄에 맞는 UI 하이라이트
            //    yield return new WaitForSeconds(lineDelay); // 지연
            //}

            List<string> shownLines = new List<string>();   // 현재 표시된 줄들
            tutorialText.text = ""; // 텍스트 초기화

            for (int i = 0; i < lines.Length; i++)  // 각 줄 표시
            {
                // 새 줄 추가
                shownLines.Add(lines[i]);
                // 최대 4줄로 제한
                if (shownLines.Count > 1)
                    shownLines.RemoveAt(0);
                // 텍스트 UI에 표시
                tutorialText.text = string.Join("\n", shownLines);
                // 해당 줄에 맞는 UI 하이라이트
                HighlightUI(highlightIndices[i]);
                yield return new WaitForSeconds(lineDelay); // 지연
            }
            waitForClick = true; // 대사 모두 표시 후 다음 진입 대기 모드
            nextImage.gameObject.SetActive(true); // 다 끝나면 클릭 안내 화살표 보이게

            // 현 배열이 끝나면 다음 배열 단계로 자동 진행
            //currentStepIdx++;   // 다음 단계로 인덱스 증가
            //if (currentStepIdx < tutorialSteps.Count)   // 다음 단계가 있으면
            //{
            //    ShowDialogueStep(currentStepIdx);   // 다음 단계 표시
            //}
            //else
            //{
            //    // 튜토리얼 종료 처리(팝업 닫기, 다음 씬 이동 등)
            //}
        }

        // 모든 UI는 항상 활성화하고, 강조 idx만 노란 테두리
        private void HighlightUI(int highlightIdx)
        {
            for (int i = 0; i < highlightObjects.Length; i++)
            {
                highlightObjects[i].SetActive(true); // 항상 보임

                var outline = highlightObjects[i].GetComponent<Outline>();
                if (outline != null)
                {
                    outline.effectColor = Color.yellow; // 테두리 색은 노란색
                    outline.enabled = (i == highlightIdx); // 강조할 idx만 켬
                }
            }
        }

        // 텍스트 UI가 클릭되면 실행됨
        public void OnClickAreaClick()  // 클릭 영역 클릭 이벤트
        {
            if (!waitForClick)  // 대기 상태가 아니면 무시
                return;

            nextImage.gameObject.SetActive(false); // 클릭하면 다시 숨김
            currentStepIdx++;   // 다음 단계로 인덱스 증가

            if (currentStepIdx < tutorialSteps.Count)   // 다음 단계가 있으면
            {
                ShowStep(currentStepIdx);   // 다음 단계 표시
            }
            else
            {
                // 종료 시 모두 활성화, 강조 없음
                foreach (var obj in highlightObjects)
                {
                    obj.SetActive(true);
                    var outline = obj.GetComponent<Outline>();
                    if (outline != null)
                        outline.enabled = false;
                }
                gameObject.SetActive(false);
                Debug.Log("튜토리얼 종료");
            }
        }


        // 각 단계별 튜토리얼 텍스트 배열
        private string[] tutorialGreetings = new string[]
        {
            "안녕하세요! 000 단장님!",
            "저는 선수단 매니저를 맡고 있는 '이유리'라고 합니다.",
            "잘 부탁드려요!",
            "본격적인 단장 업무를 수행하시기 전에 간략한 정보를 알려드려도 괜찮을까요?",
            "(튜토리얼 시작 또는 스킵 팝업창 등장)",
        };
        private string[] tutorialMainScreen = new string[]
        {
            "여기서는 훈련이나 경기를 진행할 때마다 시간이 지나갑니다.",
            "계절마다 10주씩, 1년에 총 40주이니 유념해주세요.",
            "여기에는 재화가 표시됩니다.",
            "왼쪽부터 명성, 특훈 코인, 골드입니다.",
            "명성은 시설 업그레이드와 코치 영입과 관련된 재화입니다.",
            "특훈 코인은 특정 선수의 강도 높은 훈련을 위한 재화입니다.",
            "골드는 시설 업그레이드와 선수 영입 등에 사용되는 재화입니다.",
            "여기에는 경기 일정이 표시됩니다.",
            "1년동안 펼쳐질 경기 일정들이 보여지는 공간입니다.",
            "드래그하여 일정을 확인하고, 클릭하면 대회 참가 신청 화면이 등장합니다.",
            "여기는 정보, 캐릭터, 도감, 퀘스트/업적을 확인할 수 있습니다.",
            "정보는 선수단의 전반적인 정보를 확인할 수 있습니다.",
            "캐릭터는 보유 선수와 보유 코치를 확인할 수 있습니다.",
            "도감에서는 획득 또는 미획득한 메달, 트로피, 선수들을 확인할 수 있습니다.",
            "퀘스트/업적에서는 퀘스트, 업적의 진행 상황과 달성 여부를 확인할 수 있습니다.",
        };
        private string[] tutorialfacilities = new string[]
        {
            "선수단 시설은 숙소, 휴게실, 훈련 센터, 의료 센터, 스카우트 센터가 있습니다.",
            "숙소는 선수단 수용 인원을 관리합니다.",
            "휴게실은 선수들의 피로도를 줄여주는 시설입니다.",
            "훈련 센터는 선수를 훈련시키고 육성하는 시설입니다.",
            "의료 센터는 부상 당한 선수를 회복시키는 시설입니다.",
            "스카우트 센터는 선수와 코치를 영입할 수 있는 시설입니다.",
        };
        private string[] tutorialStartOfTheFirstGame = new string[]
        {
            "00 단장님! 벌써 첫 경기가 있는 날이네요!",
            "아직 부임하신지 얼마되지 않으셨으니 결과에 연연하지 않는게 좋겠습니다!",
            "다 같이 선수들을 응원해 보도록 하죠!",
            "(경기 시작 알림 UI 등장 및 하이라이트)",
        };
        private string[] tutorialEndOfTheFirstGame = new string[]
        {
            "(첫번째 경기 종료 이후)",
            "첫 경기만에 이런 결과를!",
            "대단하십니다! 단장님과 선수들 모두 수고하셨어요.",
            "다음 경기도 열심히 준비해보죠!",
        };
        private string[] tutorialFinalCompetition = new string[]
        {
            "00단장님! 드디어 지난 1년간 선수들의 노력이 결실을 맺는 날입니다!",
            "이번 대회에서는 모든 종목에 도전하게 되니 주의해 주세요.",
            "선수들의 활약을 지켜보며 열심히 응원해보죠!",
            "(대회 입장 알림 UI 등장)",
        };
        private string[] tutorialSecondYearStartAnnouncement = new string[]
        {
            "(1회차 2년차 시작)",
            "벌써 부임하신지 1년이 지났군요?",
            "00 단장님 덕분에 선수단이 어느 정도 자리를 잡았습니다. 선수단을 대신해 감사드립니다!",
            "이번 대회부터 선수단은 모든 종목을 겨루는 대회에만 참가하게 됩니다.",
            "이대로 '국제 동계 스포츠 대회' 우승을 목표로 열심히 달려보죠!",
            "앞으로도 잘 부탁드려요!",
        };

    }
}