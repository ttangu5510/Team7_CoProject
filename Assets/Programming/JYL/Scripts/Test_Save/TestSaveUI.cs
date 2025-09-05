using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TestSaveUI : MonoBehaviour
{
    [SerializeField] private Button test1;  
    [SerializeField] private Button test2;  
    [SerializeField] private Button test3;
    
    [Inject] private ISaveManager saveManager;

    [SerializeField] private int slotNum = 1;
    void Start()
    {
        test1.onClick.AddListener(TestSave);
        test2.onClick.AddListener(TestSave2);
        test3.onClick.AddListener(TestSave3);
    }

    private void TestSave()
    {
        saveManager.SaveProgress(slotNum);
        slotNum++;
    }
    private void TestSave2()
    {
        saveManager.SaveProgress(slotNum);
        slotNum--;
    }

    private void TestSave3()
    {
        saveManager.SaveProgress(slotNum);
        slotNum *= 2;
    }

}
