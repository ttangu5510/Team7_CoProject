using UnityEngine;
using Zenject;

public class TitleSceneManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject openingPanel;
    [SerializeField] private GameObject nameInputPopup;

    [Inject] private UIManager uiManager; // 이거만 추가하면 UI매니저 쓸 수 있음
    
    private GameObject currentPanel;

    private void Start()
    {
        // 시작 시 Title 패널만 켜두기
        ShowPanel(titlePanel);
        
    }

    public void ShowPanel(GameObject target)
    {
        if (currentPanel != null)
            currentPanel.SetActive(false);

        if (target != null)
        {
            target.SetActive(true);
            currentPanel = target;
        }
    }

    // 버튼에서 직접 호출하기 쉽게 래퍼 메서드 준비
    public void OnClickTitleToOpening() => ShowPanel(openingPanel);
    public void OnClickOpeningToLoading() => ShowPanel(loadingPanel);
    public void OnClickOpeningToNameInput() => ShowPanel(nameInputPopup);
    public void OnClickErrorOk() => ShowPanel(nameInputPopup); // 에러 확인 → 입력창으로
}
