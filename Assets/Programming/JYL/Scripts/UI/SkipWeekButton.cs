using System.Collections;
using System.Collections.Generic;
using SHG;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SkipWeekButton : MonoBehaviour
{
    private Button nextWeekButton;
    
    [Inject] ITimeFlowController  timeFlowController;

    
    private void Awake()
    {
        nextWeekButton = GetComponent<Button>();
        nextWeekButton.OnClickAsObservable()
            .Subscribe(_=>timeFlowController.ProgressWeek())
            .AddTo(this);
    }
}
