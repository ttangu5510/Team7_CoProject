using SJL;
using System.Collections;
using System.Collections.Generic;
using JYL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject.SpaceFighter;

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

        public DomAthEntity playerData; // 현재 연동된 선수 정보

        public void SetPlayer(DomAthEntity player)
        {
            nameText.text = player.entityName;
            gradeText.text = player.affiliation.ToString();
            ageText.text = player.recruitAge.ToString();
            typeText.text = player.maxGrade.ToString();
            playerData = player;
        }

        private void Start()
        {
            informationButton.OnClickAsObservable()
                .Subscribe(_ => OnInformationButtonClicked())
                .AddTo(this);

            recruitmentButton.OnClickAsObservable()
                .Subscribe(_ => OnRecruitmentButtonClicked())
                .AddTo(this);
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
            playerData.Recruit();
            Destroy(gameObject);
            Debug.Log("선수 영입 버튼 클릭됨: " + nameText.text);
            
        }



    }
}