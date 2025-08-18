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


        private void Start()
        {
            // 샘플 데이터
            playerList.Add(new Player { name = "신재원", grade = "일반등급", age = 21, type = "C" });
            playerList.Add(new Player { name = "이민호", grade = "국대 후보", age = 20, type = "B" });
            playerList.Add(new Player { name = "민만준", grade = "국가대표", age = 22, type = "A" });
            playerList.Add(new Player { name = "안정환", grade = "일반등급", age = 19, type = "C" });
            playerList.Add(new Player { name = "이규진", grade = "국대 후보", age = 24, type = "B" });
            playerList.Add(new Player { name = "신희관", grade = "국가대표", age = 26, type = "A" });

            playerRecruitmentButton.onClick.AddListener(DisplayPlayers);
        }

        private void DisplayPlayers()
        {
            for (int i = 0; i < playerList.Count; i++)
            {
                GameObject go = Instantiate(playerUIPrefab, playerListPanel);
                PlayerUI ui = go.GetComponent<PlayerUI>();
                ui.SetPlayer(playerList[i]);
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