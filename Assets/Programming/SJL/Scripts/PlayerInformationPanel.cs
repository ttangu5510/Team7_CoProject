using SJL;
using System.Collections;
using System.Collections.Generic;
using JYL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class PlayerInformationPanel : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] Button closeButton;
        [Header("Player Information")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI typeText;
        [SerializeField] private TextMeshProUGUI growthPotentialText;
        [SerializeField] private TextMeshProUGUI retreatText;
        [Header("Player Attributes")]
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


        public void SetPlayer(DomAthEntity player)
        {
            // 선수 정보 설정
            nameText.text = player.entityName;
            gradeText.text = player.affiliation.ToString();
            ageText.text = player.recruitAge.ToString();
            typeText.text = player.maxGrade.ToString();
            growthPotentialText.text = $"최대 성장 가능성 : {player.maxGrade.ToString()}";
            retreatText.text = "은퇴까지 N년 N주";
            // 슬라이더 값 설정
            staminaSlider.value = player.stats.health;
            agilitySlider.value = player.stats.quickness;
            flexibilitySlider.value = player.stats.flexibility;
            techniqueSlider.value = player.stats.technic;
            speedSlider.value = player.stats.speed;
            balanceSlider.value = player.stats.balance;
            fatigueSlider.value = player.stats.fatigue;
            // 등급 텍스트 설정
            staminaRatingText.text = GetRating(player.stats.health);
            agilityRatingText.text = GetRating(player.stats.quickness);
            flexibilityRatingText.text = GetRating(player.stats.flexibility);
            techniqueRatingText.text = GetRating(player.stats.technic);
            speedRatingText.text = GetRating(player.stats.speed);
            balanceRatingText.text = GetRating(player.stats.balance);
            fatigueRatingText.text = GetRating(player.stats.fatigue);
        }

        private void Start()
        {
            closeButton.onClick.AddListener(OnCloseButtonClicked);
        }

        public void OnCloseButtonClicked()
        {
            // 정보 패널 닫기
            Debug.Log("정보 패널 닫힘");
            gameObject.SetActive(false);
        }

        private string GetRating(int value) // 등급 계산
        {
            if (value >= 85) return "A";
            if (value >= 70) return "B";
            if (value >= 50) return "C";
            return "D";
        }

    }
}