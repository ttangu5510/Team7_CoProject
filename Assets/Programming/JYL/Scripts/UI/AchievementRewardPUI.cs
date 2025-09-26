using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Cysharp.Threading.Tasks;

namespace JYL
{
    public class AchievementRewardPUI : MonoBehaviour
    {

        [Header("Set UI")] 
        [SerializeField] private Image trophyIcon;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        
        [Header("Set Trophies")]
        [SerializeField] private Sprite bronze;
        [SerializeField] private Sprite silver;
        [SerializeField] private Sprite gold;
        [SerializeField] private Sprite platinum;

        private UIAnimator uiAnimator;

        private float timer = 2.5f; // 팝업이 꺼지는 시간

        private void Awake()
        {
            uiAnimator = GetComponent<UIAnimator>();
        }

        private void OnEnable() => uiAnimator.PlayIn();
        
        // 외부에서 Init으로 내용 채움
        public void Init(AchievementController controller)
        {
            // 업적의 이름, 설명 표기
            titleText.text = controller.achieve.AchName;
            descriptionText.text = controller.achieve.AchDescription;
            
            // 보상 수준에 따라 트로피 아이콘 지정
            switch (controller.achieve.Reward)
            {
                case AchievementReward.브론즈:
                    trophyIcon.sprite = bronze;
                    break;
                case AchievementReward.실버:
                    trophyIcon.sprite = silver;
                    break;
                case AchievementReward.골드:
                    trophyIcon.sprite = gold;
                    break;
                case AchievementReward.플래티넘:
                    trophyIcon.sprite = platinum;
                    break;
            }
        }

        public async UniTaskVoid ClosePopUp()
        {
            await UniTask.WaitForSeconds(timer);
            uiAnimator.PlayOut();
            await UniTask.WaitForSeconds(uiAnimator.outDuration);
            gameObject.SetActive(false);
        }
    }
}

