using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CoachInfo : MonoBehaviour
{
    [SerializeField] Image coachImage;
    [SerializeField] TextMeshProUGUI coachName;
    [SerializeField] TextMeshProUGUI coachRetireLeftCount;
    [SerializeField] Button closeButton;
    [SerializeField] Button fireButton;

    [SerializeField] GameObject coachFirePanel;
    void Start()
    {
        closeButton.onClick.AddListener(ClosePanel);
        fireButton.onClick.AddListener(FireCoach);
    }

    void ClosePanel()
    {
        gameObject.SetActive(false); // 패널 닫기
    }

    void FireCoach()
    {
        coachFirePanel.SetActive(true);
        Debug.Log($"{coachName.text} 코치를 해고했습니다.");
        
        // 추가적인 로직 구현 필요 (예: 데이터 변경, UI 업데이트 등)
    }

    public void SetCoachInfo(Sprite image, string name, int retireCount)
    {
        coachImage.sprite = image;
        coachName.text = name;
        coachRetireLeftCount.text = $"은퇴까지 {retireCount}년 남음 케헷~";
    }

}
