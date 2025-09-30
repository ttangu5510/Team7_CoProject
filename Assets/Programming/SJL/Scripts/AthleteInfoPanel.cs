using SJL;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JYL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class AthleteInfoPanel : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] Button closeButton;
         
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

        private UIAnimator animator;

        private void Awake()
        {
            // 버튼 이벤트 설정
            closeButton.OnClickAsObservable()
                .Subscribe(_=>OnCloseButtonClicked())
                .AddTo(this);
            animator = GetComponent<UIAnimator>();
        }
        
        public void SetInfo(DomAthEntity athlete)
        {
            // 선수 정보 설정
            // TODO : 선수 스프라이트
            // athleteIcon.sprite = Resources.Load<Sprite>($"{iconPath}{athlete.id}");
            nameText.text = athlete.entityName;
            gradeText.text = athlete.affiliation.ToString();
            ageText.text = athlete.recruitAge.ToString();
            typeText.text = athlete.maxGrade.ToString();
            growthPotentialText.text = $"최대 성장 가능성 : {athlete.maxGrade.ToString()}";
            //retreatText.text = "은퇴까지 N년 N주";
            
            // 슬라이더 값 설정
            staminaSlider.value = athlete.stats.health / maxValue;
            agilitySlider.value = athlete.stats.quickness / maxValue;
            flexibilitySlider.value = athlete.stats.flexibility / maxValue;
            techniqueSlider.value = athlete.stats.technic / maxValue;
            speedSlider.value = athlete.stats.speed / maxValue;
            balanceSlider.value = athlete.stats.balance / maxValue;
            fatigueSlider.value = athlete.stats.fatigue / maxValue;
            
            // 등급 텍스트 설정
            staminaRatingText.text = GetRating(athlete.stats.health);
            agilityRatingText.text = GetRating(athlete.stats.quickness);
            flexibilityRatingText.text = GetRating(athlete.stats.flexibility);
            techniqueRatingText.text = GetRating(athlete.stats.technic);
            speedRatingText.text = GetRating(athlete.stats.speed);
            balanceRatingText.text = GetRating(athlete.stats.balance);
            fatigueRatingText.text = athlete.stats.fatigue.ToString();
        } 
        private void OnCloseButtonClicked()
        {
            // 정보 패널 닫기
            Debug.Log("정보 패널 닫힘");
            OnClose();
        }

        private async UniTaskVoid OnClose()
        {
            animator.PlayOut();
            await UniTask.WaitForSeconds(animator.outDuration);
            gameObject.SetActive(false);
        }

        private string GetRating(int value) // 등급 계산
        {
            if (value > (int)AthleteGrade.A * 100) return "A";
            if (value > (int)AthleteGrade.B * 100) return "B";
            if (value > (int)AthleteGrade.C * 100) return "C";
            if (value > (int)AthleteGrade.D * 100) return "D";
            if (value > (int)AthleteGrade.E * 100) return "E";
            if (value > (int)AthleteGrade.F) return "F";
            Debug.LogWarning($"입력 수치가 0이거나, 0 아래임{value}");
            return "-1";
        }
    }
}