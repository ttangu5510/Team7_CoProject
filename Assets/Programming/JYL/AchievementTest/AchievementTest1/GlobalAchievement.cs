using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JYL.AchievementTest01
{
    public class GlobalAchievement : MonoBehaviour
    {
        // 전역 변수
        public GameObject achNote; // 업적 알림 패널
        public AudioSource achSound; // 업적 달성 사운드
        public bool achActive = false; // 업적이 현재 발동 중인지 체크
        public GameObject achTitle;
        public GameObject achDesc;
    
        // Achievement 01 Specific
        public GameObject ach01Image;
        public static int Ach01Count; // 현재 달성도
        public int ach01Trigger = 5; // 달성 목표
        public int ach01Code; // 중복 발동 방지 코드

        // Achievement 02 Specific
        public GameObject ach02Image;
        public static bool ach02Trigger = false;
        public int ach02Code;
        
        // Achievement 03 Specific
        public GameObject ach03Image;
        public static bool ach03Trigger = false;
        public int ach03Code;
        
        void Update()
        {
            // ach01Code = PlayerPrefs.GetInt("Ach01");
            // ach02Code = PlayerPrefs.GetInt("Ach02");
            // ach03Code = PlayerPrefs.GetInt("Ach03");
            
            // 수집 카운트가 조건과 같고, 중복 발동 방지 코드가 아직 설정되지 않았다면 업적 코루틴 수행
            if (Ach01Count == ach01Trigger && ach01Code != 12345)
            {
                StartCoroutine(Trigger01Ach());
            }

            // 레벨 완료 트리거가 참이고, 코드가 다를 시 수행
            if (ach02Trigger && ach02Code != 12346)
            {
                StartCoroutine(Trigger02Ach());
            }

            if (ach03Trigger && ach03Code != 12347)
            {
                StartCoroutine(Trigger03Ach());
            }
        }

        private IEnumerator Trigger01Ach()
        {
            achActive = true; // 업적 발동
            ach01Code = 12345; // 중복 발동 방지를 위한 코드를 저장.
            PlayerPrefs.SetInt("Ach01", ach01Code); // 로컬에 달성된 업적 저장.
            achSound.Play();// 업적 사운드 재생
        
            // 업적 패널 활성화
            ach01Image.SetActive(true);
            achTitle.GetComponent<TextMeshProUGUI>().text = "Collection !!!";
            achDesc.GetComponent<TextMeshProUGUI>().text = "Created a collection based achievement.";
            achNote.SetActive(true);

            yield return new WaitForSeconds(7f); // 7초 대기
        
            // UI 초기화
            achNote.SetActive(false);
            ach01Image.SetActive(false);
            achTitle.GetComponent<TextMeshProUGUI>().text = "";
            achDesc.GetComponent<TextMeshProUGUI>().text = "";
        
            // 업적 발동 초기화
            achActive = false;
        }

        private IEnumerator Trigger02Ach()
        {
            achActive = true;
            ach02Code = 12346;
            PlayerPrefs.SetInt("Ach02", ach02Code);
            achSound.Play();

            // 업적 패널 활성화
            ach02Image.SetActive(true);
            achTitle.GetComponent<TextMeshProUGUI>().text = "Level Completed !!!";
            achDesc.GetComponent<TextMeshProUGUI>().text = "You made a \"level completed\" achievement.";
            achNote.SetActive(true);

            yield return new WaitForSeconds(7f); // 7초 대기
        
            // UI 초기화
            achNote.SetActive(false);
            ach02Image.SetActive(false);
            achTitle.GetComponent<TextMeshProUGUI>().text = "";
            achDesc.GetComponent<TextMeshProUGUI>().text = "";
        
            // 업적 발동 초기화
            achActive = false;
        }

        private IEnumerator Trigger03Ach()
        {
            achActive = true;
            ach03Code = 12347;
            PlayerPrefs.SetInt("Ach03", ach03Code);
            achSound.Play();

            // 업적 패널 활성화
            ach03Image.SetActive(true);
            achTitle.GetComponent<TextMeshProUGUI>().text = "Well Timed";
            achDesc.GetComponent<TextMeshProUGUI>().text = "You created time based achievement.";
            achNote.SetActive(true);

            yield return new WaitForSeconds(7f); // 7초 대기
        
            // UI 초기화
            achNote.SetActive(false);
            ach03Image.SetActive(false);
            achTitle.GetComponent<TextMeshProUGUI>().text = "";
            achDesc.GetComponent<TextMeshProUGUI>().text = "";
        
            // 업적 발동 초기화
            achActive = false;
        }
    }
}