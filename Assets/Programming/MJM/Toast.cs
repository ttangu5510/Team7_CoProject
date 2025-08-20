using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Toast : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float lifeTime = 1.8f;

    // 활성화시 즉시 코루틴 실행
    private void OnEnable()
    {
        StartCoroutine(AutoClose());
    }

    // 텍스트 내용 변경
    public void SetText(string msg)
    {
        if (text) text.text = msg;
    }

    // 자동 사라짐 코루틴
    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(lifeTime);
        if (UIManager.Instance) UIManager.Instance.CloseSpecificPopup(gameObject);
        else gameObject.SetActive(false);
    }
}
