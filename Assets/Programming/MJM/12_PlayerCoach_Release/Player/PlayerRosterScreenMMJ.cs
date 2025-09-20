using System.Collections.Generic;
using JYL;
using SJL;
using UnityEngine;
using Zenject;

namespace MMJ
{
    public class PlayerRosterScreenMMJ : MonoBehaviour
    {
        [Header("UI Refs")]
        [SerializeField] private Transform content;
        [SerializeField] private PlayerItemMMJ itemPrefab;
        [SerializeField] private PlayerInfoPanelMMJ infoPanel;

        [Inject] private JYL.DomAthService domAthService;

        private readonly List<DomAthEntity> roster = new();

        private void OnEnable()
        {
            roster.Clear();
            roster.AddRange(domAthService.GetAllRecruitedAthleteList());
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform c in content) Destroy(c.gameObject);

            foreach (var ath in roster)
            {
                var item = Instantiate(itemPrefab, content);

                int currentAge = ath.curAge.Value;
                int yearsToRetire = ath.retireAge - currentAge;
                if (yearsToRetire < 0) yearsToRetire = 0;

                item.Bind(
                    athlete: ath,
                    starCount: (int)ath.maxGrade,
                    yearsToRetire: yearsToRetire,
                    selected: false,
                    portraitResolver: _ => null,
                    onDetail: a =>
                    {
                        infoPanel.SetInfo(a);

                        infoPanel.OnFired -= OnAthleteFired;
                        infoPanel.OnFired += OnAthleteFired;
                    }
                );
            }
        }

        private void OnAthleteFired(DomAthEntity fired)
        {
            roster.Remove(fired);
            Redraw();
        }
    }
}
