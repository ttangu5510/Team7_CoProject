using System;
using System.Linq;
using System.Collections.Generic;

namespace SHG
{
  public class MatchScheduler 
  {
    List<MatchData> matchesOrderedByDate;  
    public int MatchCount => this.matchesOrderedByDate.Count;
    public int NextMatchIndex { get; private set; }

    public MatchScheduler(
      IList<MatchData> matchData,
      int startYear,
      int startWeek)
    {
      this.matchesOrderedByDate = new List<MatchData>();
      foreach (var match in matchData) {
        var (year, week) = (match.DateOfEvent.Year, match.DateOfEvent.Week);
        if (match.DateOfEvent.Year > startYear || (year == startYear && week >= startWeek)) {
          this.matchesOrderedByDate.Add(match);
        }
      }
      this.matchesOrderedByDate.Sort((a, b) => {
        if (a.DateOfEvent.Year == b.DateOfEvent.Year) {
          return (a.DateOfEvent.Week > b.DateOfEvent.Week ? 1: -1);
        }
        return (a.DateOfEvent.Year > b.DateOfEvent.Year ? 1: -1);
        });
      this.NextMatchIndex = 0;
    }

    public List<MatchData> GetMatchesFrom(int index)
    {
      return (this.matchesOrderedByDate.GetRange(
          index, this.matchesOrderedByDate.Count - index));
    }

    public MatchData GetMatchBy(int index)
    {
      if (index < 0 || index >= this.matchesOrderedByDate.Count) {
        throw (new ArgumentException($"{nameof(GetMatchBy)}: {nameof(index)}"));
      }
      return (this.matchesOrderedByDate[index]);
    }

    public Nullable<MatchData> GetNextMatch(int year, int week)
    {
      if (this.NextMatchIndex >= this.matchesOrderedByDate.Count) {
        return (null);
      }
      var nextMatchDate = this.matchesOrderedByDate[this.NextMatchIndex].DateOfEvent; 
      if (nextMatchDate.Year > year || (
          nextMatchDate.Year == year && nextMatchDate.Week >= week)) {
        return (this.matchesOrderedByDate[this.NextMatchIndex]); 
      }
      for (int i = this.NextMatchIndex + 1; i < this.matchesOrderedByDate.Count; ++i) {
        var matchDate = this.matchesOrderedByDate[i].DateOfEvent;
        if (matchDate.Year > year || (
            matchDate.Year == year && matchDate.Week >= week)) {
          this.NextMatchIndex = i;
          return (this.matchesOrderedByDate[this.NextMatchIndex]); 
        }
      }
      return (null);
    }
  }
}
