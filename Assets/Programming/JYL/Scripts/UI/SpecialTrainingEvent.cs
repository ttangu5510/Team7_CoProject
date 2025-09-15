using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public struct SpecialTrainingEvent
{
    public bool startTraining { get; }
    public int trainingStage { get; }

    public SpecialTrainingEvent(bool startTraining, int trainingStage)
    {
        this.startTraining = startTraining;
        this.trainingStage = trainingStage;
    }
}
