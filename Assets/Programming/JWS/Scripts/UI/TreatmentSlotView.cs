using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using JYL;

namespace JWS
{
public class TreatmentSlotView : MonoBehaviour
    {
        [Header("Click")]
        [SerializeField] private Button clickArea;   // ★ Slot 루트에 단 Button

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

        public IObservable<Unit> Clicked => _clicked;   // 외부에서 구독
        private readonly Subject<Unit> _clicked = new Subject<Unit>();

        private bool _isLocked;
        private bool _isNoAvailable;

        private void Awake()
        {
            if (clickArea != null)
                clickArea.onClick.AddListener(() => _clicked.OnNext(Unit.Default));
        }

        public void ShowAssigned(DomAthEntity ath)
        {
            SetState(player:true);
            if (nameText)     nameText.text = ath.entityName;
            if (timeLeftText) timeLeftText.text = $"남은 치료 턴 {ath.leftInjury}";
            SetInteractable(true);
            _isLocked = _isNoAvailable = false;
        }

        public void ShowEmpty()
        {
            SetState(empty:true);
            SetInteractable(true);            // 빈 슬롯은 클릭 가능 (배치 패널 열기)
            _isLocked = _isNoAvailable = false;
        }

        public void ShowLocked()
        {
            SetState(needUpgrade:true);
            SetInteractable(false);           // 잠금은 클릭 불가
            _isLocked = true; _isNoAvailable = false;
        }

        public void ShowNoAvailable()
        {
            SetState(noAvailable:true);
            SetInteractable(false);           // 배치 가능한 선수 없음 → 비활성
            _isLocked = false; _isNoAvailable = true;
        }

        public bool IsLocked => _isLocked;
        public bool IsNoAvailable => _isNoAvailable;

        private void SetState(bool player=false, bool empty=false, bool needUpgrade=false, bool noAvailable=false)
        {
            if (playerSlot)      playerSlot.SetActive(player);
            if (emptySlot)       emptySlot.SetActive(empty);
            if (needUpgradeSlot) needUpgradeSlot.SetActive(needUpgrade);
            if (noAvailableSlot) noAvailableSlot.SetActive(noAvailable);
        }

        private void SetInteractable(bool on)
        {
            if (clickArea) clickArea.interactable = on;
        }
    }
}