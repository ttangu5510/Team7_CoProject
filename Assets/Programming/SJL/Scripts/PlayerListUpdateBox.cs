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
        [Inject] private DiContainer container;
        [Inject] private IFacilitiesController facilitiesController;

        [SerializeField] public PlayerUI playerUIPrefab;
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

            // 기존에 있던 선수 UI 제거
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

            // 확률값(%) 적용: 시설 컨트롤러에서 값 가져오기
            //Debug.Log($"NationalGradeAthlete 확률: {facilitiesController.ScoutCenter.ChanceForNationalGradeAthlete.Value}");
            //Debug.Log($"시설 레벨 : {facilitiesController.ScoutCenter.CurrentStage.Value}");
            float nationalChance = facilitiesController.ScoutCenter.ChanceForNationalGradeAthlete.Value; // 0.01 ~ 0.07, etc.

            var nationalList = shuffledList.Where(x => x.affiliation == AthleteAffiliation.국가대표).ToList();  
            var candidateList = shuffledList.Where(x => x.affiliation == AthleteAffiliation.국가대표후보).ToList();
            var normalList = shuffledList.Where(x => x.affiliation == AthleteAffiliation.일반선수).ToList();

            int displayCount = Mathf.Min(5, shuffledList.Count);    // 최대 5명 표시

            List<DomAthEntity> finalList = new();   // 최종 표시할 선수 리스트

            System.Random rng = new System.Random();    // 난수 생성기

            for (int i = 0; i < displayCount; i++)  // 5명 리스트 출력
            {
                double roll = rng.NextDouble(); // 0.0 이상 1.0 미만

                if (nationalList.Count > 0 && roll < nationalChance)    // 국가대표 확률에 걸리면 국가대표 우선 선택
                {
                    int idx = rng.Next(nationalList.Count); // 국가대표 리스트에서 랜덤 선택
                    finalList.Add(nationalList[idx]);   // 최종 리스트에 추가
                    nationalList.RemoveAt(idx); // 선택된 국가대표 리스트에서 제거
                }
                else
                {
                    // 후보와 일반선수가 한쪽으로 치우치지 않게 랜덤 결정
                    bool pickCandidate = false;

                    if (candidateList.Count > 0 && normalList.Count > 0)    // 둘 다 있으면 확률로 결정
                    {
                        pickCandidate = (rng.NextDouble() < 0.3);   // 30% 확률로 후보자 선택
                    }
                    else if (candidateList.Count > 0)   // 후보자만 있으면 후보자 선택
                    {
                        pickCandidate = true;   // 후보자 선택
                    }
                    else if (normalList.Count > 0)  // 일반선수만 있으면 일반선수 선택
                    {
                        pickCandidate = false;  // 일반선수 선택
                    }
                    else
                    {
                        // 후보와 일반 모두 없음 -> 국가대표가 있다면 강제로 뽑기
                        if (nationalList.Count > 0)
                        {
                            int idx = rng.Next(nationalList.Count); // 국가대표 리스트에서 랜덤 선택
                            finalList.Add(nationalList[idx]);   // 최종 리스트에 추가
                            nationalList.RemoveAt(idx); // 선택된 국가대표 리스트에서 제거
                        }
                        else
                        {
                            // 모든 리스트가 비어있으면 루프 종료
                            break;
                        }
                        continue; // 다음 반복
                    }

                    if (pickCandidate && candidateList.Count > 0)   // 후보자 선택
                    {
                        int idx = rng.Next(candidateList.Count);    // 후보자 리스트에서 랜덤 선택
                        finalList.Add(candidateList[idx]);  // 최종 리스트에 추가
                        candidateList.RemoveAt(idx);    // 선택된 후보자 리스트에서 제거
                    }
                    else if (!pickCandidate && normalList.Count > 0)    // 일반선수 선택
                    {
                        int idx = rng.Next(normalList.Count);   // 일반선수 리스트에서 랜덤 선택
                        finalList.Add(normalList[idx]); // 최종 리스트에 추가
                        normalList.RemoveAt(idx);   // 선택된 일반선수 리스트에서 제거
                    }
                }
            }

            for (int i = 0; i < finalList.Count; i++)   // 최종 리스트로 UI 생성
            {
                PlayerUI ui = container.InstantiatePrefabForComponent<PlayerUI>(playerUIPrefab, playerListPanel);   // Zenject로 생성
                ui.SetPlayer(finalList[i]); // 변환 후 전달
                ui.playerInormationPanel = playerInformationPanel;  // 정보 패널 연결
                ui.playerData = finalList[i];   // 선수 데이터 설정
            }


            //// 섞기 (Fisher-Yates 알고리즘)
            //System.Random rng = new System.Random();
            //int n = shuffledList.Count;
            //while (n > 1)
            //{
            //    n--;
            //    int k = rng.Next(n + 1);
            //    (shuffledList[k], shuffledList[n]) = (shuffledList[n], shuffledList[k]);
            //}

            //// 앞에서부터 5명만 표시
            //int displayCount = Mathf.Min(5, shuffledList.Count);
            //for (int i = 0; i < displayCount; i++)
            //{
            //    PlayerUI ui = container.InstantiatePrefabForComponent<PlayerUI>(playerUIPrefab, playerListPanel);
            //    //PlayerUI ui = go.GetComponent<PlayerUI>();
            //    ui.SetPlayer(shuffledList[i]);
            //    ui.playerInormationPanel = playerInformationPanel;

            //    ui.playerData = shuffledList[i];
            //}

        }

    }
}