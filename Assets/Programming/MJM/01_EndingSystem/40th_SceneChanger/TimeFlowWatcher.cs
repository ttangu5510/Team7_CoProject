using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using SHG;


public class TimeFlowWatcher : MonoBehaviour
{
    private TimeFlowController controller;

    // 외부에서 연결해주는 초기화 메서드
    public void Initialize(TimeFlowController tf)
    {
        controller = tf;

        // 연도 변화 감시 → 경과 년수 체크
        controller.Year
            .Select(year => year - controller.Start.year + 1) // = YearPassedAfterStart 계산
            .DistinctUntilChanged() // 중복 제거
            .Where(passed => passed >= 40)
            .Take(1) // 한 번만 실행
            .Subscribe(_ =>
            {
                Debug.Log("게임 내 40년 경과! 씬 전환 실행");
                SceneManager.LoadSceneAsync("JYL_EndingScene"); // 씬 이름에 맞게 수정하세요
            })
            .AddTo(this); // GameObject 파괴 시 자동 해제
    }
}
