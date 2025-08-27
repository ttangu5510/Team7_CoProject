using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Title_Panel : MonoBehaviour
{
    [SerializeField] Button fromTheBeginning;
    [SerializeField] Button continue_Btn;
    [SerializeField] Button playRecord;

    private void Awake()
    {
        fromTheBeginning.onClick.AddListener(() => FromTheBeginning());
        continue_Btn.onClick.AddListener(() => Continue());
        playRecord.onClick.AddListener(() => PlayRecord());
    }


    void FromTheBeginning()
    {
        Debug.Log("처음부터 누름");
    }

    void Continue()
    {
        Debug.Log("계속하기 누름");
    }

    void PlayRecord()
    {
        Debug.Log("플레이 기록 누름");
    }

}
