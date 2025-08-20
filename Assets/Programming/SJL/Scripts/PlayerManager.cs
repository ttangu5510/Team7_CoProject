using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] Button playerRecruitmentButton;

        public List<Player> playerList = new List<Player>();

        [SerializeField] public GameObject playerUIPrefab;
        [SerializeField] public Transform playerListPanel; // 선수들을 담을 부모 오브젝트
        [SerializeField] public GameObject playerInformationPanel; // ← 패널 오브젝트 직접 참조

        private void Start()
        {
            // 샘플 데이터
            playerList.Add(new Player { name = "신재원", grade = "일반등급", age = 21, type = "C",
                stamina = 1, agility=1, flexibility=1, technique=1, speed=1, balance=1, fatigue=1, mental=1});
            playerList.Add(new Player { name = "이민호", grade = "국대 후보", age = 20, type = "B",
                stamina = 2, agility=2, flexibility=2, technique=2, speed=2, balance=2, fatigue=2, mental=2});
            playerList.Add(new Player { name = "민만준", grade = "국가대표", age = 22, type = "A",
                stamina = 3, agility=3, flexibility=3, technique=3, speed=3, balance=3, fatigue=3, mental=3});
            playerList.Add(new Player { name = "안정환", grade = "일반등급", age = 19, type = "C",
                stamina = 1, agility=1, flexibility=1, technique=1, speed=1, balance=1, fatigue=1, mental=1});
            playerList.Add(new Player { name = "이규진", grade = "국대 후보", age = 24, type = "B",
                stamina = 2, agility=2, flexibility=2, technique=2, speed=2, balance=2, fatigue=2, mental=2});
            playerList.Add(new Player { name = "신희관", grade = "국가대표", age = 26, type = "A",
                stamina = 3, agility=3, flexibility=3, technique=3, speed=3, balance=3, fatigue=3, mental=3});

            playerRecruitmentButton.onClick.AddListener(DisplayPlayers);
        }

        private void DisplayPlayers()
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                GameObject go = Instantiate(playerUIPrefab, playerListPanel);
                PlayerUI ui = go.GetComponent<PlayerUI>();
                ui.SetPlayer(playerList[i]);

                ui.playerInormationPanel = playerInformationPanel; // << 여기가 핵심!
                // (필요하면) Player 정보도 패널에 전달
                ui.playerData = playerList[i];
            }
        }
    }


    [Serializable]
    public class Player
    {
        public String name;
        public int age;
        public string grade;
        public string type;

        public int stamina;
        public int agility;
        public int flexibility;
        public int technique;
        public int speed;
        public int balance;
        public int fatigue;
        public int mental;
    }
}
