using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PopupCloseSelf : MonoBehaviour
{
    [Tooltip("비워두면 자동으로 가장 가까운 팝업 루트를 찾습니다.")]
    [SerializeField] private GameObject popupRoot;

    void Awake()
    {
        if (!popupRoot)
            // 팝업 루트 추정: 가장 가까운 Canvas(또는 자기 루트)
            popupRoot = GetComponentInParent<Canvas>()?.gameObject ?? gameObject;

        var btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (UIManager.Instance != null && popupRoot != null)
                UIManager.Instance.CloseSpecificPopup(popupRoot);
        });
    }
}