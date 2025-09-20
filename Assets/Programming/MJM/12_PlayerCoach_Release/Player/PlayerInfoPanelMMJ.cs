using JYL;
using SJL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace MMJ
{
    public class PlayerInfoPanelMMJ : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button fireButton;
        [SerializeField] private PlayerFirePanelMMJ playerFirePanel;   // GameObject → PlayerFirePanelMMJ로 교체

        [Header("Player Information")]
        [SerializeField] private Image athleteIcon;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private TextMeshProUGUI growthPotentialText;
        [SerializeField] private TextMeshProUGUI retreatText;

        [Header("Player Attributes")]
        [SerializeField] private float maxValue = 600f;
        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Slider agilitySlider;
        [SerializeField] private Slider flexibilitySlider;
        [SerializeField] private Slider techniqueSlider;
        [SerializeField] private Slider speedSlider;
        [SerializeField] private Slider balanceSlider;
        [SerializeField] private Slider fatigueSlider;

        [Header("Player Rating")]
        [SerializeField] private TextMeshProUGUI staminaRatingText;
        [SerializeField] private TextMeshProUGUI agilityRatingText;
        [SerializeField] private TextMeshProUGUI flexibilityRatingText;
        [SerializeField] private TextMeshProUGUI techniqueRatingText;
        [SerializeField] private TextMeshProUGUI speedRatingText;
        [SerializeField] private TextMeshProUGUI balanceRatingText;
        [SerializeField] private TextMeshProUGUI fatigueRatingText;

        // ▶ 상위 화면이 듣는 이벤트
        public event System.Action<DomAthEntity> OnFired;

        // 내부 상태
        private DomAthEntity current;
        private Sprite currentPortrait;

        [Inject] private JYL.DomAthService domAthService;

        private void Awake()
        {
            gameObject.SetActive(false); // 기본 비활성

            closeButton.OnClickAsObservable()
                .Subscribe(_ => gameObject.SetActive(false)) // Destroy → SetActive(false)
                .AddTo(this);

            fireButton.OnClickAsObservable()
                .Subscribe(_ => FirePlayer())
                .AddTo(this);
        }

        public void SetInfo(DomAthEntity athlete, Sprite portrait = null)
        {
            current = athlete;
            currentPortrait = portrait;

            // UI 채우기
            nameText.text = athlete.entityName;
            gradeText.text = athlete.affiliation.ToString();
            ageText.text = athlete.curAge.Value.ToString();
            typeText.text = athlete.maxGrade.ToString();
            growthPotentialText.text = $"최대 성장 가능성 : {athlete.maxGrade}";

            staminaSlider.value = athlete.stats.health / maxValue;
            agilitySlider.value = athlete.stats.quickness / maxValue;
            flexibilitySlider.value = athlete.stats.flexibility / maxValue;
            techniqueSlider.value = athlete.stats.technic / maxValue;
            speedSlider.value = athlete.stats.speed / maxValue;
            balanceSlider.value = athlete.stats.balance / maxValue;
            fatigueSlider.value = athlete.stats.fatigue / maxValue;

            staminaRatingText.text = GetRating(athlete.stats.health);
            agilityRatingText.text = GetRating(athlete.stats.quickness);
            flexibilityRatingText.text = GetRating(athlete.stats.flexibility);
            techniqueRatingText.text = GetRating(athlete.stats.technic);
            speedRatingText.text = GetRating(athlete.stats.speed);
            balanceRatingText.text = GetRating(athlete.stats.balance);
            fatigueRatingText.text = athlete.stats.fatigue.ToString();

            gameObject.SetActive(true);
        }

        private void FirePlayer()
        {
            // 중복 구독 방지
            playerFirePanel.OnConfirmed -= HandleConfirmed;
            playerFirePanel.OnCanceled -= HandleCanceled;

            playerFirePanel.OnConfirmed += HandleConfirmed;
            playerFirePanel.OnCanceled += HandleCanceled;

            playerFirePanel.Open(current, currentPortrait);
        }

        private void HandleConfirmed(DomAthEntity who)
        {
            domAthService.OutAthlete(who.entityName);
            OnFired?.Invoke(who);
            gameObject.SetActive(false); // Destroy → SetActive(false)
        }

        private void HandleCanceled()
        {
            // 취소 시 아무 일 없음
        }

        private string GetRating(int value)
        {
            if (value > (int)AthleteGrade.A * 100) return "A";
            if (value > (int)AthleteGrade.B * 100) return "B";
            if (value > (int)AthleteGrade.C * 100) return "C";
            if (value > (int)AthleteGrade.D * 100) return "D";
            if (value > (int)AthleteGrade.E * 100) return "E";
            if (value > (int)AthleteGrade.F) return "F";
            Debug.LogWarning($"입력 수치가 0이거나, 0 아래임 {value}");
            return "-1";
        }
    }
}
