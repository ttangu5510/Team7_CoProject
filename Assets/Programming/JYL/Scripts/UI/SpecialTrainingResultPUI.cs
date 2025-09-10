using System;
using System.Collections;
using System.Collections.Generic;
using JYL;
using SJL;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SpecialTrainingResultPUI : MonoBehaviour
{
    [Header("Set Athlete Icons")] 
    [SerializeField] private Image[] athleteIcon;
    [SerializeField] private TextMeshProUGUI[] nameText;
    [SerializeField] private TextMeshProUGUI[] resultText;
    
    [Header("Set Button")]
    [SerializeField] private Button closeButton;

    private string iconPath = "AthleteIcon/";
    
    private Subject<bool> confirmSubject = new();
    public IObservable<bool> ConfirmSubject => confirmSubject;
    private void Awake()
    {
        closeButton.OnClickAsObservable()
            .Subscribe(_ => gameObject.SetActive(false));
    }

    public void SetParameters(int times, Dictionary<DomAthEntity, TrainingType> dict)
    {
        int count = 0;
        foreach (var pair in dict)
        {
            if (pair.Value == TrainingType.Special)
            {
                //athleteIcon[count].sprite = Resources.Load<Sprite>($"{iconPath}{pair.Key.id}");
                nameText[count].text = pair.Key.entityName;
                resultText[count].text = $"전체 능력치 : +{times * 5}";
                count++;
            }
        }

        for (int i = count; count < athleteIcon.Length; i++)
        {
            // athleteIcon[i].gameObject.SetActive(false);
            nameText[count].text = "";
            resultText[count].text = "";
        }
    }
    
}
