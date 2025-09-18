using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace SJL
{
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
        [SerializeField] private Button returnTitleButton;
        [Header("오디오 믹서")]
        [SerializeField] private AudioMixer audioMixer;

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
            returnTitleButton.onClick.AddListener(OnGotoTitle);

            // 가이드/크레딧
            guideButton.onClick.AddListener(OnShowGuide);
            creditButton.onClick.AddListener(OnShowCredit);

            // 닫기(X)
            closeButton.onClick.AddListener(() => gameObject.SetActive(false));

            // 버전
            versionText.text = "버전. v1.0.0";

            // 슬라이더 값 초기화(필요 시 저장값에서 불러올 것)
            //masterVolumeSlider.value = 50;
            //musicVolumeSlider.value = 50;
            //sfxVolumeSlider.value = 50;
        }

        public void Start()
        {
            SoundManager.Instance.PlayMusic(0); // 시작시 배경음 재생
        }

        // 볼륨 슬라이더 연동 (AudioMixer Exposed Parameters와 연동 권장)
        private void OnMasterVolumeChanged(float value)
        {
            masterVolumeSlider.onValueChanged.AddListener(val => SoundManager.Instance.SetMasterVolume(val));
        }
        private void OnMusicVolumeChanged(float value)
        {
            musicVolumeSlider.onValueChanged.AddListener(val => SoundManager.Instance.SetMusicVolume(val));
        }
        private void OnSfxVolumeChanged(float value)
        {
            sfxVolumeSlider.onValueChanged.AddListener(val => SoundManager.Instance.SetSFXVolume(val));
        }
        private void OnMasterMuteChanged(bool mute)
        {
            audioMixer.SetFloat("MasterVolume", mute ? -80f : Mathf.Log10(Mathf.Max(masterVolumeSlider.value, 0.0001f)) * 20f);
        }
        private void OnMusicMuteChanged(bool mute)
        {
            audioMixer.SetFloat("MusicVolume", mute ? -80f : Mathf.Log10(Mathf.Max(musicVolumeSlider.value, 0.0001f)) * 20f);
        }
        private void OnSfxMuteChanged(bool mute)
        {
            audioMixer.SetFloat("SfxVolume", mute ? -80f : Mathf.Log10(Mathf.Max(sfxVolumeSlider.value, 0.0001f)) * 20f);
        }

        // ---- 그래픽/데이터 ----
        private void OnGraphicsChanged(int idx)
        {
            // ex) 낮음-0, 중간-1, 높음-2
            QualitySettings.SetQualityLevel(idx);
        }
        private void OnResetData()
        {
            // 데이터 초기화
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
}