using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JWS
{
    public class TreatmentSlotView : MonoBehaviour, IPointerClickHandler
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

        public IObservable<Unit> Clicked => _clicked;
        private readonly Subject<Unit> _clicked = new Subject<Unit>();

        private bool _isLocked;
        private bool _isNoAvailable;

        public bool IsLocked => _isLocked;
        public bool IsNoAvailable => _isNoAvailable;

        private Image _rootImg; // 루트 레이캐스트 수신자

        private void Awake()
        {
            // 루트에 Graphic 보장 (IPointerClickHandler가 레이캐스트 받도록)
            _rootImg = GetComponent<Image>();
            if (_rootImg == null)
            {
                _rootImg = gameObject.AddComponent<Image>();
                _rootImg.color = new Color(1f, 1f, 1f, 0.001f); // 사실상 투명
            }
            _rootImg.raycastTarget = true;
        }

        public void ShowAssigned(JYL.DomAthEntity ath)
        {
            SetState(player: true);
            if (nameText)     nameText.text = ath.entityName;
            if (timeLeftText) timeLeftText.text = $"남은 치료 턴 {ath.leftInjury}";
            SetInteractable(true);
            _isLocked = _isNoAvailable = false;
        }

        public void ShowEmpty()
        {
            SetState(empty: true);
            SetInteractable(true);               // 빈 슬롯은 클릭 가능
            _isLocked = _isNoAvailable = false;
        }

        public void ShowLocked()
        {
            SetState(needUpgrade: true);
            SetInteractable(false);              // 잠금은 클릭 불가
            _isLocked = true;  _isNoAvailable = false;
        }

        public void ShowNoAvailable()
        {
            SetState(noAvailable: true);
            SetInteractable(false);              // 후보 없음 → 비활성
            _isLocked = false; _isNoAvailable = true;
        }

        private void SetState(bool player = false, bool empty = false, bool needUpgrade = false, bool noAvailable = false)
        {
            if (playerSlot)      playerSlot.SetActive(player);
            if (emptySlot)       emptySlot.SetActive(empty);
            if (needUpgradeSlot) needUpgradeSlot.SetActive(needUpgrade);
            if (noAvailableSlot) noAvailableSlot.SetActive(noAvailable);
        }

        private void SetInteractable(bool on)
        {
            // 루트만 레이캐스트 받도록 고정 (Down/Up 새는 것 방지)
            if (_rootImg) _rootImg.raycastTarget = on;

            var graphics = GetComponentsInChildren<Graphic>(true);
            foreach (var g in graphics)
            {
                if (g == _rootImg) continue; // 루트는 위에서 설정
                g.raycastTarget = false;     // 자식 그래픽은 모두 차단
            }
        }

        // 버튼 없이 루트에서 직접 클릭 처리
        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isLocked || _isNoAvailable) return;
            _clicked.OnNext(Unit.Default);
        }
    }
}
