using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JYL;

namespace JWS
{
    public class RestAthleteItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI fatigueText;
        [SerializeField] private Button assignButton;
        [SerializeField] private Button unassignButton;

        private DomAthEntity _ath;

        private readonly Subject<DomAthEntity> _onAssign   = new();
        private readonly Subject<DomAthEntity> _onUnassign = new();

        public IObservable<DomAthEntity> OnAssign   => _onAssign;
        public IObservable<DomAthEntity> OnUnassign => _onUnassign;

        private void Awake()
        {
            assignButton?.onClick.AddListener(() => _onAssign.OnNext(_ath));
            unassignButton?.onClick.AddListener(() => _onUnassign.OnNext(_ath));
        }

        public void Bind(DomAthEntity ath, bool assigned)
        {
            _ath = ath;
            if (nameText)    nameText.text = ath.entityName;
            if (fatigueText) fatigueText.text = $"Fatigue {ath.fatigue}";
            SetAssigned(assigned);
        }

        public void SetAssigned(bool on)
        {
            if (assignButton)   assignButton.gameObject.SetActive(!on);
            if (unassignButton) unassignButton.gameObject.SetActive(on);
        }

        public void NudgeAssignButton()
        {
            // 가벼운 피드백(필요시 트윈/애니 추가)
            if (assignButton) assignButton.transform.SetAsLastSibling();
        }
    }
}