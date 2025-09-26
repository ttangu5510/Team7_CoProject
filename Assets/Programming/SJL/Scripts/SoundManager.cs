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

            // AudioSource는 Inspector에서 할당! (중복 할당 주의)
            //musicSource = GetComponent<AudioSource>();
            //sfxSource = GetComponent<AudioSource>();
            //PlayMusic(currentMusicIndex); // 인덱스 번호로 배경음 재생
        }


        //Mixer 파라미터 → dB 변환식 활용
        public void SetMasterVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;  // 0.0001f 방지
            audioMixer.SetFloat("MasterVolume", dB);    // Mixer에 적용
            PlayerPrefs.SetFloat("MasterVolume", sliderValue);  // 값 저장
            Debug.Log($"SetMasterVolume: value={sliderValue}, dB={dB}"); 
        }
        public void SetMusicVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat("MusicVolume", dB);
            PlayerPrefs.SetFloat("MusicVolume", sliderValue); // 저장
            Debug.Log($"SetMusicVolume: value={sliderValue}, dB={dB}");
        }
        public void SetSFXVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat("SfxVolume", dB);
            PlayerPrefs.SetFloat("SfxVolume", sliderValue); // 저장
            Debug.Log($"SetSfxVolume: value={sliderValue}, dB={dB}");
        }

        // ---------------- 저장된 값 불러오기 ----------------
        private void LoadVolumeSettings()
        {
            float master = PlayerPrefs.GetFloat("MasterVolume", 0.5f);
            float music = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            float sfx = PlayerPrefs.GetFloat("SfxVolume", 0.5f);

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


    }
}