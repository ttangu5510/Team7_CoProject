using JYL;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject.SpaceFighter;

namespace SJL
{
    public class PlayerListUpdateBox : MonoBehaviour
    {
        [SerializeField] Button playerRecruitmentButton;

        [SerializeField] public GameObject playerUIPrefab;
        [SerializeField] public Transform playerListPanel; // 선수들을 담을 부모 오브젝트

        public List<DomAthEntity> playerDataList = new List<DomAthEntity>();    // 모든 선수 데이터 리스트

        private void Start()
        {
            playerRecruitmentButton.onClick.AddListener(DisplayPlayers);
        }

        private void DisplayPlayers()
        {
            // 기존 UI 오브젝트 모두 제거
            foreach (Transform child in playerListPanel)
            {
                Destroy(child.gameObject);
            }

            // 선수 리스트를 랜덤 셔플, 5명만 선택
            var randomList = playerDataList.OrderBy(x => Random.value).Take(5).ToList();

            foreach (var player in randomList)
            {
                GameObject go = Instantiate(playerUIPrefab, playerListPanel);
                PlayerUI ui = go.GetComponent<PlayerUI>();
                ui.SetPlayer(player);
            }
        }

    }
}