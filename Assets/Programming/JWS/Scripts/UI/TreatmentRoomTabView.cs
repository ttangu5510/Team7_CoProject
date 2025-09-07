using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using TMPro;
using JYL;
using SHG; // FacilityTable 접근

public class TreatmentRoomTabView : MonoBehaviour
{
    [Header("Hierarchy")]
    [SerializeField] private Transform slotPanel;  // .../InfoView/Athlete Assignment Panel/Slot Panel

    [Header("Prefabs")]
    [SerializeField] private GameObject playerSlotPrefab;      // Player Slot.prefab
    [SerializeField] private GameObject noAvailableSlotPrefab; // No Available Player Slot.prefab
    [SerializeField] private GameObject needUpgradeSlotPrefab; // Need Upgrade Slot.prefab
    [SerializeField] private GameObject emptySlotPrefab;       // (선택)

    [Header("Config")]
    [SerializeField] private int totalSlots = 8; // Slot A~H

    [Inject] private DomAthService _ath; 
    [Inject] private ISaveManager _save;

    public void Render(IReadOnlyList<DomAthEntity> assignCandidatesOverride = null)
    {
        var recruited = _ath.GetAllRecruitedAthleteList(); // <-- 수정: 메서드 사용
        var assignCandidates = assignCandidatesOverride ?? recruited;

        int capacity = GetCapacityFromSave();
        capacity = Mathf.Clamp(capacity, 0, totalSlots);

        Rebuild(slotPanel);

        for (int i = 0; i < totalSlots; i++)
        {
            bool hasPlayer = (assignCandidates != null && i < assignCandidates.Count && assignCandidates[i] != null);
            GameObject prefab =
                (i >= capacity)              ? needUpgradeSlotPrefab :
                hasPlayer                    ? playerSlotPrefab :
                (noAvailableSlotPrefab != null ? noAvailableSlotPrefab : emptySlotPrefab);

            var go = Instantiate(prefab, slotPanel, false);

            // Player Slot일 때만 간단 바인딩
            if (hasPlayer && go != null)
            {
                var e = assignCandidates[i];
                var nameText = go.transform.Find("Ath Name Text")?.GetComponent<TMP_Text>();
                var timeText = go.transform.Find("Ath TimeLeft Text")?.GetComponent<TMP_Text>();
                if (nameText) nameText.text = e.entityName;
                if (timeText) timeText.text = (e.curState == AthleteState.Injured) ? $"{e.leftInjury}턴 남음" : "정상";
            }
        }
    }

    private int GetCapacityFromSave()
    {
        var save = _save.GetCurrentSave(); // <-- 수정: 메서드 사용
        if (save == null || save.buildings == null || save.buildings.Count == 0)
            return FacilityTable.MedicalCenter.NumberOfAthletes[0];

        // buildingId가 시설 이름(예: "의료 센터")로 저장됨. 방어적으로 몇 가지 키워드도 체크.
        var medical = save.buildings.FirstOrDefault(b =>
            b.buildingId == FacilityTable.MedicalCenter.Name ||          // "의료 센터"
            b.buildingId == "의료 센터" ||
            b.buildingId == "Medical Center" ||
            b.buildingId == "MedicalCenter");

        int stage = Mathf.Clamp(medical?.level ?? 0, 0, FacilityTable.MedicalCenter.MAX_UPGRADED_STAGE);
        // NumberOfAthletes 길이는 MAX_UPGRADED_STAGE+1 가정
        stage = Mathf.Clamp(stage, 0, FacilityTable.MedicalCenter.NumberOfAthletes.Length - 1);
        return FacilityTable.MedicalCenter.NumberOfAthletes[stage];
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
