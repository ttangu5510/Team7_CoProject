using SJL;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class PlayerInformation : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI gradeText;
        [SerializeField] private TextMeshProUGUI ageText;
        [SerializeField] private TextMeshProUGUI typeText;

        [SerializeField] private Slider staminaSlider;
        [SerializeField] private Slider ThirstSlider;
        [SerializeField] private Slider MentalitySlider;

        public void SetPlayer(Player player)
        {
            nameText.text = player.name;
            gradeText.text = player.grade;
            ageText.text = player.age.ToString();
            typeText.text = player.type;
            staminaSlider.value = player.stamina;

        }


    }
}