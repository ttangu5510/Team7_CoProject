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
            SetMasterVolume(1f);
            SetMusicVolume(1f);
            SetSFXVolume(1f);

            musicSource = GetComponent<AudioSource>();
            sfxSource = GetComponent<AudioSource>();
            PlayMusic(1); // 인덱스 0번 배경음 재생
        }

        // Mixer 파라미터 → dB 변환식 활용
        public void SetMasterVolume(float value)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
        }
        public void SetMusicVolume(float value)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
        }
        public void SetSFXVolume(float value)
        {
            audioMixer.SetFloat("SfxVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20f);
        }

        // 배경음 변경/재생
        public void PlayMusic(int index)
        {
            if (index < 0 || index >= musicClips.Length) return;
            musicSource.clip = musicClips[index];
            musicSource.loop = true;
            musicSource.Play();
        }

        // 효과음 재생
        public void PlaySFX(int index)
        {
            if (index < 0 || index >= sfxClips.Length) return;
            sfxSource.PlayOneShot(sfxClips[index]);
        }


    }
}