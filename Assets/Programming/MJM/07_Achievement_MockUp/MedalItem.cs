using UnityEngine;
using UnityEngine.UI;

public class MedalItem : MonoBehaviour
{
    public Image medalIcon; // UI에서 아이콘으로 보여질 부분
    public Text medalTitle; // 메달 이름

    public void SetData(Sprite icon, string title)
    {
        medalIcon.sprite = icon;
        medalTitle.text = title;
    }
}
