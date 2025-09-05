using JYL;
using SHG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SJL
{
    public class HiringCoachBox : MonoBehaviour
    {
        [SerializeField] Button HiringCoachButton;

        [Inject] private CoachService coachService;
        [Inject] private IResourceController resourceController;
        [Inject] private DiContainer container;

        [SerializeField] public coachUI coachUIPrefab;
        [SerializeField] public Transform coachListPanel; // 코치 담을 부모 오브젝트
        //[SerializeField] public GameObject playerInformationPanel;

        public List<CoachEntity> coachDataList = new();    // 코치 데이터 리스트

        private void Start()
        {
            HiringCoachButton.onClick.AddListener(DisplayPlayers);
        }

        private void DisplayPlayers()
        {
            resourceController.SpendMoney(100, ExpensesType.Scout);
            Debug.Log($"남은 돈: {resourceController.Money}");
            coachDataList.Clear();
            coachDataList = coachService.GetCanRecruitCoaches();

            for (int i = 0; i < coachListPanel.transform.childCount; i++)
            {
                Destroy(coachListPanel.transform.GetChild(i).gameObject);
            }

            // 다른 방법
            // foreach (Transform item in playerInformationPanel.transform)
            // {
            //     Destroy(item.gameObject);
            // }

            // 플레이어 리스트를 복제 및 섞기 // todo : 시설 수준과 선수의 등급별 확률 조정
            List<CoachEntity> shuffledList = new(coachDataList);

            // if (shuffledList[0].affiliation == AthleteAffiliation.일반선수)
            // {
            //     // 확률 = 시설 수준이 0단계면, 65% (플로우 차트 참고)
            // }

            System.Random rng = new System.Random();
            int n = shuffledList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (shuffledList[k], shuffledList[n]) = (shuffledList[n], shuffledList[k]);
            }

            // 앞에서부터 5명만 표시
            int displayCount = Mathf.Min(5, shuffledList.Count);
            for (int i = 0; i < displayCount; i++)
            {
                coachUI ui = container.InstantiatePrefabForComponent<coachUI>(coachUIPrefab, coachListPanel);
                ui.SetPlayer(shuffledList[i]); // 변환 후 전달
                //ui.playerInormationPanel = playerInformationPanel;
                ui.coachData = shuffledList[i]; // 필요시 변환 객체도 넣을 수 있음
            }

        }
    }
}
