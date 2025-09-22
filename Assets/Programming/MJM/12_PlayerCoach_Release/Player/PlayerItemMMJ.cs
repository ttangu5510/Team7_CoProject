using System;
using JYL;
using MMJ;
using SJL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemMMJ : MonoBehaviour
{
    [Header("Athlete Text")]
    [SerializeField] private TextMeshProUGUI nameText;        // ex) 민만준 (27) ★★★
    [SerializeField] private TextMeshProUGUI retireCountText; // ex) 은퇴까지 6년

    [Header("Button")]
    [SerializeField] private Button informationButton;        // 선수 정보 열기 버튼

    [Header("Image")]
    [SerializeField] private Image athleteImage;              // 선수 이미지

    // 내부 상태
    private DomAthEntity model;
    private Action<DomAthEntity> onDetail;
    private Func<DomAthEntity, Sprite> portraitResolver;

    /// <summary>
    /// 리스트 바인딩용 API (RosterScreenMMJ.Redraw에서 호출)
    /// Redraw에서 사용하는 시그니처와 정확히 일치시켰습니다.
    /// </summary>
    public void Bind(
     DomAthEntity athlete,
     int starCount,   // 여기 들어오는 값은 (int)ath.maxGrade
     int yearsToRetire,
     bool selected,
     Func<DomAthEntity, Sprite> portraitResolver = null,
     Action<DomAthEntity> onClick = null,
     Action<DomAthEntity, bool> onSelectChanged = null,
     Action<DomAthEntity> onDetail = null
 )
    {
        model = athlete;
        this.onDetail = onDetail;
        this.portraitResolver = portraitResolver;

        int curAge = athlete.curAge.Value;

        // 이름 + 나이 + 별 개수
        nameText.text = $"{athlete.entityName} ({curAge}) ★{starCount}";

        retireCountText.text = yearsToRetire > 0
            ? $"은퇴까지 {yearsToRetire}년"
            : "은퇴 예정";

        if (athleteImage && portraitResolver != null)
        {
            var spr = portraitResolver.Invoke(athlete);
            if (spr != null) athleteImage.sprite = spr;
        }

        informationButton.onClick.RemoveAllListeners();
        informationButton.onClick.AddListener(() =>
        {
            Debug.Log($"onDetail{this.onDetail == null} / inClick {onClick == null}");
            this.onDetail?.Invoke(model);
           
            // onClick?.Invoke(model);
        });
    }
}
