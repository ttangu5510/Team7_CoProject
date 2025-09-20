using System.Collections.Generic;
using JYL;
using UnityEngine;
using Zenject;

public class CoachRosterScreenMMJ : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private Transform content;          // ScrollView Content
    [SerializeField] private CoachItemMMJ itemPrefab;    // 아이템 프리팹
    [SerializeField] private CoachInfoPanelMMJ infoPanel;

    [Inject] private CoachService coachService;

    private readonly List<CoachEntity> roster = new();

    private void OnEnable()
    {
        roster.Clear();
        roster.AddRange(coachService.GetRecruitedCoaches());
        Redraw();
    }

    private void Redraw()
    {
        foreach (Transform c in content) Destroy(c.gameObject);

        foreach (var coach in roster)
        {
            var item = Instantiate(itemPrefab, content);

            int yearsToRetire = coach.retireAge - coach.curAge.Value;
            if (yearsToRetire < 0) yearsToRetire = 0;

            item.Bind(coach, yearsToRetire, a =>
            {
                infoPanel.SetInfo(a);

                infoPanel.OnFired -= OnCoachFired;
                infoPanel.OnFired += OnCoachFired;
            });
        }
    }

    private void OnCoachFired(CoachEntity fired)
    {
        roster.Remove(fired);
        Redraw();
    }
}
