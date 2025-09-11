using JYL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Zenject;

namespace SJL
{
    public class CoachItem : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image coachImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI abilityText;
        [SerializeField] private TextMeshProUGUI ageText;
        
        [Header("Buttons")]
        [SerializeField] public Button assignButton;
        [SerializeField] TextMeshProUGUI assignText;

        private string iconPath = "CoachIcon/";

        public void Init(CoachEntity entity, bool isAssigned)
        {
            // coachImage.sprite = Resources.Load<Sprite>($"{iconPath}{entity.id}");
            nameText.text = entity.entityName;
            abilityText.text = $"피로도 감소: -{(int)entity.grade}";
            ageText.text = $"({entity.curAge})";

            UpdateButton(isAssigned);
        }

        public void UpdateButton(bool isAssigned)
        {
            if (isAssigned)
            {
                assignText.text = "배치 중";
                assignButton.gameObject.GetComponent<UISquircle>().color = Color.grey;
            }
            else
            {
                assignText.text = "배치 하기";
                assignButton.gameObject.GetComponent<UISquircle>().color = Color.cyan;
            }
        }
    }
}