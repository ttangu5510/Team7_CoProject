using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SJL
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [Header("Mixer 및 소스")]
        [SerializeField] private AudioMixer audioMixer; // Exposed된 Mixer
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [Header("클립들")]
        [SerializeField] private AudioClip[] musicClips; // 배경음 모음
        [SerializeField] private AudioClip[] sfxClips;   // 효과음 모음

        [SerializeField] public int currentMusicIndex = 0; // 현재 재생중인 배경음 인덱스

        private string masterVolumeKey = "masterVolume";
        private string musicVolumeKey = "musicVolume";
        private string sfxVolumeKey = "sfxVolume";

        private float masterVolumeValue;
        private float musicVolumeValue;
        private float sfxVolumeValue;

        private void Awake()
        {
            // 싱글톤 패턴(중복 방지)
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            // 저장된 볼륨 불러오기
            LoadVolumeSettings();

            // 시작시 배경음 재생
            PlayMusic(0); 

        }

        public void Update()
        {
           if(Input.GetMouseButtonDown(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                PlaySFX(0); // 효과음 재생 테스트
            }
        }


        //Mixer 파라미터 → dB 변환식 활용
        public void SetMasterVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;  // 0.0001f 방지
            audioMixer.SetFloat(masterVolumeKey, dB);    // Mixer에 적용
            PlayerPrefs.SetFloat(masterVolumeKey, sliderValue);  // 값 저장
            masterVolumeValue = sliderValue;
            Debug.Log($"SetMasterVolume: value={sliderValue}, dB={dB}"); 
        }
        public void SetMusicVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat(musicVolumeKey, dB);
            PlayerPrefs.SetFloat(musicVolumeKey, sliderValue); // 저장
            musicVolumeValue = sliderValue;
            Debug.Log($"SetMusicVolume: value={sliderValue}, dB={dB}");
        }
        public void SetSFXVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat(sfxVolumeKey, dB);
            PlayerPrefs.SetFloat(sfxVolumeKey, sliderValue); // 저장
            sfxVolumeValue = sliderValue;
            Debug.Log($"SetSfxVolume: value={sliderValue}, dB={dB}");
        }

        // ---------------- 저장된 값 불러오기 ----------------
        private void LoadVolumeSettings()
        {
            float master = PlayerPrefs.GetFloat(masterVolumeKey, 0.5f);
            float music = PlayerPrefs.GetFloat(musicVolumeKey, 0.5f);
            float sfx = PlayerPrefs.GetFloat(sfxVolumeKey, 0.5f);
            Debug.Log($"LoadVolumeSettings: master={master}, music={music}, sfx={sfx}");
            SetMasterVolume(master);
            SetMusicVolume(music);
            SetSFXVolume(sfx);
        }

        // 배경음 변경/재생
        public void PlayMusic(int index)
        {
            if (index < 0 || index >= musicClips.Length) return;    // 인덱스 범위 체크
            musicSource.clip = musicClips[index];   // 클립 교체
            musicSource.loop = true;    // 반복 재생 설정
            musicSource.Play(); // 재생
            currentMusicIndex = index;  // 현재 인덱스 저장
        }

        // 효과음 재생
        public void PlaySFX(int index)
        {
            if (index < 0 || index >= sfxClips.Length) return;
            sfxSource.PlayOneShot(sfxClips[index]);
        }

        // 현재 볼륨 값 반환 (0~1)
        public float GetMasterVolume()
        {
            return masterVolumeValue;
        }
        public float GetMusicVolume()
        {
            return musicVolumeValue;
        }
        public float GetSFXVolume()
        {
            return sfxVolumeValue;
        }



    }
}