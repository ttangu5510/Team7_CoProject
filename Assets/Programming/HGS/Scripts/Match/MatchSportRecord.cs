using System;
using System.Collections.Generic;
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

      public float CalcedValue;
      public int Rank;
      public float NormalizedValue;
    }

    public enum ProgressType
    {
      OneByOne,
      AllAtOnce
    }

    public static ProgressType GetProgressType(SportType sportType)
    {
      switch (sportType) {
        case SportType.SkiJumping:
        case SportType.Skeleton:
        case SportType.FigureSkating:
          return (ProgressType.OneByOne);
        case SportType.SpeedSkating:
          return (ProgressType.AllAtOnce);
        default: 
          throw (new NotImplementedException());
      }
    }

    [SerializeField]
    public SportType SportType;
    [SerializeField]
    public List<(IContender athlete, Record record)> RecordsByAthletes;  
    List<(IContender athlete, float value)> preCalcedRecordValues;
    [SerializeField]
    public int CurrentStage;
    public ProgressType Type 
    {
      get => this.type; 
      private set => this.type = value;
    }
    ProgressType type;
    IContender[] athletes;

    public MatchSportRecord(
      SportType sportType,
      in IContender[] athletes)
    {
      this.SportType = sportType;
      this.athletes = athletes;
      this.CurrentStage = 1;
      this.type = GetProgressType(sportType);
      if (this.type == ProgressType.AllAtOnce) {
        this.RecordsByAthletes = new (athletes.Length);
        for (int i = 0; i < athletes.Length; i++) {
          this.RecordsByAthletes.Add((athletes[i], new Record {})); 
        }
        this.preCalcedRecordValues = null;
      }
      else {
        this.preCalcedRecordValues = new ();
        this.RecordsByAthletes = new ();
        foreach (var athlete in athletes) {
          this.preCalcedRecordValues.Add(
            (athlete, this.GetRecordValueBy(athlete.Stats))); 
        }
        this.preCalcedRecordValues.Sort(this.CompareRecordValue);
      }
    } 


    public bool IsEnded()
    {
      if (this.Type == ProgressType.OneByOne) {
        return (this.CurrentStage > this.athletes.Length);
      }
      else {
        return (this.CurrentStage >= Match.TOTAL_STAGE);
      }
    }

    public MatchSportRecord Progress()
    {
      if (this.Type == ProgressType.AllAtOnce) {
        return (this.ProgressAllAtOnce());
      }
      else {
        return (this.ProgressOneByOne());
      }
    }

    MatchSportRecord ProgressOneByOne()
    {
      var athlete = this.SelectNextAthlete();
      var index = this.preCalcedRecordValues.FindIndex(
        record => record.athlete == athlete);
      if (index == - 1) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(ProgressOneByOne)}: Fail to find {athlete} in {nameof(this.preCalcedRecordValues)}"));
        #else
        return (new MatchSportRecord{});
        #endif
      }
      var newRecord =  new Record {
         CalcedValue = this.preCalcedRecordValues[index].value,
         Rank = index + 1
         };
      newRecord.NormalizedValue = this.GetNormalizedRecordValueOf(newRecord);
      this.RecordsByAthletes.Add((athlete, newRecord));

      return (new MatchSportRecord(this));
    }

    IContender SelectNextAthlete()
    {
      return (this.athletes[this.CurrentStage - 1]);
    }

    MatchSportRecord ProgressAllAtOnce()
    {
      var athletes = new IContender[this.RecordsByAthletes.Count];
      for (int i = 0; i < this.RecordsByAthletes.Count; i++) {
        var currentValue = this.RecordsByAthletes[i].record.CalcedValue;
        var stats = this.RecordsByAthletes[i].athlete.Stats;
        var (athlete, record)= this.RecordsByAthletes[i];
        record.CalcedValue = this.GetRecordValueFrom(currentValue, stats);  
        record.NormalizedValue = this.GetNormalizedRecordValueOf(record);
        this.RecordsByAthletes[i] = (athlete, record);
        athletes[i] = this.RecordsByAthletes[i].athlete;
      } 
      Array.Sort(athletes, this.CompareAthleteByRecord);
      for (int i = 0; i < athletes.Length; i++) {
        var athlete = athletes[i];
        var index = this.RecordsByAthletes.FindIndex(
          record => record.athlete == athlete);
        var (recordAthlete, record) = this.RecordsByAthletes[index];
        record.Rank = i + 1;
        this.RecordsByAthletes[index] = (recordAthlete, record);
      }

      return (new MatchSportRecord (this));
    }

    MatchSportRecord(in MatchSportRecord oldRecord)
    {
      this.SportType = oldRecord.SportType;
      this.athletes = oldRecord.athletes;
      this.type = oldRecord.type;
      this.preCalcedRecordValues = oldRecord.preCalcedRecordValues;
      oldRecord.RecordsByAthletes.Sort(
        (lhs, rhs) => (lhs.record.Rank < rhs.record.Rank ? -1 : 1));
      this.RecordsByAthletes = oldRecord.RecordsByAthletes;
      this.CurrentStage = oldRecord.CurrentStage + 1;
    }

    int CompareRecordValue(
      (IContender athlete, float value) lhs, 
      (IContender athlete, float value) rhs)  {

      var lhsRecord = lhs.value;
      var rhsRecord = rhs.value;
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

    int CompareAthleteByRecord(IContender lhs, IContender rhs)
    {
      var lhsRecord = this.GetRecordOf(lhs).CalcedValue;
      var rhsRecord = this.GetRecordOf(rhs).CalcedValue;
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

    /*
     * 즉시 결과 계산식	
     *  => ( 해당 능력치 평균 - (피로도  * 보정 값)) * 1 +- 0.1
    */
    float GetRecordValueBy(AthleteStats stats)
    {
      float statAverage = this.GetStatAverage(stats); 
      float fatigueAdjust = this.GetFatigueAdjustValue(statAverage);
    
      return ((statAverage - (stats.fatigue * fatigueAdjust)) * 1.0f - 0.1f);
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

    float GetNormalizedRecordValueOf(in Record record)
    {
      var (min, max) = this.GetRecordRangeOf(this.SportType);
      int athleteCount = this.athletes.Length;

      if (this.Type  == ProgressType.AllAtOnce ) {
        athleteCount = this.RecordsByAthletes.Count;
        float stageAdjust = (float)this.CurrentStage / (float)Match.TOTAL_STAGE;
        min *= stageAdjust;
        max *= stageAdjust;
      }
      float rankAdjust = this.SportType switch {
        SportType.Skeleton or SportType.SpeedSkating => 
          (float)(record.Rank) / (float)athleteCount,
        SportType.SkiJumping or SportType.FigureSkating =>
          (float)(athleteCount - record.Rank) / (float)athleteCount,
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
      int index = this.RecordsByAthletes.FindIndex(
        recordWitAthlete => 
        recordWitAthlete.athlete is ConvertedDomesticAthlete converted &&
        converted.IsSameWith(athlete));
      if (index == -1) {
        throw (new ApplicationException());
      }
      return (this.RecordsByAthletes[index].record);
    }

    Record GetRecordOf(IContender athlete)
    {
      int index = this.RecordsByAthletes.FindIndex(
        recordWitAthlete => recordWitAthlete.athlete == athlete);
      if (index == -1) {
        throw (new ApplicationException());
      }
      return (this.RecordsByAthletes[index].record);
    }

    void SetRecordOf(DomAthEntity athlete, float record)
    {
      int index = this.RecordsByAthletes.FindIndex(
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
      var (recordAthlete, _) = this.RecordsByAthletes[index];
      this.RecordsByAthletes[index] = (recordAthlete,  new Record {
        CalcedValue = record,
      });
    }

    void SetRecord(IContender athlete, float record)
    {
      int index = this.RecordsByAthletes.FindIndex(
        recordWitAthlete => recordWitAthlete.athlete == athlete);
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException());
        #else
        return ;
        #endif
      }
      this.RecordsByAthletes[index]= (athlete,  new Record {
        CalcedValue = record
      });
    }
  }
}
