using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AthleteRecruitedEvent
{
    public int athleteId;
    public AthleteRecruitedEvent(int id) { athleteId = id; }
}

public struct AthleteOutEvent
{
    public int athleteId;
    public AthleteOutEvent(int id) { athleteId = id; }
}

public struct AthleteRetiredEvent
{
    public int athleteId;
    public AthleteRetiredEvent(int id) { athleteId = id; }
}


