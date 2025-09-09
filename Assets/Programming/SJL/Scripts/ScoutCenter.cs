using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoutCenter : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private Button closeButton;
    [SerializeField] private Button FacilityInformation;
    [SerializeField] private Button PlayerRecruitment;
    [SerializeField] private Button coachRecruitment;
    [Header("Text")]
    [SerializeField] private TextMeshProUGUI explanatoryText;

    [Header("GameObject")] 
    [SerializeField] private GameObject scoutCenterCanvas; 

    [SerializeField] private GameObject FacilityInformationBox; 
    [SerializeField] private GameObject PlayerListUpdateBox;
    [SerializeField] private GameObject HiringCoachBox;

    private void Awake()
    {
        closeButton.OnClickAsObservable()
          .Subscribe(_ => {
            Debug.Log("closebutton clicked");
            scoutCenterCanvas.SetActive(false);
              }).AddTo(this);
        FacilityInformation.OnClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("FacilityInformation clicked");
                explanatoryText.text = "스카우트 센터는 새로운 선수와 코치를 영입할 수 있는 곳입니다.\n";
                FacilityInformationBox.SetActive(true);
                PlayerListUpdateBox.SetActive(false);
                HiringCoachBox.SetActive(false);
            }).AddTo(this);
        PlayerRecruitment.OnClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("PlayerRecruitment clicked");
                explanatoryText.text = "선수 영입을 통해 팀의 전력을 강화하세요!\n" +
                    "다양한 능력치를 가진 선수들이 대기 중입니다.\n" +
                    "영입 비용을 확인하고, 팀에 필요한 선수를 선택하여 최고의 라인업을 구성해보세요!";
                FacilityInformationBox.SetActive(false);
                PlayerListUpdateBox.SetActive(true);
                HiringCoachBox.SetActive(false);
            }).AddTo(this);
        coachRecruitment.OnClickAsObservable()
            .Subscribe(_ => {
                Debug.Log("coachRecruitment clicked");
                explanatoryText.text = "코치 영입을 통해 팀의 훈련 효율을 극대화하세요!\n";
                FacilityInformationBox.SetActive(false);
                PlayerListUpdateBox.SetActive(false);
                HiringCoachBox.SetActive(true);
            }).AddTo(this);

    }

    private void Start()
    {
       
    }

    private void OnEnable()
    {
        //underBar.SetActive(false);
        //progressCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        //underBar.SetActive(true);
        //progressCanvas.SetActive(true);
    }

}
