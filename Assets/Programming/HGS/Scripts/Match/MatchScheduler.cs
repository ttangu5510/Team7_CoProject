using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SHG
{
  public class MatchScheduler : MonoBehaviour
  {
    List<MatchData> matchesOrderedByDate;  

    public MatchScheduler(IList<MatchData> matchData)
    {
      this.matchesOrderedByDate = new List<MatchData>(matchData.Count);
      this.matchesOrderedByDate.AddRange(matchData);
      this.matchesOrderedByDate.Sort((a, b) => {
        if (a.DateOfEvent.year == b.DateOfEvent.year) {
          return (a.DateOfEvent.week > b.DateOfEvent.week ? 1: -1);
        }
        return (a.DateOfEvent.year > b.DateOfEvent.year ? 1: -1);
        });
    }
  }
}
