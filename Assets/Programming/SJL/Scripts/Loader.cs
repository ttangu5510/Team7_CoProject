using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.HDROutputUtils;

public class Loader : MonoBehaviour
{
    public Image loadingAnimImg;    // 애니메이션 이미지
    public Sprite[] loadingSprites, endSprites; // 애니메이션 스프라이트 배열
    public Button guideButton; // 가이드 버튼
    public TextMeshProUGUI guideButtonText;   // 가이드 텍스트
    public TextMeshProUGUI percentText; // 로딩 퍼센트 텍스트
    public Slider percentSlider;   // 로딩 바
    public float animationInterval = 1f;    // 애니메이션 변경 간격

    float timer = 0f;   // 타이머
    int spriteIdx = 0;  // 현재 스프라이트 인덱스
    AsyncOperation operation;
    //float progress = operation.progress; // 0~1 (현재 로딩 진행률)
    float progress = 0f;    // 0~1 (현재 로딩 진행률)
    bool loading = true;    // 로딩 중 여부
    private int guideIndex = 0; // 현재 가이드 텍스트 인덱스

    public void Start()
    {
        guideButton.onClick.AddListener(ShowNextGuide); // 버튼 클릭 이벤트 등록
        guideButtonText.text = guideTexts[guideIndex];  // 초기 가이드 텍스트 설정
        // 초기화
        percentSlider.value = 0f;   // 로딩 바 초기값
        percentText.text = "로딩 0%"; // 로딩 텍스트 초기값
        if (loadingSprites.Length > 0)      // 애니메이션 스프라이트가 있으면 첫 번째로 이미지 설정
            loadingAnimImg.sprite = loadingSprites[0];  // 초기 이미지 설정
        //StartCoroutine(LoadSceneCoroutine());
    }

    //IEnumerator LoadSceneCoroutine()    // 씬 로딩 코루틴
    //{
    //    operation = SceneManager.LoadSceneAsync("JYL_MainScene");
    //    operation.allowSceneActivation = false;

    //    while (operation.progress < 0.9f)
    //    {
    //        progress = operation.progress; // operation으로부터 진행률을 읽어 온다
    //        yield return null;
    //    }
    //    // 0.9 ~ 1.0은 직접 보간 (UI만)
    //    float t = 0f;
    //    while (progress < 1f)
    //    {
    //        t += Time.deltaTime;
    //        progress = Mathf.Lerp(0.9f, 1f, t / 0.5f); // 0.5초간 0.9~1.0으로
    //        yield return null;
    //    }
    //    progress = 1f;

    //    // End image 등 처리
    //    loading = false;
    //    if (endSprites != null && endSprites.Length > 0)
    //        loadingAnimImg.sprite = endSprites[0];
    //    percentSlider.value = 1f;
    //    percentText.text = "로딩 100%";

    //    // 실제 씬 활성화
    //    operation.allowSceneActivation = true;
    //}

    public void Update()
    {
        if (loading)
        {
            //진행률이 5초 동안 100%로 상승 (실제 사용시에는 씬 로딩 등과 연동)
            if (progress < 1f)
                progress += Time.deltaTime / 3f; // 5초간 0→1

            // 로딩 바, 텍스트 업데이트
            percentSlider.value = progress;
            percentText.text = $"로딩 {(int)(progress * 100)}%";

            // 1초마다 애니메이션 스프라이트 변경
            timer += Time.deltaTime;
            if (timer > animationInterval && loadingSprites.Length > 0)
            {
                timer = 0f;
                spriteIdx = (spriteIdx + 1) % loadingSprites.Length;
                loadingAnimImg.sprite = loadingSprites[spriteIdx];
            }

            // 로딩 완료 처리 예시
            if (progress >= 1f)
            {
                loading = false;
                // 완료 스프라이트 중 첫 번째로 이미지 변경
                if (endSprites != null && endSprites.Length > 0)
                    loadingAnimImg.sprite = endSprites[0];
                percentSlider.value = 1f;
                percentText.text = "로딩 100%";
            }
        }
    }

    public void ShowNextGuide()
    {
        Debug.Log("가이드 버튼 클릭됨");
        guideIndex = (guideIndex + 1) % guideTexts.Length;
        guideButtonText.text = guideTexts[guideIndex];
    }

    private string[] guideTexts = {
    "1+1 은 귀요미",
    "2+2 도 귀요미",
    "3+3 은 6 이야. 뭘 기대했어?"
    };

}
