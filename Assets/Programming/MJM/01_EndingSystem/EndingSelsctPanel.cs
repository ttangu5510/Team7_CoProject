using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndingSelectPanel : MonoBehaviour
{
    [SerializeField] TMP_Text txtGameTitle;
    [SerializeField] Button btnCredits;
    [SerializeField] Button btnContinue;
    [SerializeField] Button btnRestart;

    public System.Action onCredits;
    public System.Action onContinue;
    public System.Action onRestart;

    void Awake()
    {
        if (btnCredits) btnCredits.onClick.AddListener(() => onCredits?.Invoke());
        if (btnContinue) btnContinue.onClick.AddListener(() => onContinue?.Invoke());
        if (btnRestart) btnRestart.onClick.AddListener(() => onRestart?.Invoke());
    }

    public void SetTitle(string nameKo)
    {
        if (txtGameTitle) txtGameTitle.text = nameKo;
    }
}
