using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;

namespace SJL
{
    public class TrainingCenter : MonoBehaviour
    {
        [Header("Button")]
        [SerializeField] private Button closeButton;
        [SerializeField] private Button FacilityInformationButton;
        [SerializeField] private Button trainingButton;
        [SerializeField] private Button specialTrainingButton;
        [SerializeField] private Button coachButton;
        [Header("Text")]
        [SerializeField] private TextMeshProUGUI explanatoryText;
        [Header("panel")]
        [SerializeField] private GameObject TrainingCenterConvers;
        [SerializeField] private GameObject FacilityInformationBox;
        [SerializeField] private GameObject TrainingBox;
        [SerializeField] private GameObject specialTrainingBox;
        [SerializeField] private GameObject underBar;
        [SerializeField] private GameObject progressCanvas;

        [SerializeField] private StatefulComponent statefulComponent;


        private void Awake()
        {
            // UniRx로 버튼 클릭 처리
            closeButton.OnClickAsObservable()
                .Subscribe(_ => TrainingCenterConvers.SetActive(false)).AddTo(this);

            FacilityInformationButton.OnClickAsObservable()
                .Subscribe(_ => ShowPanel(PanelType.FacilityInformation)).AddTo(this);

            trainingButton.OnClickAsObservable()
                .Subscribe(_ => { statefulComponent.SetState((int)StateRole.Active);
                    statefulComponent.SetRawTextByRole((int)TextRole.ExplanatioryText, "선수틀을 배치하여 훈련시킬 수 있습니다.\n" +
                        "루틴에 따라 상승하는 능력치가 달라집니다.\n\n" +
                        "<color=#FF3333>훈련을 진행할 때 7~12의 피로도가 상승하며 1턴(1주)가 소모됩니다.</color>");
                });
                //.Subscribe(_ => ShowPanel(PanelType.Training)).AddTo(this);

            specialTrainingButton.OnClickAsObservable()
                .Subscribe(_ => ShowPanel(PanelType.SpecialTraining)).AddTo(this); 
        }

        private void OnEnable()
        {
            underBar.SetActive(false);
           // progressCanvas.SetActive(false);
        }

        private void OnDisable()
        {
            underBar.SetActive(true);
            //progressCanvas.SetActive(true);
        }

        private enum PanelType { FacilityInformation, Training, SpecialTraining }

        private void ShowPanel(PanelType type)
        {
            switch (type)
            {
                case PanelType.FacilityInformation:
                    explanatoryText.text = "시설 : 0 단계\n훈련 스탯 추가 수치 : + 0\n훈련과 특훈 진행 가능.";
                    FacilityInformationBox.SetActive(true);
                    TrainingBox.SetActive(false);
                    specialTrainingBox.SetActive(false);
                    break;
                case PanelType.Training:
                    explanatoryText.text = "선수틀을 배치하여 훈련시킬 수 있습니다.\n" +
                        "루틴에 따라 상승하는 능력치가 달라집니다.\n\n" +
                        "<color=#FF3333>훈련을 진행할 때 7~12의 피로도가 상승하며 1턴(1주)가 소모됩니다.</color>";
                    FacilityInformationBox.SetActive(false);
                    TrainingBox.SetActive(true);
                    specialTrainingBox.SetActive(false);
                    break;
                case PanelType.SpecialTraining:
                    explanatoryText.text = "선수틀을 배치하여 특훈시킬 수 있습니다.\n루틴에 따라 상승하는 능력치가 달라집니다.";
                    FacilityInformationBox.SetActive(false);
                    TrainingBox.SetActive(false);
                    specialTrainingBox.SetActive(true);
                    break;
            }
        }

        
    }


}

