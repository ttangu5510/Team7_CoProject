using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loadingWindow : MonoBehaviour
{
    [SerializeField] private Slider trainingSlider; 
    [SerializeField] private float fillDuration = 3f; // 슬라이드가 다 차는 데 걸리는 시간(초)

    public void StartTraining() // 이 메서드는 훈련 시작 버튼에 연결됩니다.
    {
        StartCoroutine(FillSliderOverTime()); //슬라이더가 천천히 차오릅니다
    }

    private IEnumerator FillSliderOverTime()
    {
        trainingSlider.value = 0f;
        float elapsed = 0f;

        while (elapsed < fillDuration)
        {
            elapsed += Time.deltaTime;
            trainingSlider.value = Mathf.Clamp01(elapsed / fillDuration);
            yield return null;
        }

        trainingSlider.value = 1f;
    }
}
