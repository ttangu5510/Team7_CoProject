using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;

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
    [SerializeField] private GameObject underBar;
    [SerializeField] private GameObject progressCanvas;
    
    [SerializeField] private StatefulComponent statefulComponent;

    private void Awake()
    {
        closeButton.OnClickAsObservable()
            .Subscribe(_ => scoutCenterCanvas.SetActive(false)).AddTo(this);

    }

    private void Start()
    {
        FacilityInformation.OnClickAsObservable()
            .Subscribe(_ => { statefulComponent.SetState((int)StateRole.Active);
                statefulComponent.SetRawTextByRole((int)TextRole.ExplanatioryText, "스카우트 센터에서는 선수와 코치를 영입할 수 있습니다.\n영입 시 구단의 예산이 소모됩니다.\n\n영입한 선수와 코치는 팀 관리에서 확인할 수 있습니다.");
            }).AddTo(this);
    }

    private void OnEnable()
    {
        underBar.SetActive(false);
        //progressCanvas.SetActive(false);
    }

    private void OnDisable()
    {
        underBar.SetActive(true);
        //progressCanvas.SetActive(true);
    }

}
