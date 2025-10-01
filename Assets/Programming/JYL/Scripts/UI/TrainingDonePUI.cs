using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TrainingDonePUI : MonoBehaviour
{
    [Header("Set UI")]
    [SerializeField] private Image trainingDoneImage;
    
    [Header("Set Button")] 
    [SerializeField] private Button okButton;
    
    [Header("Set Text")]
    [SerializeField] private TextMeshProUGUI stateText;
    
    [Header("Set Images")]
    [SerializeField] private Sprite successSprite;
    [SerializeField] private Sprite failSprite;

    private Subject<bool> confirmSubject = new();
    public IObservable<bool> ConfirmSubject => confirmSubject;

    public void Init(bool success)
    {
        if (success)
        {
            stateText.text = "훈련 성공";
            trainingDoneImage.sprite = successSprite;
        }
        else
        {
            stateText.text = "훈련 실패";
            trainingDoneImage.sprite = failSprite;
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
