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
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI typeText;
        [Header("Buttons")]
        [SerializeField] private Button informationButton;
        [SerializeField] private Button recruitmentButton;
        [Header("Panels")]
        [SerializeField] public GameObject playerInormationPanel;
        [SerializeField] public GameObject NotificationWindow;  // 알림창 패널
        [SerializeField] public GameObject ConfirmPlayerRecruitment;    // 선수 영입 확인 패널

        public Player playerData; // 현재 연동된 선수 정보

        public void SetPlayer(Player player)
        {
            nameText.text = player.name;
            gradeText.text = player.grade;
            ageText.text = player.age.ToString();
            typeText.text = player.type;
            playerData = player;
        }

        private void Start()
        {
            informationButton.onClick.AddListener(OnInformationButtonClicked);
            recruitmentButton.onClick.AddListener(OnRecruitmentButtonClicked);
        }

        public void OnInformationButtonClicked()
        {
            Debug.Log("선수 정보 버튼 클릭됨: " + nameText.text);
            if (playerInormationPanel != null && playerData != null)
            {
                playerInormationPanel.SetActive(true);

                // 여기에서 PlayerInformationPanel로 캐스팅!
                PlayerInformationPanel info = playerInormationPanel.GetComponent<PlayerInformationPanel>();
                if (info != null)
                    info.SetPlayer(playerData); // 선수 데이터 넘기기
            }
            else
            {
                Debug.LogError("패널 또는 선수 정보가 할당되지 않았습니다.");
            }
        }

        public void OnRecruitmentButtonClicked()
        {
            // 선수 영입 버튼 클릭 시 동작
            Debug.Log("선수 영입 버튼 클릭됨: " + nameText.text);
            
        }



    }
}