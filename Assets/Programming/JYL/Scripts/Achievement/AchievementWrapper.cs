using System.Collections;
using System.Collections.Generic;
using JWS;
using UniRx;
using UnityEngine;

public class AchievementWrapper
{
    public ReactiveProperty<int> MatchEntryCount { get; }
    public ReactiveProperty<int> MatchWinCount { get; }
    public ReactiveProperty<int> TrainCount { get; } // 연결완료
    public ReactiveProperty<int> RecoverCount { get; }
    public ReactiveProperty<int> SpecialTrainCount { get; } // 연결완료
    public ReactiveProperty<int> AthleteRecruitCount { get; } // 연결완료
    public ReactiveProperty<int> CoachRecruitCount { get; } // 연결완료
    public ReactiveProperty<int> AthleteRetireCount { get; } // 연결완료

    private AchievementRecord data;
    
    public AchievementWrapper(AchievementRecord record)
    {
        data = record;
        
        // ReactiveProperty 초기화
        MatchEntryCount = new(data.matchEntryCount);
        MatchWinCount = new(data.matchWinCount);
        TrainCount = new(data.trainCount);
        RecoverCount = new(data.recoverCount);
        SpecialTrainCount = new(data.specialTrainCount);
        AthleteRecruitCount = new(data.athleteRecruitCount);
        CoachRecruitCount = new(data.coachRecruitCount);
        AthleteRetireCount = new(data.athleteRetireCount);
        
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
