using UnityEngine;
using UnityEngine.UI;

public class MedalDetailPopup : MonoBehaviour
{
    public Text headerText;   // "경기 기록"
    public Image icon;
    public Text titleText;    // "국제 피겨스케이팅 대회"
    public Text recordText;   // "경기 기록 : 1위"
    public Text dateText;     // "메달 획득일 : 25.08.11"
    public Button closeButton;

    void Awake()
    {
        if (closeButton) closeButton.onClick.AddListener(() => Destroy(gameObject));
    }

    public void Open(MedalData d)
    {
        if (headerText) headerText.text = "경기 기록";
        if (icon) icon.sprite = d.icon;
        if (titleText) titleText.text = d.unlocked ? d.title : "???";
        if (recordText) recordText.text = $"경기 기록 : {(d.unlocked ? d.recordText : "-")}";
        if (dateText) dateText.text = $"메달 획득일 : {(d.unlocked ? d.acquiredDate : "-")}";
        gameObject.SetActive(true);
    }
}
