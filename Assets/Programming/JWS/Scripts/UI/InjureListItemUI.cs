using System;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JYL;

namespace JWS
{
    public class InjureListItemUI : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Button assignButton;      // "배치하기"
        [SerializeField] private GameObject assignedButton;// "배치중"(비활성 표시용)
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI fatigueText;

        private DomAthEntity _ath;
        private Action<DomAthEntity> _onAssign;

        /// <param name="isAssigned">이미 치료실에 배치된 선수인가?</param>
        public void Bind(DomAthEntity ath, bool isAssigned, Action<DomAthEntity> onAssign)
        {
            _ath = ath;
            _onAssign = onAssign;

            if (nameText)     nameText.text = $"{ath.entityName} ({ath.curAge.Value}세)";
            if (fatigueText)  fatigueText.text = $"남은 치료 턴 {ath.leftInjury}";

            ToggleAssigned(isAssigned);

            assignButton.onClick.RemoveAllListeners();
            assignButton.onClick.AddListener(() => _onAssign?.Invoke(_ath));
        }

        public void ToggleAssigned(bool assigned)
        {
            // assigned=true  → "배치중" 표시 / "배치하기" 숨김
            if (assignedButton) assignedButton.SetActive(assigned);
            if (assignButton)   assignButton.gameObject.SetActive(!assigned);
            if (assignButton)   assignButton.interactable = !assigned;
        }
    }
    
}
