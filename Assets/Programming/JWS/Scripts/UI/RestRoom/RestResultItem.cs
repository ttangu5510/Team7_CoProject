using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RestResultItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI fatigueText;

    public void Bind(Sprite portrait, string name, int reduced)
    {
        if (image) image.sprite = portrait;
        if (nameText) nameText.text = name;
        if (fatigueText) fatigueText.text = $"피로도 -{reduced}";
    }
}
