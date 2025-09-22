using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListPanelMMJ : MonoBehaviour
{
    [Header("Set Content Transform")]
    [SerializeField] private RectTransform popUpParent; // 선수 정보 팝업창의 부모
    [SerializeField] Transform parentContent;   // 아이템들이 생성될 부모

    [Header("Set Prefabs")]
    [SerializeField] private AthleteTrainingItemUI athleteItem;

    // 아이템을 클릭하면 정보가 출력됨
    private Button itemButton;

}
