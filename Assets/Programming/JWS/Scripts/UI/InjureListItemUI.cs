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
        [SerializeField] private Button assignButton;        // 배치하기
        [SerializeField] private GameObject assignedBadge;   // 배치됨 표시
        [SerializeField] private Button infoButton;          // 상세 보기
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text injuryLeftText;

        private DomAthEntity _ath;
        private Action<DomAthEntity> _onAssign;
        private Action<DomAthEntity> _onOpenInfo;

        void Awake()
        {
            if (assignButton)
            {
                assignButton.OnClickAsObservable()
                    .Subscribe(_ => _onAssign?.Invoke(_ath))
                    .AddTo(this);
            }

            if (infoButton)
            {
                infoButton.OnClickAsObservable()
                    .Subscribe(_ => _onOpenInfo?.Invoke(_ath))
                    .AddTo(this);
            }
        }

        public void Bind(
            DomAthEntity ath,
            bool isAssigned,
            Action<DomAthEntity> onAssign,
            Action<DomAthEntity> onOpenInfo)
        {
            _ath = ath;
            _onAssign = onAssign;
            _onOpenInfo = onOpenInfo;

            if (nameText)       nameText.text = $"{ath.entityName} ({ath.curAge.Value}세)";
            if (injuryLeftText) injuryLeftText.text = $"남은 치료 턴 {ath.leftInjury}";

            if (assignedBadge) assignedBadge.SetActive(isAssigned);
            if (assignButton)  assignButton.gameObject.SetActive(!isAssigned);
        }
    }
    
}
