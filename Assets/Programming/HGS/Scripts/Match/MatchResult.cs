using System;
using UnityEngine;

namespace SHG
{
  [Serializable]
  public class MatchResult
  {
    public const int MAX_RANK = 8;

    public enum ResultType
    {
      Domestic,
      International
    }

    public enum MedalType 
    {
      Gold,
      Silver,
      Bronze
    }

    public readonly ResultType Type;
    public Country Country 
    { 
      get => this.country;
      set => this.country = value;
    }
    Country country;
    public int[] RankCount => this.rankCount;
    [SerializeField]
    int[] rankCount;
    public bool IsUser => this.isUser;
    bool isUser;
    [SerializeField]
    int domesticRank;
    IContenderAthlete domesticAthlete;

    public MatchResult(Match match, Country country)
    {
      this.Type = ResultType.International;
      this.country = country;
      this.rankCount = new int[MAX_RANK];
      foreach (var (sportType, record) in match.SportRecords) {
        int rank = this.GetRankIn(record.RecordsByAthletes);
        this.rankCount[rank - 1] += 1;
      }
    }

    public MatchResult(Match match, IContenderAthlete athlete)
    {
      this.Type = ResultType.Domestic;
      this.country = athlete.Country;
      if (athlete is ConvertedDomesticAthlete userAthlete) {
        this.isUser = true;
        this.domesticRank = this.GetRankIn(match, userAthlete);
      }
      else {
        this.isUser = false;
        this.domesticRank = this.GetRankIn(match, athlete);
      }
      this.domesticAthlete = athlete;
    }

    public int[] GetMedalCounts()
    {
      var medals = new int[Enum.GetValues(typeof(MedalType)).Length];
      if (this.Type != ResultType.Domestic) {
        Array.Copy(this.rankCount, medals, 3);
      }
      else if (this.domesticRank < 3) {
        medals[this.domesticRank] = 1;
      }
      return (medals);
    }

    public int GetDomesticRank()
    {
      #if UNITY_EDITOR
      if (this.Type != ResultType.Domestic) {
        throw (new ApplicationException($"{nameof(GetDomesticRank)}: {nameof(ResultType)} is not {ResultType.Domestic}"));
      }
      #endif
      return (this.domesticRank + 1);
    }

    public IContenderAthlete GetDomesticAthlete()
    {
      #if UNITY_EDITOR
      if (this.Type != ResultType.Domestic) {
        throw (new ApplicationException($"{nameof(GetDomesticAthlete)}: {nameof(ResultType)} is not {ResultType.Domestic}"));
      }
      #endif
      return (this.domesticAthlete);
    }

    int GetRankIn((IContenderAthlete athlete, 
        MatchSportRecord.Record record)[] recordByAthletes)
    {
      int index = Array.FindIndex(
        recordByAthletes, (pair) => 
          pair.athlete.Country == country 
        );
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {country} in {recordByAthletes}"));
        #else
        continue;
        #endif
      }
      return (recordByAthletes[index].record.Rank);
    }

    int GetRankIn(Match match, IContenderAthlete athlete)
    {
      var recordByAthletes = match.SportRecords[match.Data.SportType].RecordsByAthletes;
      var index = Array.FindIndex(
        recordByAthletes,
        pair => pair.athlete == athlete);
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {athlete} in {recordByAthletes}"));
        #else
        continue;
        #endif
      }
      return (recordByAthletes[index].record.Rank);
    }

    int GetRankIn(Match match, ConvertedDomesticAthlete athlete)
    {
      var recordByAthletes = match.SportRecords[match.Data.SportType].RecordsByAthletes;
      var index = Array.FindIndex(
        recordByAthletes,
        pair => athlete.IsSameWith(pair.athlete));
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {athlete} in {recordByAthletes}"));
        #else
        continue;
        #endif
      }
      return (recordByAthletes[index].record.Rank);
    }
  }
}
