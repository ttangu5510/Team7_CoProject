using JYL;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CoachRosterScreenMMJ : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform content;          // ScrollView Content
    [SerializeField] private CoachItemMMJ itemPrefab;    // 코치 아이템 프리팹
    [SerializeField] private CoachInfoPanelMMJ infoPanel; // 미리 하이어라키에 둔 패널

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
            item.Bind(coach, a =>
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
