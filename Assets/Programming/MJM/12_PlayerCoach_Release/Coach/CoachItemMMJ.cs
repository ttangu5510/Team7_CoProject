using JYL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CoachItemMMJ : MonoBehaviour
{
    [Header("Coach Text")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI retireCountText;

    [Header("Button")]
    [SerializeField] private Button infoButton;

    private CoachEntity model;
    private Action<CoachEntity> onDetail;

    public void Bind(CoachEntity coach, int yearsToRetire, Action<CoachEntity> onDetail)
    {
        model = coach;
        this.onDetail = onDetail;

        nameText.text = $"{coach.entityName} ({coach.curAge.Value}세)";
        retireCountText.text = yearsToRetire > 0 ? $"은퇴까지 {yearsToRetire}년" : "은퇴 예정";

        infoButton.onClick.RemoveAllListeners();
        infoButton.onClick.AddListener(() => this.onDetail?.Invoke(model));
    }
}
