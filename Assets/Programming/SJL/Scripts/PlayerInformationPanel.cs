using SJL;
using System.Collections;
using System.Collections.Generic;
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


        public void SetPlayer(Player player)
        {
            // 선수 정보 설정
            nameText.text = player.name;
            gradeText.text = player.grade;
            ageText.text = player.age.ToString();
            typeText.text = player.type;
            growthPotentialText.text = $"최대 성장 가능성 : {player.type}";
            retreatText.text = "은퇴까지 N년 N주";
            // 슬라이더 값 설정
            staminaSlider.value = player.stamina;
            agilitySlider.value = player.agility;
            flexibilitySlider.value = player.flexibility;
            techniqueSlider.value = player.technique;
            speedSlider.value = player.speed;
            balanceSlider.value = player.balance;
            fatigueSlider.value = player.fatigue;
            mentalSlider.value = player.mental;

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