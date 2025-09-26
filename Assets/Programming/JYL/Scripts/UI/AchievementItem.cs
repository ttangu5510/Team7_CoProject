using System;
using JWS;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace JYL
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AchievementItem : MonoBehaviour
    {
        [Header("Set UIs")] 
        [SerializeField] private Image trophyIcon;
        [SerializeField] private Image inCompleteImg;
        [SerializeField] private Button canCompleteButton;
        [SerializeField] private Image completedImg;
        [SerializeField] private Slider slider;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        [Header("Set Trophy")] 
        [SerializeField] private Sprite locked;
        [SerializeField] private Sprite transparentTrophy;
        [SerializeField] private Sprite bronze;
        [SerializeField] private Sprite silver;
        [SerializeField] private Sprite gold;
        [SerializeField] private Sprite platinum;
        
        private CanvasGroup canvasGroup;
        private AchievementController controller;
        private AchievementRewardPUI rewardUI;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        
        // 초기화
        public void Init(AchievementController achieveController ,AchievementRewardPUI popUpUi)
        {
            // 참조 초기화
            rewardUI = popUpUi;
            controller = achieveController;
            
            // 히든 업적의 경우, CanComplete 전환 전까지는 아이템으로 표시하지 않음.
            if ((achieveController.achieve.State == AchievementState.Hidden) && 
                (achieveController.state.Value is not (AchievementState.CanComplete or AchievementState.Completed)))
            {
                return;
            }
            
            // 업적 내용 표기
            titleText.text = achieveController.achieve.AchName;
            descriptionText.text = achieveController.achieve.AchDescription;

            // 이벤트 구독. 상태 변화 시 UI 업데이트함.
            controller.state
                .Skip(1)
                .DistinctUntilChanged()
                .Subscribe(_ => UpdateUI())
                .AddTo(this);
            
            // UI 최신화
            UpdateUI();
        }

        // 업적 UI 업데이트에 사용.
        public void UpdateUI()
        {
            // 버튼과 슬라이더의 값 설정
            inCompleteImg.gameObject.SetActive(controller.state.Value == AchievementState.Unlocked);
            canCompleteButton.gameObject.SetActive(controller.state.Value == AchievementState.CanComplete);
            completedImg.gameObject.SetActive(controller.state.Value == AchievementState.Completed);
            slider.gameObject.SetActive(controller.state.Value is not AchievementState.Locked);
            
            // 업적의 상태에 따라 스프라이트 이미지 변경
            if (controller.state.Value == AchievementState.Locked)
            {
                trophyIcon.sprite = locked;
            }
            else if (controller.state.Value == AchievementState.Unlocked)
            {
                trophyIcon.sprite =transparentTrophy;
            }
            else
            {
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

            // 업적의 상태에 따라 여러 로직 처리 수행
            switch (controller.state.Value)
            {
                case AchievementState.Unlocked:
                    slider.value = Mathf.Clamp01((float)controller.progress.Value / controller.achieve.CompleteNumber);
                    break;
                case AchievementState.CanComplete:
                    canCompleteButton.OnClickAsObservable()
                        .Subscribe(_ => CompleteProcess(controller))
                        .AddTo(this);
                    slider.value = 1f;
                    break;
                case AchievementState.Completed:
                    canvasGroup.alpha = 0.7f;
                    slider.value = 1f;
                    break;
            }
        }

        private void CompleteProcess(AchievementController achieveController)
        {
            achieveController.state.Value = AchievementState.Completed; // 완료 상태로 변경
            rewardUI.gameObject.SetActive(true);
            rewardUI.Init(achieveController);
            _ = rewardUI.ClosePopUp();
            // TODO : 여기서 업적 완료에 대한 이벤트 발행 후, 패널 쪽에서
            // 후행 업적을 찾아서 UpdateUI 해줘야 함.
        }
    } 
}

