using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class FacilityInformationBox : MonoBehaviour
{
    [SerializeField] Button Step0ProceedButton;
    [SerializeField] Button Step1ProceedButton;
    [SerializeField] Button Step2ProceedButton;
    [SerializeField] Button Step3ProceedButton;

    private void Awake()
    {
        //Step0ProceedButton.OnClickAsObservable()
        //    .Subscribe(_ => );
    }

}
