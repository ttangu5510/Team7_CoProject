using SJL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{ 
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI typeText;

        [SerializeField] private Button informationButton;
        [SerializeField] private Button recruitmentButton;

        [SerializeField] public GameObject PlayerInformationPrefab;

        public void SetPlayer(Player player)
        {
            nameText.text = player.name;
            gradeText.text = player.grade;
            ageText.text = player.age.ToString();
            typeText.text = player.type;
        }

        private void Start()
        {
            informationButton.onClick.AddListener(OnInformationButtonClicked);
            recruitmentButton.onClick.AddListener(OnRecruitmentButtonClicked);
        }

        public void OnInformationButtonClicked()
        {
            // 선수 정보 버튼 클릭 시 동작
            Debug.Log("선수 정보 버튼 클릭됨: " + nameText.text);

            // 이미 열려있는 창이 있으면 중복 생성 방지 (원하면 삭제 가능)
            if (GameObject.Find(PlayerInformationPrefab.name + "(Clone)") != null)
                return;

            // 부모 캔버스 찾기 (가장 보편적으로는 "Canvas" 이름)
            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null)
            {
                Debug.LogError("Canvas를 찾을 수 없습니다!");
                return;
            }

            // 선수 정보 팝업 생성
            GameObject infoWindow = Instantiate(PlayerInformationPrefab, canvas.transform);

        }

        public void OnRecruitmentButtonClicked()
        {
            // 선수 영입 버튼 클릭 시 동작
            Debug.Log("선수 영입 버튼 클릭됨: " + nameText.text);
        }



    }
}