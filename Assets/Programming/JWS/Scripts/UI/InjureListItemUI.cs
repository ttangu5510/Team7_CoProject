using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using JYL;

namespace JWS
{
    public class InjureListItemUI : MonoBehaviour
    {
         [Header("Root Button (아이템 전체)")]
        [SerializeField] private Button rootButton;           // 아이템 전체 클릭 → 상세보기

        [Header("Sub Buttons")]
        [SerializeField] private Button assignButton;         // 배치하기
        [SerializeField] private Button assignedButton;       // 배치됨 표시(토글/표시용)

        [Header("Texts & Image")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI fatigueText;
        [SerializeField] private Image profileImage;          // (옵션)

        private DomAthEntity _ath;

        // 이벤트
        private readonly Subject<DomAthEntity> _onAssign = new();
        public IObservable<DomAthEntity> OnAssign => _onAssign;

        private readonly Subject<DomAthEntity> _onOpenInfo = new();
        public IObservable<DomAthEntity> OnOpenInfo => _onOpenInfo;

        void Awake()
        {
            if (rootButton)
                rootButton.OnClickAsObservable()
                    .TakeUntilDestroy(this)
                    .Subscribe(_ => { if (_ath != null) _onOpenInfo.OnNext(_ath); })
                    .AddTo(this);

            if (assignButton)
                assignButton.OnClickAsObservable()
                    .TakeUntilDestroy(this)
                    .Subscribe(_ => { if (_ath != null) _onAssign.OnNext(_ath); })
                    .AddTo(this);

            if (assignedButton)
                assignedButton.interactable = false; // 표시 전용
        }

        public void Bind(DomAthEntity ath, bool isAssigned)
        {
            _ath = ath;

            if (nameText)       nameText.text = $"{ath.entityName} ({ath.curAge.Value}세)";
            if (fatigueText) fatigueText.text = $"남은 치료 턴 {ath.leftInjury}";

            // 프로필 이미지 있으면 여기서 설정
            // if (profileImage) profileImage.sprite = ...

            if (assignButton)   assignButton.gameObject.SetActive(!isAssigned);
            if (assignedButton) assignedButton.gameObject.SetActive(isAssigned);
        }

        public void SetAssigned(bool assigned)
        {
            if (assignButton)   assignButton.gameObject.SetActive(!assigned);
            if (assignedButton) assignedButton.gameObject.SetActive(assigned);
        }
    }
}
