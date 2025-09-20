using JYL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CoachInfoPanelMMJ : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button fireButton;   // 계약해지 버튼
    [SerializeField] private CoachFirePanelMMJ firePanel;

    [Header("UI Fields")]
    [SerializeField] private Image coachIcon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI retireText;
    [SerializeField] private TextMeshProUGUI fatigueText;
    [SerializeField] private TextMeshProUGUI routineText;

    public event System.Action<CoachEntity> OnFired;

    private CoachEntity current;

    [Inject] private CoachService coachService;
    [Inject] private SaveManager saveManager;

    private void Awake()
    {
        gameObject.SetActive(false);

        closeButton.OnClickAsObservable()
            .Subscribe(_ => gameObject.SetActive(false))
            .AddTo(this);

        fireButton.OnClickAsObservable()
            .Subscribe(_ => FireCoach())
            .AddTo(this);
    }

    public void SetInfo(CoachEntity coach)
    {
        current = coach;

        nameText.text = $"{coach.entityName} ({coach.curAge.Value}세)";
        int yearsToRetire = coach.retireAge - coach.curAge.Value;
        retireText.text = yearsToRetire > 0 ? $"은퇴까지 {yearsToRetire}년" : "은퇴 예정";

        // 코치 등급 기반 피로도 감소
        int fatigueReduction = (coach.grade == CoachGrade.스카우트센터) ? 1 : 2;
        fatigueText.text = $"{fatigueReduction}";

        // SaveManager에서 루틴 배치 확인
        var save = saveManager.GetCurrentSave();
        int routineIndex = System.Array.IndexOf(save.coachAssign, coach.id);

        if (routineIndex >= 0 && routineIndex < 4)
        {
            // 슬롯 번호 → 시설 이름 매핑
            string[] routineNames = { "스피드 스케이팅", "피겨 스케이팅", "스켈레톤", "스키점프" };
            routineText.text = $"{routineNames[routineIndex]}";
        }
        else
        {
            routineText.text = "없음";
        }

        gameObject.SetActive(true);
    }

    private void FireCoach()
    {
        firePanel.OnConfirmed -= HandleConfirmed;
        firePanel.OnCanceled -= HandleCanceled;

        firePanel.OnConfirmed += HandleConfirmed;
        firePanel.OnCanceled += HandleCanceled;

        firePanel.Open(current);
    }

    private void HandleConfirmed(CoachEntity who)
    {
        coachService.OutCoach(who);
        OnFired?.Invoke(who);
        gameObject.SetActive(false);
    }

    private void HandleCanceled()
    {
        // 취소 시 아무 동작 없음
    }
}
