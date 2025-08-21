using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace SHG
{
  public class MatchController
  {
    public ReactiveProperty<Nullable<MatchData>> NextMatch { get; set; }

    List<MatchData> matchesOrderedByDate;  
    MatchScheduler scheduler;

    public MatchController(IList<MatchData> matchData)
    {
      this.scheduler = new (matchData);
    }
  }
}
