using System;
using UnityEngine;
using JYL;

namespace SHG
{
  using Random = UnityEngine.Random;

  [Serializable]
  public struct MatchSportRecord 
  {
    [Serializable]
    public struct Record {

      public float Value;
      public int Rank;
    }

    [SerializeField]
    public SportType SportType;
    [SerializeField]
    public (IContenderAthlete athlete, Record record)[] RecordsByAthletes;  
    [SerializeField]
    public int CurrentStage;

    public MatchSportRecord(
      SportType sportType,
      in IContenderAthlete[] athletes)
    {
      this.SportType = sportType;
      this.RecordsByAthletes = new (IContenderAthlete athlete, Record record)[athletes.Length];
      for (int i = 0; i < athletes.Length; i++) {
        this.RecordsByAthletes[i] = (athletes[i], new Record {}); 
      }
      this.CurrentStage = 1;
    } 

    public MatchSportRecord Progress()
    {
      var athletes = new IContenderAthlete[this.RecordsByAthletes.Length];
      for (int i = 0; i < this.RecordsByAthletes.Length; i++) {
        var currentValue = this.RecordsByAthletes[i].record.Value;
        var stats = this.RecordsByAthletes[i].athlete.Stats;
        this.RecordsByAthletes[i].record.Value = this.GetRecordValueFrom(
          currentValue, stats);  
        athletes[i] = this.RecordsByAthletes[i].athlete;
      } 
      Array.Sort(athletes, this.CompareAthleteByRecord);
      for (int i = 0; i < athletes.Length; i++) {
        var athlete = athletes[i];
        var index = Array.FindIndex(this.RecordsByAthletes, record => record.athlete == athlete);
        this.RecordsByAthletes[index].record.Rank = i + 1;
      }

      return (new MatchSportRecord (this));
    }

    MatchSportRecord(in MatchSportRecord oldRecord)
    {
      this.SportType = oldRecord.SportType;
      Array.Sort(oldRecord.RecordsByAthletes, 
        (lhs, rhs) => (lhs.record.Rank < rhs.record.Rank ? -1 : 1));
      this.RecordsByAthletes = oldRecord.RecordsByAthletes;
      this.CurrentStage = oldRecord.CurrentStage + 1;
    }

    int CompareAthleteByRecord(IContenderAthlete lhs, IContenderAthlete rhs)
    {
      var lhsRecord = this.GetRecordOf(lhs).Value;
      var rhsRecord = this.GetRecordOf(rhs).Value;
      switch (this.SportType) {
        case SportType.Skeleton:
        case SportType.SpeedSkating:
          return (lhsRecord < rhsRecord ? -1 : 1);
        case SportType.SkiJumping:
        case SportType.FigureSkating:
          return (lhsRecord < rhsRecord ? 1: -1);
        default: 
          throw (new NotImplementedException());
      }
    }

    /* TODO: 계산식 적용
     * 순위 변동 계산식	
     *   => ( 해당 능력치 평균 - (피로도 * 보정 값)) * 순위 가변치 + - 0.1
     * 즉시 결과 계산식	
     *  => ( 해당 능력치 평균 - (피로도  * 보정 값)) * 1 +- 0.1
    */
    float GetRecordValueFrom(float currentValue, AthleteStats stats)
    {
      float statAverage = this.GetStatAverage(stats); 
      float fatigueAdjust = this.GetFatigueAdjustValue(statAverage);
      float rankAdjust = this.GetRankAdjustValue();
      
      float calcedValue = (statAverage - (stats.fatigue * fatigueAdjust))
        * rankAdjust - 0.1f;
      return (currentValue + calcedValue);
    }

    float GetRankAdjustValue()
    {
      switch (this.CurrentStage) {
        case 0:
          return (Random.Range(0f, 12f));
        case 1:
          return (Random.Range(0f, 9f));
        case 2:
          return (Random.Range(0f, 6f));
        case 3:
          return (Random.Range(0f, 3f));
        default:
          return (1f);
      }
    }

    public float GetNormalizedRecordValueOf(in Record record)
    {
      var (min, max) = this.GetRecordRangeOf(this.SportType);

      float stageAdjust = (float)this.CurrentStage / (float)Match.TOTAL_STAGE;
      min *= stageAdjust;
      max *= stageAdjust;
      float rankAdjust = this.SportType switch {
        SportType.Skeleton or SportType.SpeedSkating => 
          (float)(record.Rank) / (float)this.RecordsByAthletes.Length,
        SportType.SkiJumping or SportType.FigureSkating =>
          (float)(this.RecordsByAthletes.Length - record.Rank) / (float)this.RecordsByAthletes.Length,
        _ => throw (new NotImplementedException())
      };
      return (Mathf.Lerp(min, max, rankAdjust));
    }

    (float min, float max) GetRecordRangeOf(SportType sportType)
    {
      switch (sportType) {
        case SportType.SkiJumping:
          return (180f, 230f);
        case SportType.Skeleton:
          return (50f, 120f);
        case SportType.FigureSkating:
          return (270f, 320f);
        case SportType.SpeedSkating:
          return (35f, 45f);
        default: 
          throw (new NotImplementedException());
      }
    }

    float GetFatigueAdjustValue(float statAverage)
    {
      switch (statAverage) {
        case <= 100f:
          return (0.1f);
        case <= 200f:
          return (0.2f);
        case <= 300f:
          return (0.3f);
        case <= 400f:
          return (0.4f);
        case <= 500f:
          return (0.5f);
        case <= 600f:
          return (0.6f);
        default:
          return (0.6f);
      }
    }

    // TODO: 적용 능력치 수정
    float GetStatAverage(AthleteStats stat)
    {
      float total = 0f; 
      int count = 0;
      switch (this.SportType) {
        case SportType.FigureSkating:
          total += stat.technic;
          total += stat.health;
          count += 2;
          break;
        case SportType.SpeedSkating:
          total += stat.quickness;
          total += stat.technic;
          count += 2;
          break;
        case SportType.Skeleton:
          total += stat.health;
          total += stat.flexibility;
          count += 2;
          break;
        case SportType.SkiJumping:
          total += stat.speed;
          total += stat.balance;
          count += 2;
          break;
        default:
          throw (new NotImplementedException());
      }
      return (total / count);
    }

    public Record GetRecordOf(DomAthEntity athlete)
    {
      int index = Array.FindIndex(
        this.RecordsByAthletes, 
        recordWitAthlete => 
        recordWitAthlete.athlete is ConvertedDomesticAthlete converted &&
        converted.IsSameWith(athlete));
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException());
        #else
        return (0f);
        #endif
      }
      return (this.RecordsByAthletes[index].record);
    }

    Record GetRecordOf(IContenderAthlete athlete)
    {
      int index = Array.FindIndex(
        this.RecordsByAthletes, 
        recordWitAthlete => recordWitAthlete.athlete == athlete);
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException());
        #else
        return (0f);
        #endif
      }
      return (this.RecordsByAthletes[index].record);
    }

    void SetRecordOf(DomAthEntity athlete, float record)
    {
      int index = Array.FindIndex(
        this.RecordsByAthletes, 
        recordWitAthlete => 
        recordWitAthlete.athlete is ConvertedDomesticAthlete converted &&
        converted.IsSameWith(athlete));
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException());
        #else
        return;
        #endif
      }
      this.RecordsByAthletes[index].record = new Record {
        Value = record,
      };
    }

    void SetRecord(IContenderAthlete athlete, float record)
    {
      int index = Array.FindIndex(
        this.RecordsByAthletes, 
        recordWitAthlete => recordWitAthlete.athlete == athlete);
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException());
        #else
        return ;
        #endif
      }
      this.RecordsByAthletes[index].record = new Record {
        Value = record
      };
    }
  }
}
