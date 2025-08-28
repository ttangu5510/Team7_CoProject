using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JYL;
using SHG;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SJL
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] Button playerRecruitmentButton;

        [Inject] private DomAthService athService;
        [Inject] private IResourceController  resourceController;
        
        private List<DomAthEntity> canRecruitList = new();

        [SerializeField] public GameObject playerUIPrefab;
        [SerializeField] public Transform playerListPanel; // 선수들을 담을 부모 오브젝트
        [SerializeField] public GameObject playerInformationPanel; // ← 패널 오브젝트 직접 참조

        private void Start()
        {
            playerRecruitmentButton.onClick.AddListener(DisplayPlayers);
        }

        private void DisplayPlayers()
        {
            resourceController.SpendMoney(100, ExpensesType.Scout);
            
            canRecruitList.Clear();
            canRecruitList = athService.GetAllCanRecruitAthleteList();
            
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
            List<DomAthEntity> shuffledList = new(canRecruitList);
            
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
