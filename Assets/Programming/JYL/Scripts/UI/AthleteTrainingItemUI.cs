using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using SJL;

namespace JYL
{
    public class AthleteTrainingItemUI : MonoBehaviour
    {
        [Header("Athlete Text")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        
        [Header("Assign Button")]
        [SerializeField] private Button assignButton;
        [SerializeField] private TextMeshProUGUI assignText;

        [Header("Athlete Info")] 
        [SerializeField] private AthleteInfoPanel athleteInfoPanel;


        // Init에 의해서 외부에 의해 주입
        private Dictionary<DomAthEntity, TrainingType> trainingDict;
        private DomAthEntity athlete; 
        private TrainingType trainingType;
        private RectTransform infoParent;
        
        // 아이템을 클릭하면 정보가 출력됨
        private Button itemButton;

        private void Awake()
        {
            assignButton.OnClickAsObservable()
                .Subscribe(_ => AssignTraining())
                .AddTo(this);
            
            itemButton = GetComponent<Button>();
            itemButton.OnClickAsObservable()
                .Subscribe(_ => OnClickItemButton())
                .AddTo(this);
            
            trainingType = TrainingType.None;
        }
       
        public void Init(Dictionary<DomAthEntity, TrainingType> dict, TrainingType type, DomAthEntity athlete, RectTransform parent) // 생성될 때 호출
        {
            trainingDict = dict;
            this.athlete = athlete;
            trainingType = type;
            infoParent = parent;

            // 부상당한 선수일 경우, 훈련 비활성화
            if (athlete.curState == AthleteState.Injured)
            {
                nameText.text = $"{this.athlete.entityName}({this.athlete.curAge}) <color=red>(부상)</color>";
            }
            else
            {
                nameText.text = $"{this.athlete.entityName}({this.athlete.curAge})";
            }
            gradeText.text = this.athlete.maxGrade.ToString();
            
            SetItem();
        }

        public void SetItem()
        {
            // 배치 가능한 경우
            if (athlete.curState == AthleteState.Active)
            {
                if (trainingDict[athlete] == TrainingType.None)
                {
                    assignText.text = "배치하기";
                    assignText.color = Color.black;
                    assignButton.GetComponent<Image>().color = Color.cyan;
                }
                else if (trainingDict[athlete] != TrainingType.None)
                {
                    assignText.text = "배치중";
                    assignText.color = Color.black;
                    assignButton.GetComponent<Image>().color = Color.grey;
                }
            }
            // 부상일 경우
            else 
            {
                assignText.text = "배치 불가";
                assignText.color = Color.black;
                assignButton.GetComponent<Image>().color = Color.white;
                assignButton.interactable = false;
            }
        }

        private void AssignTraining() // 버튼을 눌럿을 때 전환
        {
            if (trainingDict[athlete] == TrainingType.None)
            {
                if (trainingDict.Values.Count(t => t == trainingType) >= 4) return; // 4명 이상 배치되었으니 리턴
                trainingDict[athlete] = trainingType; // 배치 가능하면 배치하고 타입을 기록함
            }
            else
            {
                trainingDict[athlete] = TrainingType.None; // 배치 중인 경우, None으로 변경
            }

            SetItem();
        }

        private void OnClickItemButton()
        {
            AthleteInfoPanel athleteInfo = Instantiate(athleteInfoPanel, infoParent);
            athleteInfo.gameObject.SetActive(true);
            athleteInfo.SetInfo(athlete);
        }

    }
}

