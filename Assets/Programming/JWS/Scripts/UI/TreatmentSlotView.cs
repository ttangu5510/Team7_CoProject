using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JYL;

namespace JWS
{
    public class TreatmentSlotView : MonoBehaviour
    {
        [Header("Player Slot UI")]
        [SerializeField] private GameObject playerSlot;
        [SerializeField] private Image athImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI timeLeftText;

        [Header("Empty Slot")]
        [SerializeField] private GameObject emptySlot;
        
        [Header("Need Upgrade Slot")]
        [SerializeField] private GameObject needUpgradeSlot;

        [Header("No Available Slot")]
        [SerializeField] private GameObject noAvailableSlot;

        private DomAthEntity _ath;

        /// <summary>선수 데이터 바인딩</summary>
        public void ShowAssigned(DomAthEntity ath)
        {
            _ath = ath;
            SetState(player:true);

            if (nameText)     nameText.text = ath.entityName;
            if (timeLeftText) timeLeftText.text = $"남은 치료 턴 {ath.leftInjury}";
            // 이미지는 나중에 확인
            // if (athImage)  athImage.sprite = ...;
        }


        public void ShowEmpty()
        {
            _ath = null;
            SetState(empty:true);
        }

        public void ShowLocked()
        {
            _ath = null;
            SetState(needUpgrade:true);
        }

        public void ShowNoAvailable()
        {
            _ath = null;
            SetState(noAvailable:true);
        }

        private void SetState(bool player=false, bool empty=false, bool needUpgrade=false, bool noAvailable=false)
        {
            if (playerSlot)       playerSlot.SetActive(player);
            if (emptySlot)        emptySlot.SetActive(empty);
            if (needUpgradeSlot)  needUpgradeSlot.SetActive(needUpgrade);
            if (noAvailableSlot)  noAvailableSlot.SetActive(noAvailable);
        }

        private int GetRemainWeeks(DomAthEntity ath)
        {
            // TODO: 실제 부상 데이터로 교체 (예: ath.Injury.RemainWeeks)
            return 2;
        }
    }
}