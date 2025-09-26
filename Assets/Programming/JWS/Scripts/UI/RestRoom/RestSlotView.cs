using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace JWS
{
    public class RestSlotView : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI")]
        [SerializeField] private GameObject playerSlot;
        [SerializeField] private Image athImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI fatigueText;

        [SerializeField] private GameObject emptySlot;
        [SerializeField] private GameObject needUpgradeSlot;
        [SerializeField] private GameObject noAvailableSlot;

        public IObservable<Unit> Clicked => _clicked;
        private readonly Subject<Unit> _clicked = new();

        private bool _isLocked;
        private bool _isNoAvailable;

        public bool IsLocked => _isLocked;
        public bool IsNoAvailable => _isNoAvailable;

        private void Reset()
        {
            // 루트에 투명 Image로 레이캐스트 받게
            var img = GetComponent<Image>() ?? gameObject.AddComponent<Image>();
            img.raycastTarget = true;
            img.color = new Color(1, 1, 1, 0.001f);
        }

        public void ShowAssigned(JYL.DomAthEntity ath)
        {
            SetState(player:true);
            if (nameText)     nameText.text = ath.entityName;
            if (fatigueText)  fatigueText.text = $"피로도 {ath.stats.fatigue}";
            _isLocked = _isNoAvailable = false;
        }

        public void ShowEmpty()
        {
            SetState(empty:true);
            _isLocked = _isNoAvailable = false;
        }

        public void ShowLocked()
        {
            SetState(needUpgrade:true);
            _isLocked = true; _isNoAvailable = false;
        }

        public void ShowNoAvailable()
        {
            SetState(noAvailable:true);
            _isLocked = false; _isNoAvailable = true;
        }

        private void SetState(bool player=false, bool empty=false, bool needUpgrade=false, bool noAvailable=false)
        {
            if (playerSlot)      playerSlot.SetActive(player);
            if (emptySlot)       emptySlot.SetActive(empty);
            if (needUpgradeSlot) needUpgradeSlot.SetActive(needUpgrade);
            if (noAvailableSlot) noAvailableSlot.SetActive(noAvailable);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_isLocked || _isNoAvailable) return;
            _clicked.OnNext(Unit.Default);
        }
    }
}
