using SJL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{ 
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI typeText;

        [SerializeField] private Button informationButton;
        [SerializeField] private Button recruitmentButton;

        public void SetPlayer(Player player)
        {
            nameText.text = player.name;
            gradeText.text = player.grade;
            ageText.text = player.age.ToString();
            typeText.text = player.type;
        }

        private void Start()
        {
            informationButton.onClick.AddListener(OnInformationButtonClicked);
            recruitmentButton.onClick.AddListener(OnRecruitmentButtonClicked);
        }

        public void OnInformationButtonClicked()
        {
            // 선수 정보 버튼 클릭 시 동작
            Debug.Log("선수 정보 버튼 클릭됨: " + nameText.text);
        }

        public void OnRecruitmentButtonClicked()
        {
            // 선수 영입 버튼 클릭 시 동작
            Debug.Log("선수 영입 버튼 클릭됨: " + nameText.text);
        }


    }
}