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
    public IObservable<bool> ConfirmSubject => confirmSubject; // 외부에서는 이거 구독하면 팝업에서 입력받는 정보 알 수 있음.

    public void Init(string[] texts, bool hasAfter)
    {
        mainText.text = texts[0];
        subText.text = texts[1];
        
        if (hasAfter)
        {
            afterText.text = texts[2];
        }
        
        SetButtons(hasAfter);
        
    }

    private void SetButtons(bool hasAfter)
    {
        // 예 버튼
        confirmButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                confirmSubject.OnNext(true); // true 전달
                confirmSubject.OnCompleted();
                
                if (hasAfter) // 확인 후 결과창이 있을 경우
                {
                    afterPanel.SetActive(true); // 확인 후 패널 세팅
                    confirmButton.gameObject.SetActive(false);
                    cancelButton.gameObject.SetActive(false);
                    afterConfirmButton.gameObject.SetActive(true);
                }
                else //아닐 경우
                {
                    Destroy(gameObject);
                }
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
        if (hasAfter)
        {
            afterConfirmButton.OnClickAsObservable()
                .Subscribe(_=>Destroy(gameObject)).AddTo(this); // 확인하면 파괴
        }
    }
}
