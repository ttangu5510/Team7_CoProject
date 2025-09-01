using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SavePanelUI : MonoBehaviour
{
    [Header("Set References")]
    [SerializeField] private SaveFileItem item;
    [SerializeField] private Button togle;
    [SerializeField] private Transform uiContent;

    [Inject] private ISaveManager saveManager;

    private int slotNunber = 0;
    
    
    private void Awake()
    {
        
    }
}
