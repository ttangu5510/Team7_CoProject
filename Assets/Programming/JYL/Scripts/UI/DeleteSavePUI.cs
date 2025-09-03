using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JWS;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace JYL
{
    public class DeleteSavePUI : MonoBehaviour
    {
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button yesButton;

        [SerializeField] private GameObject confirmPanel;
        // 이벤트를 직접 발행
        private Subject<bool> confirmSubject = new();
        public IObservable<bool> ConfirmSubject => confirmSubject;
        
        private void Awake()
        {
            cancelButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    confirmSubject.OnNext(false);
                    confirmSubject.OnCompleted();
                    Destroy(gameObject);
                })
                .AddTo(this);
            
            yesButton.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    confirmSubject.OnNext(true);
                    confirmSubject.OnCompleted();
                    confirmPanel.SetActive(true);
                    confirmButton.gameObject.SetActive(true);
                    yesButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(false);
                })
                .AddTo(this);
            
            confirmButton.OnClickAsObservable()
                .Subscribe(_ => Destroy(gameObject))
                .AddTo(this);
        }
    }
}

