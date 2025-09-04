using UnityEngine;
using TMPro;

[ExecuteAlways]
public class StatBar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] TMP_Text label;
    [SerializeField] RectTransform fill;   // Fill의 RectTransform
    [SerializeField] TMP_Text valueText;   // 선택

    [Header("Inspector")]
    [SerializeField] string labelText = "체력";
    [Range(0,100)] [SerializeField] int percent = 0;

    void OnEnable()   { SetupFill(); Apply(percent / 100f); }
    void OnValidate() { SetupFill(); Apply(percent / 100f); }

    void SetupFill()
    {
        if (!fill) return;
        // Fill을 ‘왼쪽에서 오른쪽으로 커지는 바’로 강제 초기화
        fill.anchorMin = new Vector2(0f, 0f);
        fill.anchorMax = new Vector2(0f, 1f);   // 0% 시작
        fill.pivot     = new Vector2(0f, 0.5f);
        fill.offsetMin = Vector2.zero;
        fill.offsetMax = Vector2.zero;
    }

    public void Set(string name, int pct)
    {
        labelText = name;
        percent   = Mathf.Clamp(pct, 0, 100);
        Apply(percent / 100f);
    }

    void Apply(float p01)
    {
        if (label)     label.text = labelText;
        if (valueText) valueText.text = Mathf.RoundToInt(p01 * 100f).ToString();

        if (!fill) return;
        fill.anchorMax = new Vector2(Mathf.Clamp01(p01), 1f); // 0~1 비율로 채움
    }
}