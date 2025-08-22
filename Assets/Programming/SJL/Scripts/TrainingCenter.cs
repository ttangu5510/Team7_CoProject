using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class TrainingCenter : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button FacilityInformationButton;
        [SerializeField] private Button trainingButton;
        [SerializeField] private Button specialTrainingButton;
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI explanatoryText;
        [Header("panel")]
        [SerializeField] private GameObject FacilityInformationBox;
        [SerializeField] private GameObject TrainingBox;
        [SerializeField] private GameObject specialTrainingBox;

        private void Awake()
        {
            // 버튼 클릭 이벤트 등록
            FacilityInformationButton.onClick.AddListener(OnFacilityInformationClicked);
            trainingButton.onClick.AddListener(OnTrainingClicked);
            specialTrainingButton.onClick.AddListener(OnSpecialTrainingClicked);
        }

        private void OnFacilityInformationClicked()
        {
            // 설명 텍스트 설정
            explanatoryText.text = "시설 : 0 단계\n" +
                                   "훈련 스탯 추가 수치 : + 0\n" +
                                   "훈련과 특훈 진행 가능.";
            // 시설 정보 박스 활성화
            FacilityInformationBox.SetActive(true);
            TrainingBox.SetActive(false);
            specialTrainingBox.SetActive(false);
        }

        private void OnTrainingClicked()
        {
            // 설명 텍스트 설정
            explanatoryText.text = "선수틀을 배치하여 훈련시킬 수 있습니다.\n" +
                                   "루틴에 따라 상승하는 능력치가 달라집니다..";
            // 훈련 박스 활성화
            FacilityInformationBox.SetActive(false);
            TrainingBox.SetActive(true);
            specialTrainingBox.SetActive(false);
        }

        private void OnSpecialTrainingClicked()
        {
            // 설명 텍스트 설정
            explanatoryText.text = "선수틀을 배치하여 훈련시킬 수 있습니다.\n" +
                                   "루틴에 따라 상승하는 능력치가 달라집니다..";
            // 특별 훈련 박스 활성화
            FacilityInformationBox.SetActive(false);
            TrainingBox.SetActive(false);
            specialTrainingBox.SetActive(true);
        }


    }
}
