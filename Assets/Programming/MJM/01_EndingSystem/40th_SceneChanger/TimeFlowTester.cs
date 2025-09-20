using SHG;
using UnityEngine;
using UnityEngine.UI;
public class TimeFlowTester : MonoBehaviour
{
    [SerializeField] private Button oneYearButton;

    private TimeFlowController controller;

    public void Initialize(TimeFlowController tf)
    {
        controller = tf;

        // 버튼 클릭 시 1년(52주) 경과
        oneYearButton.onClick.AddListener(() =>
        {
            Debug.Log("테스트 버튼 클릭됨: 1년 경과");
            controller.ProgressWeeks(ITimeFlowController.WEEK_FOR_YEAR);
        });
    }
}

