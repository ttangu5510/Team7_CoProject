using JYL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
        [SerializeField] Button arrangementButton;
        [SerializeField] TextMeshProUGUI arrangementButtonText;


        public void Awake()
        {
            arrangementButton.onClick.AddListener(arrangementButtonClick);
        }

        private void arrangementButtonClick()
        {
            // 코치 배치하기
            Debug.Log($"코치 배치버튼 클릭됨");
        }

        
    }
}