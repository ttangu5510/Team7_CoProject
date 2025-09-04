using JYL;
using SHG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
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

        [SerializeField] private TextMeshProUGUI recruitChanceText;
        [SerializeField] private TextMeshProUGUI recruitCostText;
        [Header("Buttons")]
        [SerializeField] private Button informationButton;
        [SerializeField] private Button recruitmentButton;
        [Header("Panels")]
        [SerializeField] public GameObject playerInormationPanel;
        [SerializeField] public GameObject NotificationWindow;  // 알림창 패널
        [SerializeField] public GameObject ConfirmPlayerRecruitment;    // 선수 영입 확인 패널

        public DomAthEntity playerData; // 현재 연동된 선수 정보

        private int recruitCost;    // 영입 비용
        private int recruitSuccessRate; // 영입 성공 확률

        [Inject] private IResourceController resourceController;    // 자원 컨트롤러
        [Inject] private DomAthService athService;  // 국내 선수 서비스


        public void SetPlayer(DomAthEntity player)
        {
            nameText.text = player.entityName;
            gradeText.text = player.affiliation.ToString();
            ageText.text = player.recruitAge.ToString();
            typeText.text = player.maxGrade.ToString();
            playerData = player;

            // 등급에 따른 성공률/비용 설정
            switch (player.affiliation)
            {
                case AthleteAffiliation.일반선수:
                    recruitSuccessRate = 60;
                    recruitCost = 100;
                    break;
                case AthleteAffiliation.국가대표후보:
                    recruitSuccessRate = 40;
                    recruitCost = 500;
                    break;
                case AthleteAffiliation.국가대표:
                    recruitSuccessRate = 20;
                    recruitCost = 1000;
                    break;
                default:
                    recruitSuccessRate = 30;
                    recruitCost = 800;
                    break;
            }
            recruitChanceText.text = $"성공률\n{recruitSuccessRate}%";
            recruitCostText.text = $"영입하기\n{recruitCost:N0}G";
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

        public void OnInformationButtonClicked()    // 선수 정보
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

        public void OnRecruitmentButtonClicked()    // 영입
        {
            //resourceController.SpendMoney(500, ExpensesType.Scout);
            //Debug.Log($"남은 돈: {resourceController.Money}");
            if (resourceController == null)
            {
                Debug.LogError("[PlayerUI] resourceController가 null입니다. Zenject 바인딩과 인스턴스 생성을 확인하세요.");
                return;
            }
            if (resourceController.Money.Value < recruitCost)
            {
                Debug.LogWarning("골드가 부족합니다.");
                if (NotificationWindow != null)
                    NotificationWindow.SetActive(true);
                return;
            }

            // 영입 성공 확률 적용
            int dice = Random.Range(1, 101); // 1~100
            if (dice <= recruitSuccessRate)
            {
                //playerData.Recruit();
                athService.RecruitAthlete(playerData.entityName);
                Debug.Log($"{nameText.text} 영입 성공!");
                resourceController.SpendMoney(recruitCost, ExpensesType.Scout);
                Debug.Log($"남은 돈: {resourceController.Money.Value}");
                // 영입 성공시 UI 오브젝트 삭제 (혹은 상태 변경)
                Destroy(gameObject);
            }
            else
            {
                // 실패 로직 예시: 알림 팝업 호출(선택)
                if (NotificationWindow != null)
                    NotificationWindow.SetActive(true);
                Debug.Log($"{nameText.text} 영입 실패.");
                resourceController.SpendMoney(recruitCost, ExpensesType.Scout);
                Debug.Log($"남은 돈: {resourceController.Money.Value}");
            }

        }



    }
}