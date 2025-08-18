using SJL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{ 
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI typeText;

        public void SetPlayer(Player player)
        {
            nameText.text = player.name;
            gradeText.text = player.grade;
            ageText.text = player.age.ToString();
            typeText.text = player.type;
        }
    }
}