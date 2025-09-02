using JYL;
using SHG;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Zenject.SpaceFighter;

namespace SJL
{
    public class PlayerListUpdateBox : MonoBehaviour
    {
        [SerializeField] Button playerRecruitmentButton;

        [Inject] private DomAthService athService;
        [Inject] private IResourceController resourceController;

        [SerializeField] public GameObject playerUIPrefab;
        [SerializeField] public Transform playerListPanel; // 선수들을 담을 부모 오브젝트
        [SerializeField] public GameObject playerInformationPanel;

        public List<DomAthEntity> playerDataList = new();    // 모든 선수 데이터 리스트

        private void Start()
        {
            playerRecruitmentButton.onClick.AddListener(DisplayPlayers);
        }

        private void DisplayPlayers()
        {
            resourceController.SpendMoney(100, ExpensesType.Scout);
            Debug.Log($"남은 돈: {resourceController.Money}");
            playerDataList.Clear();
            playerDataList = athService.GetAllCanRecruitAthleteList();

            for (int i = 0; i < playerListPanel.transform.childCount; i++)
            {
                Destroy(playerListPanel.transform.GetChild(i).gameObject);
            }

            // 다른 방법
            // foreach (Transform item in playerInformationPanel.transform)
            // {
            //     Destroy(item.gameObject);
            // }

            // 플레이어 리스트를 복제 및 섞기 // todo : 시설 수준과 선수의 등급별 확률 조정
            List<DomAthEntity> shuffledList = new(playerDataList);

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
                GameObject go = Instantiate(playerUIPrefab, playerListPanel);
                PlayerUI ui = go.GetComponent<PlayerUI>();
                ui.SetPlayer(shuffledList[i]);
                ui.playerInormationPanel = playerInformationPanel;

                ui.playerData = shuffledList[i];
            }

        }

    }
}