using System;
using System.Collections;
using System.Collections.Generic;
using JYL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace JWS
{
    public class InjureAthInfoPanel : MonoBehaviour
    {
        [SerializeField] private Button closeButton; // X
        [SerializeField] private Image profileImage; // 옵션
        [SerializeField] private TMP_Text outlineText;
        [SerializeField] private StatBar[] statBars; // 7개

        private readonly Subject<Unit> _onClosed = new();
        public IObservable<Unit> OnClosed => _onClosed;

        private CompositeDisposable _cd;

        private void Awake()
        {
            if (closeButton)
            {
                closeButton.OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        gameObject.SetActive(false); // 자기 자신만 비활성
                        _onClosed.OnNext(Unit.Default); // 닫힘 신호
                    })
                    .AddTo(this); // 파괴 시 자동 해제
            }
        }

        public void Open(DomAthEntity ath)
        {
            // 덮기
            transform.SetAsLastSibling();
            gameObject.SetActive(true);

            if (outlineText)
            {
                outlineText.text =
                    $"{ath.entityName} ({ath.curAge.Value}세)\n" +
                    $"등급: {ath.affiliation}\n" +
                    $"최대 성장: {ath.maxGrade}";
            }

            if (statBars != null && statBars.Length >= 7)
            {
                statBars[0].Set("체력", ath.stats.health, 800);
                statBars[1].Set("순발력", ath.stats.quickness, 800);
                statBars[2].Set("유연성", ath.stats.flexibility, 800);
                statBars[3].Set("기술", ath.stats.technic, 800);
                statBars[4].Set("속도", ath.stats.speed, 800);
                statBars[5].Set("균형감각", ath.stats.balance, 800);
                statBars[6].Set("피로도", ath.stats.fatigue, 100);
            }
        }
    }
}

