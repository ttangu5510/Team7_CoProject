using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{    
    [Header("닫기")]
    [SerializeField] private Button closeButton;
    [Header("볼륨 설정")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle masterMuteToggle;
    [SerializeField] private Toggle musicMuteToggle;
    [SerializeField] private Toggle sfxMuteToggle;
    [Header("오디오 믹서")]
    //[SerializeField] private AudioMixer audioMixer;
    [Header("그래픽/데이터")]
    [SerializeField] private TMP_Dropdown graphicsDropdown;
    [SerializeField] private Button resetDataButton;
    [Header("가이드/크레딧")]
    [SerializeField] private Button guideButton;
    [SerializeField] private Button creditButton;
    [Header("버전/하단")]
    [SerializeField] private TextMeshProUGUI versionText;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button toTitleButton;

    private void Awake()
    {
        // 볼륨 슬라이더/토글 이벤트 연결
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
        musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        masterMuteToggle.onValueChanged.AddListener(OnMasterMuteChanged);
        musicMuteToggle.onValueChanged.AddListener(OnMusicMuteChanged);
        sfxMuteToggle.onValueChanged.AddListener(OnSfxMuteChanged);

        // 그래픽 드롭다운
        graphicsDropdown.onValueChanged.AddListener(OnGraphicsChanged);

        // 데이터 리셋
        resetDataButton.onClick.AddListener(OnResetData);

        // 하단 기능
        saveButton.onClick.AddListener(OnSave);
        loadButton.onClick.AddListener(OnLoad);
        toTitleButton.onClick.AddListener(OnGotoTitle);

        // 가이드/크레딧
        guideButton.onClick.AddListener(OnShowGuide);
        creditButton.onClick.AddListener(OnShowCredit);

        // 닫기(X)
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));

        // 버전
        versionText.text = "버전. v1.0.0";

        // 슬라이더 값 초기화(필요 시 저장값에서 불러올 것)
        masterVolumeSlider.value = 50;
        musicVolumeSlider.value = 50;
        sfxVolumeSlider.value = 50;
        UpdateVolume();
    }

    // ---- 볼륨 기능 (AudioMixer 기준) ----
    private void OnMasterVolumeChanged(float value) //전체 볼륨
    {
        // 슬라이더 (0~100) → AudioMixer (-80~0 dB)
        float volume = Mathf.Lerp(-80f, 0f, value / 100f);
        //audioMixer.SetFloat("MasterVolume", volume);
        UpdateVolume();
    }
    private void OnMusicVolumeChanged(float value)  //음악 볼륨
    {
        float volume = Mathf.Lerp(-80f, 0f, value / 100f);
        //audioMixer.SetFloat("MusicVolume", volume);
        UpdateVolume();
    }
    private void OnSfxVolumeChanged(float value)    //효과음 볼륨
    {
        float volume = Mathf.Lerp(-80f, 0f, value / 100f);
        //audioMixer.SetFloat("SfxVolume", volume);
        UpdateVolume();
    }
    private void UpdateVolume() //볼륨 업데이트
    {
        
    }
    private void OnMasterMuteChanged(bool mute) //전체 음소거
    {
        //audioMixer.SetFloat("MasterVolume", mute ? -80f : Mathf.Lerp(-80f, 0f, masterVolumeSlider.value / 100f));
    }
    private void OnMusicMuteChanged(bool mute)  //음악 음소거
    {
        //audioMixer.SetFloat("MusicVolume", mute ? -80f : Mathf.Lerp(-80f, 0f, musicVolumeSlider.value / 100f));
    }
    private void OnSfxMuteChanged(bool mute)    //효과음 음소거
    {
        //audioMixer.SetFloat("SfxVolume", mute ? -80f : Mathf.Lerp(-80f, 0f, sfxVolumeSlider.value / 100f));
    }

    // ---- 그래픽/데이터 ----
    private void OnGraphicsChanged(int idx)
    {
        // ex) 낮음-0, 중간-1, 높음-2
        QualitySettings.SetQualityLevel(idx);
    }
    private void OnResetData()
    {
        // 데이터 초기화 구현 (PlayerPrefs.DeleteAll 등)
        Debug.Log("데이터 리셋됨");
    }

    // ---- 하단 기능 ----
    private void OnSave()
    {
        // 저장 구현
        Debug.Log("수동 저장!");
    }
    private void OnLoad()
    {
        // 불러오기 구현
        Debug.Log("불러오기!");
    }
    private void OnGotoTitle()
    {
        // 타이틀 화면 이동
        Debug.Log("타이틀 화면 이동!");
    }

    // ---- 팝업 ----
    private void OnShowGuide()
    {
        // 가이드 보기
        Debug.Log("가이드 보기!");
    }
    private void OnShowCredit()
    {
        // 크레딧 보기
        Debug.Log("크레딧 보기!");
    }
}
