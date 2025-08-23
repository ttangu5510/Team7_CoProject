using System;
using System.Collections.Generic;

namespace SHG
{
  public class MatchScheduler 
  {
    List<MatchData> matchesOrderedByDate;  
    public int MatchCount => this.matchesOrderedByDate.Count;
    public int NextMatchIndex { get; private set; }
    HashSet<MatchData> registeredMatches;

    // TODO: Load registered matches from save data
    public MatchScheduler(
      IList<MatchData> matchData,
      int startYear,
      int startWeek)
    {
      this.matchesOrderedByDate = new List<MatchData>();
      this.registeredMatches = new ();
      var startDate = new MatchData.Date {
        Year = startYear,
        Week = startWeek
      };
      foreach (var match in matchData) {
        if (match.DateOfEvent > startDate) {
          this.matchesOrderedByDate.Add(match);
        }
      }
      this.matchesOrderedByDate.Sort((a, b) => { 
        if (a.DateOfEvent == b.DateOfEvent) {
          return (0);
        }
        return (a.DateOfEvent < b.DateOfEvent ? - 1: 1);
        });
      this.NextMatchIndex = 0;
    }

    public List<MatchData> GetMatchesFrom(int index)
    {
      return (this.matchesOrderedByDate.GetRange(
          index, this.matchesOrderedByDate.Count - index));
    }

    public bool IsRegistered(in MatchData match) {
      return (this.registeredMatches.Contains(match));
    }

    public void Regiester(in MatchData match)
    {
      this.registeredMatches.Add(match);
    }

    public void UnRegister(in MatchData match) {
      this.registeredMatches.Remove(match); 
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
      var date = new MatchData.Date { Year = year, Week = week };
      var nextMatchDate = this.matchesOrderedByDate[this.NextMatchIndex].DateOfEvent; 
      if (date < nextMatchDate || date == nextMatchDate) {
        return (this.matchesOrderedByDate[this.NextMatchIndex]); 
      }
      for (int i = this.NextMatchIndex + 1; i < this.matchesOrderedByDate.Count; ++i) {
        var matchDate = this.matchesOrderedByDate[i].DateOfEvent;
        if (matchDate > date) {
          this.NextMatchIndex = i;
          return (this.matchesOrderedByDate[this.NextMatchIndex]); 
        }
      }
      return (null);
    }
  }
}
