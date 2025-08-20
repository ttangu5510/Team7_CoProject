using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestToast : MonoBehaviour
{
    [SerializeField] private GameObject popupToastPrefab;
    [SerializeField] private Transform popupsParentTransform;

    public void ShowToast()
    {
        var toast = Instantiate(popupToastPrefab, popupsParentTransform);
        toast.GetComponent<Toast>().SetText("This is Just Toast!");
        UIManager.Instance.ShowPopup(toast);
    }
}
