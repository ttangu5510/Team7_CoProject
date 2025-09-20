using JYL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CoachItemMMJ : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI retireText;
    [SerializeField] private Button infoButton;

    [Header("Image")]
    [SerializeField] private Image coachImage;              


    private CoachEntity model;
    private Action<CoachEntity> onDetail;

    public void Bind(
    CoachEntity coach,
    Action<CoachEntity> onDetail,
    Func<CoachEntity, Sprite> portraitResolver = null
)
    {
        model = coach;
        this.onDetail = onDetail;

        nameText.text = $"{coach.entityName} ({coach.curAge.Value}세)";
        int yearsToRetire = coach.retireAge - coach.curAge.Value;
        retireText.text = yearsToRetire > 0 ? $"은퇴까지 {yearsToRetire}년" : "은퇴 예정";

        if (coachImage && portraitResolver != null)
        {
            var sprite = portraitResolver.Invoke(coach);
            if (sprite != null) coachImage.sprite = sprite;
        }

        infoButton.onClick.RemoveAllListeners();
        infoButton.onClick.AddListener(() => this.onDetail?.Invoke(model));
    }
}
