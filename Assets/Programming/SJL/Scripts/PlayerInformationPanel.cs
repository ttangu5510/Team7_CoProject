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
        [SerializeField] private Slider mentalSlider;


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
            mentalSlider.value = 100; // TODO : 이거 있는건지 확인필요

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

    }
}