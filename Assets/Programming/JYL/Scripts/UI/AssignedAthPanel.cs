using System.Collections.Generic;
using JYL;
using SJL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssignedAthPanel : MonoBehaviour
{
    [Header("Set Image")] 
    [SerializeField] private Image[] athleteImg;

    [Header("Set Text")] 
    [SerializeField] private TextMeshProUGUI[] nameText;

    private string iconPath = $"AthleteIcon/";

    public void UpdateUI(Dictionary<DomAthEntity, TrainingType> dict, TrainingType type)
    {
        int count = 0;
        foreach (var pair in dict)
        {
            if (pair.Value == type)
            {
                Debug.Log($"확인용{pair.Key.entityName}");
                 athleteImg[count].gameObject.SetActive(true);
                // athleteImg[count].sprite = Resources.Load<Sprite>($"{iconPath}{pair.Key.id}");
                nameText[count].text = $"{pair.Key.entityName}";
                count++;
            }
        }

        for (int i = count; i < athleteImg.Length; i++)
        {
            athleteImg[i].gameObject.SetActive(false);
            nameText[i].text = "";
        }
    }
}
