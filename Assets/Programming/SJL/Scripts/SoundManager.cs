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
            // 시작시 기본 볼륨 설정
            //SetMasterVolume(0.5f);
            //SetMusicVolume(0.5f);
            //SetSFXVolume(0.5f);

            // AudioSource는 Inspector에서 할당! (중복 할당 주의)
            //musicSource = GetComponent<AudioSource>();
            //sfxSource = GetComponent<AudioSource>();
            //PlayMusic(currentMusicIndex); // 인덱스 번호로 배경음 재생
        }


        //Mixer 파라미터 → dB 변환식 활용
        public void SetMasterVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f; // -80 ~ 0dB
            audioMixer.SetFloat("MasterVolume", dB);
            Debug.Log($"SetMasterVolume: value={sliderValue}, dB={dB}");
        }
        public void SetMusicVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat("MusicVolume", dB); 
            Debug.Log($"SetMusicVolume: value={sliderValue}, dB={dB}");
        }
        public void SetSFXVolume(float sliderValue)
        {
            float dB = Mathf.Log10(Mathf.Max(sliderValue, 0.0001f)) * 20f;
            audioMixer.SetFloat("SfxVolume", dB);
            Debug.Log($"SetSfxVolume: value={sliderValue}, dB={dB}");
        }

        // 배경음 변경/재생
        public void PlayMusic(int index)
        {
            if (index < 0 || index >= musicClips.Length) return;
            musicSource.clip = musicClips[index];
            musicSource.loop = true;
            musicSource.Play();
            currentMusicIndex = index;
        }

        // 효과음 재생
        public void PlaySFX(int index)
        {
            if (index < 0 || index >= sfxClips.Length) return;
            sfxSource.PlayOneShot(sfxClips[index]);
        }


    }
}