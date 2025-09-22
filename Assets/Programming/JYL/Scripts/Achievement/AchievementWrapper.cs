using System.Collections;
using System.Collections.Generic;
using JWS;
using UniRx;
using UnityEngine;

public class AchievementWrapper
{
    public ReactiveProperty<int> MatchEntryCount { get; }
    public ReactiveProperty<int> MatchWinCount { get; }
    public ReactiveProperty<int> TrainCount { get; }
    public ReactiveProperty<int> RecoverCount { get; }
    public ReactiveProperty<int> SpecialTrainCount { get; }
    public ReactiveProperty<int> AthleteRecruitCount { get; }
    public ReactiveProperty<int> CoachRecruitCount { get; }
    public ReactiveProperty<int> AthleteRetireCount { get; }

    private AchievementRecord data;
    
    public AchievementWrapper(AchievementRecord record)
    {
        data = record;
        
        // ReactiveProperty 초기화
        MatchEntryCount = new(record.matchEntryCount);
        MatchWinCount = new(record.matchWinCount);
        TrainCount = new(record.trainCount);
        RecoverCount = new(record.recoverCount);
        SpecialTrainCount = new(record.specialTrainCount);
        AthleteRecruitCount = new(record.athleteRecruitCount);
        CoachRecruitCount = new(record.coachRecruitCount);
        AthleteRetireCount = new(record.athleteRetireCount);
        
        // ReactiveProperty -> 세이브데이터 초기화
        MatchEntryCount.Subscribe(n => data.matchEntryCount = n);
        MatchWinCount.Subscribe(n => data.matchWinCount = n);
        TrainCount.Subscribe(n => data.trainCount = n);
        RecoverCount.Subscribe(n => data.recoverCount = n);
        SpecialTrainCount.Subscribe(n => data.specialTrainCount = n);
        AthleteRecruitCount.Subscribe(n => data.athleteRecruitCount = n);
        CoachRecruitCount.Subscribe(n => data.coachRecruitCount = n);
        AthleteRetireCount.Subscribe(n => data.athleteRetireCount = n);
    }
}
