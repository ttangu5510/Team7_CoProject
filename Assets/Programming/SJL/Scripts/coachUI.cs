using JYL;
using SHG;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;
using UnityEngine.UI;
using UniRx;

namespace SJL
{
    public class coachUI : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;

        [SerializeField] private TextMeshProUGUI recruitChanceText;
        [SerializeField] private TextMeshProUGUI recruitCostText;
        [Header("Buttons")]
        [SerializeField] private Button recruitmentButton;
        [Header("Panels")]
        [SerializeField] public GameObject NotificationWindow;  // 알림창 패널
        [SerializeField] public GameObject ConfirmPlayerRecruitment;    // 선수 영입 확인 패널

        public CoachEntity coachData; // 현재 연동된 선수 정보

        private int recruitCost;    // 영입 비용
        private int recruitSuccessRate; // 영입 성공 확률

        [Inject]
        private IResourceController resourceController;


        public void SetPlayer(CoachEntity coach)
        {
            nameText.text = coach.entityName;
            gradeText.text = coach.grade.ToString();
            ageText.text = coach.curAge.ToString();

            coachData = coach;

            // 등급에 따른 성공률/비용 설정
            switch (coach.grade)
            {
                case CoachGrade.스카우트센터:
                    recruitSuccessRate = 50;
                    recruitCost = 1000;
                    break;
                case CoachGrade.선수출신:
                    recruitSuccessRate = 40;
                    recruitCost = 1500;
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
            recruitmentButton.OnClickAsObservable()
                .Subscribe(_ => OnRecruitmentButtonClicked())
                .AddTo(this);
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
                coachData.Recruit();
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