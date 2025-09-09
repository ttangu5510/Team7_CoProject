using System;
using System.Collections.Generic;
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

    public static int CompareMatchResult(
      MatchResult lhs, MatchResult rhs)     
    {       
      var lhsPoint = lhs.CalcPoint();
      var rhsPoint = rhs.CalcPoint();
      if (lhsPoint != rhsPoint) {
        return (lhsPoint > rhsPoint ? -1: 1);
      }       
      var lhsRank = lhs.GetHighestRank();
      var rhsRank = rhs.GetHighestRank();
      return (lhsRank < rhsRank ? - 1: 1);     
    }

    public readonly ResultType Type;
    public IGroup Group 
    { 
      get => this.group;
      set => this.group = value;
    }
    IGroup group;
    public int[] RankCount => this.rankCount;
    [SerializeField]
    int[] rankCount;
    public bool IsUser => this.isUser;
    bool isUser;
    [SerializeField]
    int singleSportRank;
    IContender singleSportAthlete;

    public MatchResult(Match match, IGroup group)
    {
      this.Type = group.Type == IGroup.GroupType.Country ?
        ResultType.International: ResultType.Domestic;
      this.group = group;
      this.rankCount = new int[MAX_RANK];
      foreach (var (sportType, record) in match.SportRecords) {
        int rank = this.GetRankIn(record.RecordsByAthletes);
        this.rankCount[rank - 1] += 1;
      }
    }

    public MatchResult(Match match, IContender athlete)
    {
      this.Type = ResultType.Domestic;
      this.group = athlete.Group;
      if (athlete is ConvertedDomesticAthlete userAthlete) {
        this.isUser = true;
        this.singleSportRank = this.GetRankIn(match, userAthlete);
      }
      else {
        this.isUser = false;
        var record = match.SportRecords[match.Data.SportType];
        this.singleSportRank = this.GetRankIn(record.RecordsByAthletes);
      }
      this.singleSportAthlete = athlete;
    }

    public int[] GetMedalCounts()
    {
      var medals = new int[Enum.GetValues(typeof(MedalType)).Length];
      if (this.rankCount != null) {
        Array.Copy(this.rankCount, medals, 3);
      }
      else if (this.singleSportRank <= 3) {
        medals[this.singleSportRank - 1] = 1;
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
      return (this.singleSportRank);
    }

    public IContender GetDomesticAthlete()
    {
      #if UNITY_EDITOR
      if (this.Type != ResultType.Domestic) {
        throw (new ApplicationException($"{nameof(GetDomesticAthlete)}: {nameof(ResultType)} is not {ResultType.Domestic}"));
      }
      #endif
      return (this.singleSportAthlete);
    }

    public int CalcPoint()
    {
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
        return (this.singleSportRank);
      }
      for (int i = 0; i < this.RankCount.Length; i++) {
        if (this.RankCount[i] != 0) {
          return (i + 1);
        } 
      }
      return (int.MaxValue);
    }

    int GetRankIn(List<(IContender athlete, 
        MatchSportRecord.AthleteRecord record)> recordByAthletes)
    {
      int index = recordByAthletes.FindIndex( 
        (pair) => pair.athlete.Group.Equals(group));
      if (index == -1) {
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {group} in {recordByAthletes}"));
      }
      return (recordByAthletes[index].record.Rank);
    }

    int GetRankIn(Match match, IContender athlete)
    {
      var recordByAthletes = match.SportRecords[match.Data.SportType].RecordsByAthletes;
      var index = recordByAthletes.FindIndex(
        pair => pair.athlete.Equals(athlete));
      if (index == -1) {
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {athlete} in {recordByAthletes}"));
      }
      return (recordByAthletes[index].record.Rank);
    }

    int GetRankIn(Match match, ConvertedDomesticAthlete athlete)
    {
      var recordByAthletes = match.SportRecords[match.Data.SportType].RecordsByAthletes;
      var index = recordByAthletes.FindIndex(
        pair => athlete.IsSameWith(pair.athlete));
      if (index == -1) {
        throw (new ApplicationException($"{nameof(GetRankIn)}: Fail to find {athlete} in {recordByAthletes}"));
      }
      return (recordByAthletes[index].record.Rank);
    }
  }
}
