using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace SJL
{
    public class TrainingProgressPUI : MonoBehaviour
    {
        [Header("Set UI")] 
        [SerializeField] private Image progressImage;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Slider progressSlider;

        [Header("Set Timer")]
        [SerializeField] private float progressTime = 5f;
        [SerializeField] private float delayTime = 0.5f;

        [Header("Set Sprite")] 
        [SerializeField] private Sprite[] sprites = new Sprite[3];
        private Animator animator;

        // 이벤트 발행
        private Subject<bool> confirmedSubject = new();
        public IObservable<bool> Confirmed => confirmedSubject;

        // 훈련 진행 중 텍스트 최신화
        private async UniTask<bool> RunProgressText()
        {
            int counter = 0;
            float timer = 0;
            string tmpText = "훈련 진행 중";
            
            // 랜덤 이미지 지정
            int randIndex = Random.Range(0, 2);
            Debug.Log($"랜덤숫자{randIndex}");
            if (progressImage == null)
            {
                Debug.LogWarning("프로그레스 이미지가 널");
            }

            if (sprites == null)
            {
                Debug.LogWarning("스프라이트 리스트가 null");
            }

            if (sprites[randIndex] == null)
            {
                Debug.LogWarning($"해당 인덱스의 스프라이트가 null");
            }
            progressImage.sprite = sprites[randIndex];
            
            // 프로그레스 바 채워지는 로직
            // 다 채워지면 훈련 완료 창 띄움
            while (timer < progressTime)
            {
                StringBuilder dots = new();
                for (int i = 0; i <= counter; i++)
                {
                    dots.Append(".");
                }

                progressText.text = $"{tmpText}{dots.ToString()}";
                counter++;
                if (counter == 3) counter = 0;
                timer += delayTime;
                await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
            }
            return true;
        }

        private async UniTask<bool> RunProgressBar()
        {
            float timer = 0;
            progressSlider.value = 0;

            while (timer < progressTime)
            {
                await UniTask.Yield(PlayerLoopTiming.FixedUpdate);
                timer += Time.fixedDeltaTime;
                progressSlider.value = Mathf.Clamp01(timer / progressTime);
            }

            return true;
        }


        public async UniTaskVoid Init()
        {
            animator = GetComponent<Animator>();
            UniTask<bool> isTextDone = RunProgressText();
            UniTask<bool> isProgressBarDone = RunProgressBar();
            await UniTask.WhenAll(isTextDone, isProgressBarDone);

            confirmedSubject.OnNext(true);
            confirmedSubject.OnCompleted();
            Destroy(gameObject);
        }
    }
}