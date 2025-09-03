using JYL;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TitleSceneManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private GameObject openingPanel;
    [SerializeField] private GameObject SaveUIPanel;
    [SerializeField] private GameObject nameInputPopup;

    [Header("Buttons")]
    [SerializeField] private Button beginButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button infoButton;
    [SerializeField] private Button skipLoadingButton;
    [SerializeField] private Button skipOpeningButton;
    
    [Inject] private IUiManager uiManager; // 이거만 추가하면 UI매니저 쓸 수 있음
    
    private void Awake() { }
    private GameObject currentPanel;

    private void Start()
    {
        // 시작 시 Title 패널만 켜두기
        ShowPanel(titlePanel);
        
        beginButton.OnClickAsObservable()
            .Subscribe(_=>OnClickBeginButton())
            .AddTo(this);
        
        continueButton.OnClickAsObservable()
            .Subscribe(_=>OnClickContinueButton())
            .AddTo(this);
        
        infoButton.OnClickAsObservable()
            .Subscribe(_=>OnClickInfoButton())
            .AddTo(this);
        
        skipLoadingButton.OnClickAsObservable()
            .Subscribe(_=>OnClickSkipLoadingButton())
            .AddTo(this);
        
        skipOpeningButton.OnClickAsObservable()
            .Subscribe(_=>OnClickSkipOpeningButton())
            .AddTo(this);
        
    }

    private void ShowPanel(GameObject target)
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
    private void OnClickBeginButton() => ShowPanel(loadingPanel);
    private void OnClickContinueButton() => ShowPanel(SaveUIPanel);
    private void OnClickInfoButton() => ShowPanel(nameInputPopup);
    private void OnClickSkipLoadingButton() => ShowPanel(openingPanel);
    private void OnClickSkipOpeningButton() => ShowPanel(nameInputPopup);
    private void OnClickErrorOk() => ShowPanel(nameInputPopup); // 에러 확인 → 입력창으로
}
