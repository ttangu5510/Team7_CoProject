using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject popupRoot; // Popup.Test 루트

    private void Reset()
    {
        // 자동으로 자기 부모 중 Panel 찾아 넣기 (선택)
        popupRoot = transform.root.gameObject;
    }

    public void Close()
    {
        UIManager.Instance.CloseSpecificPopup(popupRoot);
    }
}
