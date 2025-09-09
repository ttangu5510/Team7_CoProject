using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JYL;
using SJL;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine.Serialization;
using Zenject;

namespace SJL
{
    public class AthleteListPanel : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] Button closeButton;
        [SerializeField] private Button applyButton;
        [SerializeField] private Button resetButton;

        [Header("Set Content Transform")] 
        [SerializeField] private RectTransform popUpParent;
        [SerializeField] Transform parentContent;
        
        [Header("Set Training Box")]
        [SerializeField] TrainingBox trainingBox;

        [Header("Set Prefabs")] 
        [SerializeField] private AthleteTrainingItemUI athleteItem;

        [Inject] private DomAthService athleteService;
        
        private TrainingType trainingType; // 입력받은 훈련 타입
        private List<DomAthEntity> list = new(); // 훈련 가능한 전체 선수
        
        private Subject<bool> closeSubject = new(); // 이벤트 발행
        public IObservable<bool> CloseSubject => closeSubject; // 이벤트 발행

        private Dictionary<DomAthEntity, TrainingType> cachedDict = new();
        public Dictionary<DomAthEntity, TrainingType> changeDict = new();
        
        // 리셋 버튼을 위한 아이템들 캐싱
        private List<AthleteTrainingItemUI> itemList = new();

        public void Awake()
        {
            applyButton.OnClickAsObservable()
                .Subscribe(_ => OnClickApplyButton()).AddTo(this);
            
            resetButton.OnClickAsObservable()
                .Subscribe(_ => OnClickResetButton()).AddTo(this);
            
            closeButton.OnClickAsObservable()
                .Subscribe(_ => OnClickCloseButton()).AddTo(this);
        }

        //  패널이 켜지고 초기화 작업
        public void SelectTrainingAthlete(List<DomAthEntity> trainingAthlete, TrainingType type, Dictionary<DomAthEntity, TrainingType> dict)
        {
            // 리스트에 캐싱
            list = trainingAthlete;
                
            // 전해받은 딕셔너리 캐싱해놓기. 
            cachedDict = dict;
            changeDict.Clear();
            
            // 기존에 생성했던 아이템들 파괴
            foreach (var item in itemList)
            {
                Destroy(item.gameObject);
            }

            itemList.Clear();
            
            trainingType = type; // 입력받은 훈련 타입 최신화

            foreach (var ath in list) // 리스트의 선수들 숫자만큼 아이템 생성
            {
                if (cachedDict[ath] == TrainingType.None || cachedDict[ath] == type)
                {
                    changeDict[ath] = cachedDict[ath];
                    AthleteTrainingItemUI tmpItem = Instantiate(athleteItem, parentContent);
                    tmpItem.Init(changeDict, type, ath, popUpParent);
                    itemList.Add(tmpItem);
                }
            }
        }

        private void OnClickApplyButton()
        {
            // 변경 사항 적용함
            foreach (var key in changeDict.Keys)
            {
                cachedDict[key] = changeDict[key];
            }
            
            closeSubject.OnNext(true);
            // closeSubject.OnCompleted(); // 스트림 제거
            
            gameObject.SetActive(false);
        }

        // 리셋 버튼 누를 시
        private void OnClickResetButton()
        {
            // 현재 배치 중인 선수들을 리셋
            foreach (var key in cachedDict.Keys.ToList())
            {
                if(cachedDict[key] == trainingType) changeDict[key] = TrainingType.None;
            }
            
            // UI 최신화
            foreach (var item in itemList)
            {
                item.SetItem();
            }
        }

        private void OnClickCloseButton()
        {
            // 변경사항 적용 없이 패널 종료
            gameObject.SetActive(false);
        }
    }


}

