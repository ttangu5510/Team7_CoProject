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

        [Header("Need Upgrade UI")]
        [SerializeField] private GameObject needUpgradeSlot;

        [Header("No Available UI")]
        [SerializeField] private GameObject noAvailableSlot;

        private DomAthEntity _ath;

        /// <summary>선수 데이터 바인딩</summary>
        public void ShowAssigned(DomAthEntity ath)
        {
            _ath = ath;
            SetState(true, false, false);

            if (nameText) nameText.text = ath.entityName;
            if (timeLeftText) timeLeftText.text = $"남은 치료일 {GetRemainWeeks(ath)}주";

            // 이미지 연결 필요시
            // if (athImage) athImage.sprite = ath.portraitSprite;
        }

        /// <summary>빈 슬롯</summary>
        public void ShowEmpty()
        {
            _ath = null;
            SetState(false, false, true); // 여기서 "No Available Slot" 켜줌
        }

        /// <summary>시설 업그레이드 필요 슬롯</summary>
        public void ShowLocked()
        {
            _ath = null;
            SetState(false, true, false);
        }

        private void SetState(bool player, bool upgrade, bool noAvailable)
        {
            if (playerSlot) playerSlot.SetActive(player);
            if (needUpgradeSlot) needUpgradeSlot.SetActive(upgrade);
            if (noAvailableSlot) noAvailableSlot.SetActive(noAvailable);
        }

        private int GetRemainWeeks(DomAthEntity ath)
        {
            // 실제 DomAthEntity의 부상 정보 필드 사용
            // 예: ath.injury.RemainWeeks
            return 2; // 기본값: 목업에 맞게 2주
        }
    }
}