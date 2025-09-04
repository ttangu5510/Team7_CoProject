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

namespace JYL
{
    public enum ConfirmState { YesOrNoAndConfirm, OnlyConfirm, OnlyYesOrNo }
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
        public IObservable<bool> ConfirmSubject => confirmSubject; // 외부에서는 이거 구독하면 팝업에서 입력받는 정보 알 수 있음.
    
        public void Init(string[] texts, ConfirmState state) // 외부에서 쓰는 함수. 예,아니오만 쓰고 싶으면 false, 확인 결과창 쓰려면 true 
        {
            switch (state)
            {
               case ConfirmState.YesOrNoAndConfirm:
                   mainText.text = texts[0]; // 메인
                   subText.text = texts[1]; // 서브
                   afterText.text = texts[2]; // 결과 확인
                   break;
               
               case ConfirmState.OnlyConfirm:
               case ConfirmState.OnlyYesOrNo:
                   mainText.text = texts[0]; // 메인
                   subText.text = texts[1]; // 서브
                   break;
            }
            SetButtons(state);
        }
    
        private void SetButtons(ConfirmState state)
        {
            switch (state)
            {
                // 예/아니오 => 확인
                case ConfirmState.YesOrNoAndConfirm:
                    // 예 버튼
                    confirmButton.OnClickAsObservable()
                        .Subscribe(_ =>
                        {
                            confirmSubject.OnNext(true); // true 전달
                            confirmSubject.OnCompleted();
                            
                            afterPanel.SetActive(true); // 확인 후 패널 세팅
                            confirmButton.gameObject.SetActive(false);
                            cancelButton.gameObject.SetActive(false);
                            afterConfirmButton.gameObject.SetActive(true);
                        }).AddTo(this);
                    
                    // 아니오 버튼
                    cancelButton.OnClickAsObservable()
                        .Subscribe(_ =>
                        {
                            confirmSubject.OnNext(false); // false 전달
                            confirmSubject.OnCompleted();
                    
                            Destroy(gameObject); // 팝업 파괴
                        }).AddTo(this);
                    
                    // 결과 이후 확인 버튼
                    afterConfirmButton.OnClickAsObservable()
                        .Subscribe(_=>Destroy(gameObject)).AddTo(this); // 확인하면 파괴
                    break;
                
                // 예/아니오만
                case ConfirmState.OnlyConfirm:
                    // 확인 버튼 세팅
                    confirmButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(true);
                    afterConfirmButton.gameObject.SetActive(true);
                    // 확인 버튼 구독
                    afterConfirmButton.OnClickAsObservable()
                        .Subscribe(_=>Destroy(gameObject)).AddTo(this);
                    break;
                
                // 예/아니오만
                case ConfirmState.OnlyYesOrNo:
                    // 예 버튼
                    confirmButton.OnClickAsObservable()
                        .Subscribe(_ =>
                        {
                            confirmSubject.OnNext(true); // true 전달
                            confirmSubject.OnCompleted();
                            Destroy(gameObject);
                        }).AddTo(this);
                    
                    // 아니오 버튼
                    cancelButton.OnClickAsObservable()
                        .Subscribe(_ =>
                        {
                            confirmSubject.OnNext(false); // false 전달
                            confirmSubject.OnCompleted();
                    
                            Destroy(gameObject); // 팝업 파괴
                        }).AddTo(this);
                    break;
            }
        }
    }
}

