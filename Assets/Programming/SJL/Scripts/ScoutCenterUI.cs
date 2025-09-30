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
    public class ScoutCenterUI : MonoBehaviour
    {
        [SerializeField] Button playerRecruitmentButton;

        [Inject] private DiContainer container;
        [Inject] private DomAthService athService;
        [Inject] private IResourceController  resourceController;
        [Inject] private IFacilitiesController facilitiesController; // 시설 컨트롤러

        private List<DomAthEntity> canRecruitList = new();

        [SerializeField] public GameObject playerUIPrefab;
        [SerializeField] public Transform playerListPanel; // 선수들을 담을 부모 오브젝트
        
        public AthleteInfoPanel playerInformationPanel; // ← 패널 오브젝트 직접 참조



        private void Start()
        {
            playerRecruitmentButton.onClick.AddListener(DisplayPlayers);
        }

        private void DisplayPlayers()
        {
            resourceController.SpendMoney(100, ExpensesType.Scout); // 목록 갱신 시마다 100G 차감
            canRecruitList.Clear(); // 기존 리스트 초기화
            canRecruitList = athService.GetAllCanRecruitAthleteList();  // 영입 가능한 선수들 리스트 받아오기

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

            Debug.Log($"NationalGradeAthlete 확률: { facilitiesController.ScoutCenter.ChanceForNationalGradeAthlete.Value}");

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
                PlayerUI ui = container.InstantiatePrefabForComponent<PlayerUI>(playerUIPrefab, playerListPanel);
                ui.SetPlayer(shuffledList[i]);
                ui.playerInfoPanel = playerInformationPanel;
                
                ui.playerData = shuffledList[i];
            }
        }
    }
}
