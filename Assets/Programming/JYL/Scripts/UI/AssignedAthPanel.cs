using System.Collections.Generic;
using JYL;
using SJL;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AssignedAthPanel : MonoBehaviour
{
    [Header("Set Image")] 
    [SerializeField] private Image[] athleteImg;

    [Header("Set Text")] 
    [SerializeField] private TextMeshProUGUI[] nameText;


    public void UpdateUI(Dictionary<DomAthEntity, TrainingType> dict, TrainingType type)
    {
        int count = 0;
        foreach (var pair in dict)
        {
            if (pair.Value == type)
            {
                athleteImg[count].gameObject.SetActive(true);
                var handle = Addressables.LoadAssetAsync<Sprite>($"ImageAssets/character profile/ID순/{pair.Key.id}.png");
                var count1 = count;
                handle.Completed += h =>
                {
                    athleteImg[count1].sprite = h.Result;
                };
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
