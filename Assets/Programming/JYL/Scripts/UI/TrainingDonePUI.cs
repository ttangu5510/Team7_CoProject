using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TrainingDonePUI : MonoBehaviour
{
    [Header("Set Button")] 
    [SerializeField] private Button okButton;
    
    [Header("Set Text")]
    [SerializeField] private TextMeshProUGUI stateText;

    private Subject<bool> confirmSubject = new();
    public IObservable<bool> ConfirmSubject => confirmSubject;

    public void Init(bool success)
    {
        if (success)
        {
            stateText.text = "훈련 성공";
        }
        else
        {
            stateText.text = "훈련 실패";
        }
        okButton.OnClickAsObservable()
            .Subscribe(_ => OnClickOk())
            .AddTo(this);
    }

    private void OnClickOk()
    {
        Debug.Log("클릭됨");
        okButton.interactable = false;
        confirmSubject.OnNext(true);
        confirmSubject.OnCompleted();
        Destroy(gameObject);
    }
    
}
