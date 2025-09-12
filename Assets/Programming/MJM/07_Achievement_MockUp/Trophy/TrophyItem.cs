using UnityEngine;
using UnityEngine.UI;

public class TrophyItem : MonoBehaviour
{
    public Image trophyIcon; // UI에서 아이콘으로 보여질 부분
    public Text trophyTitle; // 메달 이름

    public void SetData(Sprite icon, string title)
    {
        trophyIcon.sprite = icon;
        trophyTitle.text = title;
    }
}
