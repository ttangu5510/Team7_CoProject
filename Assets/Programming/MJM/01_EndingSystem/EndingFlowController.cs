using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingFlowController : MonoBehaviour
{
    [Header("Data & Calc")]
    [SerializeField] EndingScoreFormula formula;

    [Header("Screens")]
    [SerializeField] EndScorePopup scorePopup;             // 점수 팝업
    [SerializeField] EndingAnimationView animationView;    // 애니메이션(선택)
    [SerializeField] EndingCreditsPanel creditsPanel;      // 크레딧
    [SerializeField] EndingSelectPanel selectPanel;        // 버튼 선택

    //[Header("Config")]
    //[SerializeField] string titleSceneName = "Title";      // 크레딧 끝 → 타이틀
    //[SerializeField] string worldSceneName = "World";      // 이어서하기
    //[SerializeField] string startSceneName = "Start";      // 처음부터

    void Start()
    {
        HideAll();
        RunFlow();
    }

    void HideAll()
    {
        if (scorePopup) scorePopup.gameObject.SetActive(false);
        if (animationView) animationView.gameObject.SetActive(false);
        if (creditsPanel) creditsPanel.gameObject.SetActive(false);
        if (selectPanel) selectPanel.gameObject.SetActive(false);
    }

    public void RunFlow()
    {
        // 1) 데이터 준비 & 점수 계산
        var data = formula.useDummy ? formula.dummy : FetchLive();
        var bd = EndingScorer.Compute(data, formula);

        // 2) 점수 팝업
        scorePopup.gameObject.SetActive(true);
        scorePopup.Bind(data, bd);
        scorePopup.onConfirm = () =>
        {
            scorePopup.gameObject.SetActive(false);
            // 3) (선택) 애니메이션 → 바로 버튼 선택으로 가고 싶으면 이 블럭 건너뛰기
            if (animationView != null)
            {
                animationView.gameObject.SetActive(true);
                animationView.onFinishedOrSkip = () =>
                {
                    animationView.gameObject.SetActive(false);
                    OpenSelect();
                };
            }
            else
            {
                OpenSelect();
            }
        };

        // 내부 로컬 함수
        void OpenSelect()
        {
            selectPanel.gameObject.SetActive(true);
            selectPanel.SetTitle("게임 이름"); // 필요시 바인딩
            selectPanel.onCredits = () =>
            {
                selectPanel.gameObject.SetActive(false);
                OpenCredits();
            };
            selectPanel.onContinue = () => Debug.Log("계속하기 누름, 씬전환은 없어~ 가고파도~");
            //LoadScene(worldSceneName);
            selectPanel.onRestart = () => Debug.Log("첨부터하기 누름, 씬전환은 없어~ 가고파도~");
            //LoadScene(startSceneName);
        }
        void OpenCredits()
        {
            creditsPanel.gameObject.SetActive(true);
            Debug.Log("엔딩크레딧 누름~");
            // creditsPanel.onCreditFinished = () => LoadScene(titleSceneName); // ★ 자동 타이틀 이동
        }
    }

    EndingScoreData FetchLive() => new EndingScoreData(); // 실제 값 연결 지점

    void LoadScene(string scene)
    {
        // TODO: 세이브/리셋 로직 등 삽입
        SceneManager.LoadScene(scene);
    }
}
