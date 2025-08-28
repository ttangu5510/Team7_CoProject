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
    public class PlayerListInformation : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] Button closeButton;
        [SerializeField] private Button applyButton;
        
        [Header("Set Content Transform")]
        [SerializeField] Transform parentContent;
        
        [Header("Set Training Box")]
        [SerializeField] TrainingBox trainingBox;

        [Header("Set Prefabs")] 
        [SerializeField] private AthleteTrainingItemUI athleteItem;

        [Inject] private DomAthService athleteService;
        
        private TrainingType trainingType; // 입력받은 훈련 타입
        private List<DomAthEntity> list = new(); // 훈련 가능한 전체 선수

        public void Awake()
        {
            closeButton.OnClickAsObservable()
                .Subscribe(_ => gameObject.SetActive(false));
            applyButton.OnClickAsObservable()
                .Subscribe(_ => OnClickApplyButton());
        }

        //  패널이 켜지고 초기화 작업
        public void SelectTrainingAthlete(List<DomAthEntity> trainingAthlete, TrainingType type, Dictionary<DomAthEntity, TrainingType> dict)
        {
            foreach (Transform item in parentContent)
            {
                Destroy(item.gameObject);
            }
            
            list.Clear(); // 기존 리스트 클리어
            trainingType = type; // 입력받은 훈련 타입 최신화
            
            list = athleteService.GetAllRecruitedAthleteList() // 현재 훈련 가능한 선수 중, 배치되지 않은 선수들만 리스트 화
                .Where(ath => ath.curState == AthleteState.Active && (dict[ath] == TrainingType.None||dict[ath] == trainingType))
                .ToList();

            foreach (var ath in list) // 리스트의 선수들 숫자만큼 아이템 생성
            {
                athleteItem = Instantiate(athleteItem, parentContent);
                athleteItem.Init(dict, type, ath);
            }
        }

        private void OnClickApplyButton()
        {
            
        }
    }


}

