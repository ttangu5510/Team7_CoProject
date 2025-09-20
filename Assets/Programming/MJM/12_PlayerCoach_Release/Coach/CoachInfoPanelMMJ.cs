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

        // 코치 효과 / 루틴 정보
        // fatigueText.text = $"훈련 피로도 감소 : {coach.fatigueReduction}";
        // routineText.text = coach.assignedRoutine >= 0
           // ? $"배치되어 있는 루틴 : 루틴 {coach.assignedRoutine + 1}"
           // : "배치되어 있는 루틴 : 없음"; -------------------------------------- todo 

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
        coachService.OutCoach(who.entityName);
        OnFired?.Invoke(who);
        gameObject.SetActive(false);
    }

    private void HandleCanceled()
    {
        // 취소 시 아무것도 안 함
    }
}
