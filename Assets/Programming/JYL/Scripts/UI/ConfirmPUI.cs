using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using JYL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ConfirmPUI : MonoBehaviour
{
    [Header("Set After Panel")] 
    [SerializeField] private GameObject afterPanel;
    
    [Header("Set Text")]
    [SerializeField] private TextMeshProUGUI mainText;
    [SerializeField] private TextMeshProUGUI subText;
    [SerializeField] private TextMeshProUGUI afterText;
    
    [Header("Set Buttons")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button afterConfirmButton;
    
    // 이벤트 직접 발행
    private Subject<bool> confirmSubject = new();
    public IObservable<bool> ConfirmSubject => confirmSubject; // 외부에서는 이거 구독하면됨

    private void Awake()
    {
        confirmButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                confirmSubject.OnNext(true);
                confirmSubject.OnCompleted();
                
                afterPanel.SetActive(true);
                confirmButton.gameObject.SetActive(false);
                cancelButton.gameObject.SetActive(false);
                afterConfirmButton.gameObject.SetActive(true);
            }).AddTo(this);
        
        cancelButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                confirmSubject.OnNext(false);
                confirmSubject.OnCompleted();
                Destroy(gameObject);
            }).AddTo(this);
        
        afterConfirmButton.OnClickAsObservable()
            .Subscribe(_=>Destroy(gameObject)).AddTo(this);
    }
}
