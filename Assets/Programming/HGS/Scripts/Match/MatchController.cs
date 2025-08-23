using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace SHG
{
  /// <summary>
  /// 경기를 관리하는 역할, IMatchController를 참고
  /// </summary>
  public class MatchController: IMatchController
  {
    ITimeFlowController timeFlowController;

    public ReactiveProperty<Match> CurrentMatch { get; private set; }
    public ReactiveProperty<Nullable<MatchData>> NextMatch { get; private set; }
    public ReactiveCollection<MatchData> ScheduledMatches { get; private set; }
    public Func<Country, List<IContenderAthlete>> ContenderGetter;
    public IList<MatchData> MatchData { get; private set; }
    public ReactiveCollection<MatchData> RegisteredMatches { get; private set; }

    MatchScheduler scheduler;
    IDisposable timeSubscribed;

    public MatchController(IList<MatchData> matchData)
    {
      this.MatchData = matchData;
      this.CurrentMatch = new (null);
      this.NextMatch = new (null);
    }

    //TODO: Load save data
    [Inject]
    public void Init(ITimeFlowController timeFlowController)
    {
      this.timeFlowController = timeFlowController;
      this.scheduler = new MatchScheduler(
        matchData: this.MatchData,
        startYear: this.timeFlowController.YearPassedAfterStart,
        startWeek: this.timeFlowController.WeekInYear.Value);
      this.NextMatch.Value = this.scheduler.GetNextMatch(
        year: this.timeFlowController.YearPassedAfterStart,
        week: this.timeFlowController.WeekInYear.Value);
      this.ScheduledMatches = new (this.GetScheduledMatches());
      this.timeSubscribed = this.SubscribeTimeFlow();
      this.RegisteredMatches = new ();
    }

    public void Register(in MatchData match) {
      this.scheduler.Regiester(match);
      int index = 0;
      while (this.RegisteredMatches.Count > index) {
        if (this.RegisteredMatches[index].DateOfEvent > 
          match.DateOfEvent) {
          break;
        }
        index += 1;
      }
      if (index < this.RegisteredMatches.Count) {
        this.RegisteredMatches.Insert(index, match);
      }
      else {
        this.RegisteredMatches.Add(match);
      }
    }

    public void UnRegister(in MatchData match) {
      this.scheduler.UnRegister(match);
      int index = 0;
      for (; index < this.RegisteredMatches.Count; ++index) {
        if (this.RegisteredMatches[index] == match) {
          break;
        } 
      }
      if (index < this.RegisteredMatches.Count) {
        this.RegisteredMatches.RemoveAt(index);
      }
      #if UNITY_EDITOR
      throw (new ApplicationException($"{nameof(UnRegister)}: Fail to find index of {match}"));
      #endif
    }

    public bool IsRegistered(in MatchData match) {
      return (this.scheduler.IsRegistered(match));
    }

    public void EnterNextMatch()
    {
      if (this.NextMatch.Value == null) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(EnterNextMatch)}: {nameof(NextMatch)} is null"));
      #else
        return ;
      #endif
      }

      this.CurrentMatch.Value = this.CreateMatch(this.NextMatch.Value.Value);
    }

    Match CreateMatch(MatchData data)
    {
      var match = new Match(
        data: data,
        contenderGetter: this.ContenderGetter);
      return (match);
    }

    List<MatchData> GetScheduledMatches()
    {
      List<MatchData> matches = new (this.scheduler.MatchCount - this.scheduler.NextMatchIndex);
      for (int i = this.scheduler.NextMatchIndex; i < this.scheduler.MatchCount; i++) {
        matches.Add(this.scheduler.GetMatchBy(i)); 
      }
      return (matches);
    }

    IDisposable SubscribeTimeFlow()
    {
      return (this.timeFlowController.Year
        .Merge(this.timeFlowController.WeekInYear)
        .Subscribe(_ => {
          var prevMatch = this.NextMatch.Value;
          this.NextMatch.Value = this.scheduler.GetNextMatch(
            year: this.timeFlowController.YearPassedAfterStart,
            week: this.timeFlowController.WeekInYear.Value);
          if (prevMatch != this.NextMatch.Value) {
            this.UpdateScheduledMatch();
          }}));
    }

    void UpdateScheduledMatch()
    {
      if (this.ScheduledMatches.Count == 0) {
      #if UNITY_EDITOR
        // 스케쥴되어 있는 경기가 없을 경우 (게임 진행중 새로운 매치 생성?)
        throw (new ApplicationException($"{nameof(UpdateScheduledMatch)}: {nameof(this.ScheduledMatches)} count is not 0"));
      #else 
        return ;
      #endif
      }
      if (this.ScheduledMatches.Count == 1) {
      #if UNITY_EDITOR
        if (this.NextMatch.Value != null) {
          // 경기가 1개 남았을 때 변화는 사라지는 것만 가능
          throw (new ApplicationException($"{nameof(UpdateScheduledMatch)}: {nameof(this.ScheduledMatches)} count == 1, {nameof(this.NextMatch)} is not null"));   
        }
      #endif
        this.ScheduledMatches.Clear();
      }
      var newMatches = this.scheduler.GetMatchesFrom(
        this.scheduler.NextMatchIndex);
      var prevMatch = this.ScheduledMatches[1];
      if (newMatches.Count == this.ScheduledMatches.Count - 1 &&
        newMatches[0] == prevMatch) {
        this.ScheduledMatches.RemoveAt(0); 
      }
      else {
      #if UNITY_EDITOR
        // 경기가 2개 이상 지나간 경우 또는 경기 내용이 바뀐 경우
        throw (new ApplicationException($"{nameof(UpdateScheduledMatch)}: unexpected changing of ScheduledMatches"));
      #endif
      }
    }

  }
}
