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
        [Header("Athlete Info")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        
        [Header("Assign Button")]
        [SerializeField] private Button assignButton;
        [SerializeField] private TextMeshProUGUI assignText;

        // Init에 의해서 외부에 의해 주입
        private Dictionary<DomAthEntity, TrainingType> trainingDict;
        private DomAthEntity athlete; 
        private TrainingType trainingType;
        
        private void Awake()
        {
            assignButton.OnClickAsObservable()
                .Subscribe(_ => AssignTraining())
                .AddTo(this);
        }

        public void Init(Dictionary<DomAthEntity, TrainingType> dict, TrainingType type, DomAthEntity athlete) // 생성될 때 호출
        {
            trainingDict = dict;
            this.athlete = athlete;
            trainingType = type;
            
            nameText.text = this.athlete.entityName;
            gradeText.text = this.athlete.maxGrade.ToString();
            
            SetItem();

        }

        private void SetItem()
        {
            if (trainingDict[athlete] == TrainingType.None)
            {
                assignText.text = "훈련 배치";
                assignText.color = Color.white;
                assignButton.GetComponent<Image>().color = Color.gray;
            }
            else
            {
                assignText.text = "배치 완료";
                assignText.color = Color.black;
                assignButton.GetComponent<Image>().color = Color.green;
            }
        }

        private void AssignTraining() // 버튼을 눌럿을 때 전환
        {
            if (trainingDict[athlete] == TrainingType.None)
            {
                if (trainingDict.Values.Count(t => t == trainingType) >= 4) return; // 4명 이상 배치되었으니 리턴
                trainingDict[athlete] = trainingType;
            }
            else
            {
                trainingDict[athlete] = TrainingType.None;
            }

            SetItem();
        }

    }
}

