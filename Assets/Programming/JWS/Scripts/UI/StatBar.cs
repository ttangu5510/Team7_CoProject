using UnityEngine;
using TMPro;

[ExecuteAlways]
public class StatBar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TMP_Text label;
    [SerializeField] RectTransform fill;   // BarContainer 자식
    [SerializeField] TMP_Text valueText;   // 우측 숫자(현재 값 표시)

    [Header("Inspector")]
    [SerializeField] string labelText = "체력";
    [Min(0)] [SerializeField] int currentValue = 0;
    [Min(1)] [SerializeField] int maxValue = 100;

    void OnEnable()   { SetupFill(); Apply(); }
    void OnValidate() { SetupFill(); Apply(); }

    void SetupFill()
    {
        if (!fill) return;
        fill.anchorMin = new Vector2(0f, 0f);
        fill.anchorMax = new Vector2(0f, 1f);   // 0%
        fill.pivot     = new Vector2(0f, 0.5f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
    }

    public void Set(string name, int value, int max)
    {
        labelText    = name;
        maxValue     = Mathf.Max(1, max);
        currentValue = Mathf.Clamp(value, 0, maxValue);
        Apply();
    }

    void Apply()
    {
        if (label) label.text = labelText;

        maxValue     = Mathf.Max(1, maxValue);
        currentValue = Mathf.Clamp(currentValue, 0, maxValue);

        float ratio = (float)currentValue / maxValue;   // 0~1
        if (fill) fill.anchorMax = new Vector2(ratio, 1f);

        // ✅ 현재 값 숫자 반영
        if (valueText) valueText.text = currentValue.ToString();
    }
}