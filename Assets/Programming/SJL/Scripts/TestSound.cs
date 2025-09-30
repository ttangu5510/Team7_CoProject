using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSound : MonoBehaviour
{
    [SerializeField] Button musicButton;
    [SerializeField] Button sfxButton;

    [SerializeField] public int currentIndex = 0; // 현재 재생중인 인덱스

    private void Start()
    {
        musicButton.onClick.AddListener(() => SJL.SoundManager.Instance.PlayMusic(currentIndex));
        sfxButton.onClick.AddListener(() => SJL.SoundManager.Instance.PlaySFX(currentIndex));
    }
}
