using System;
using UnityEngine;
using UnityEngine.UI;

public class MedalItem : MonoBehaviour
{
    public Button clickArea;
    public Image medalIcon;
    public Text medalTitle;
    public GameObject lockMask; // 잠금시 덮개(옵션)

    private MedalData data;
    private Action<MedalData> onClick;

    public void Bind(MedalData data, Action<MedalData> onClick)
    {
        this.data = data;
        this.onClick = onClick;

        medalIcon.sprite = data.icon;
        medalTitle.text = data.unlocked ? data.title : "???";
        if (lockMask) lockMask.SetActive(!data.unlocked);

        clickArea.onClick.RemoveAllListeners();
        clickArea.onClick.AddListener(() => this.onClick?.Invoke(this.data));
    }
}
