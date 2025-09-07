using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using TMPro;
using JYL;

public class InjuredListPanelView : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField] private Transform listContent; // .../Scroll View/Viewport/Content

    [Header("Prefabs")]
    [SerializeField] private GameObject injuredItemPrefab; // Injured Athlete Item.prefab

    [Inject] private DomAthService _ath; // 읽기 전용

    public void Render()
    {
        var injured = _ath.GetAllRecruitedAthleteList()      // <-- 수정: 메서드 사용
            .Where(a => a.curState == AthleteState.Injured)
            .ToList();

        Rebuild(listContent);

        foreach (var e in injured)
        {
            var go = Instantiate(injuredItemPrefab, listContent, false);

            var nameText    = go.transform.Find("Ath NameText")?.GetComponent<TMP_Text>();
            var fatigueText = go.transform.Find("Fatigue Text")?.GetComponent<TMP_Text>();

            if (nameText)    nameText.text    = e.entityName;
            if (fatigueText) fatigueText.text = $"{(e.stats != null ? e.stats.fatigue : 0)}%";
        }
    }

    private void Rebuild(Transform parent)
    {
        if (!parent) return;
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) DestroyImmediate(parent.GetChild(i).gameObject);
            else Destroy(parent.GetChild(i).gameObject);
#else
            Destroy(parent.GetChild(i).gameObject);
#endif
        }
    }
}