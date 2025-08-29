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
      Gold = 5, // 5 point
      Silver = 3, // 3 point
      Bronze = 1 // 1 point
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
      return (this.domesticRank);
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

    public int CalcPoint()
    {
      if (this.Type == ResultType.Domestic) {
        switch (this.domesticRank) {
          case (1):
            return ((int)MedalType.Gold);
          case (2):
            return ((int)MedalType.Silver);
          case (3):
            return ((int)MedalType.Bronze);
          default: 
            return (0);
        } 
      }
      int point = 0;
      var medals = this.GetMedalCounts();
      point += medals[0] * (int)MedalType.Gold;
      point += medals[1] * (int)MedalType.Silver;
      point += medals[2] * (int)MedalType.Bronze;
      return (point);
    }

    public int GetHighestRank()
    {
      if (this.Type == ResultType.Domestic) {
        return (this.domesticRank);
      }
      for (int i = 0; i < this.RankCount.Length; i++) {
        if (this.RankCount[i] != 0) {
          return (i + 1);
        } 
      }
      return (int.MaxValue);
    }

    int GetRankIn((IContenderAthlete athlete, 
        MatchSportRecord.Record record)[] recordByAthletes)
    {
      int index = Array.FindIndex(
        recordByAthletes, (pair) => 
          pair.athlete.Country == country 
        );
      if (index == -1) {
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {country} in {recordByAthletes}"));
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
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {athlete} in {recordByAthletes}"));
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
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {athlete} in {recordByAthletes}"));
      }
      return (recordByAthletes[index].record.Rank);
    }
  }
}
