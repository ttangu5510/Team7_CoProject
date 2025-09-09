using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Cysharp.Threading.Tasks;
using JYL;

namespace SHG
{
  [Serializable]
  public class Match 
  {
    public enum State
    {
      NotStartable,
      BeforeStart,
      BeforeSport,
      InSport,
      AfterSport,
      Ended
    }

    public const int TOTAL_STAGE = 5;
    public const float INTERVAL_BETWEEN_STAGE_IN_SECOND = 3f;
    public const int DOMESTIC_CONTENDER_COUNT = 7;
    public MatchData Data 
    { 
      get => this.data;
      private set {
        this.data = value;
      } 
    }

    public ReactiveProperty<Nullable<SportType>> CurrentSport { get; private set; } 
    [SerializeField]
    public ReactiveCollection<SportType> EndedSports;
    public ReactiveDictionary<SportType, DomAthEntity> UserAthletes { get; set; }
    public ReactiveProperty<State> CurrentState { get; private set; }
    public Dictionary<SportType, ReactiveCollection<IContender>> ContenderAthletesBySport { get; private set; }
    public ReactiveDictionary<SportType, MatchSportRecord> SportRecords;
    Dictionary<IGroup, List<IContender>> contenderAthletesByGroups;
    public List<MatchResult> Results { get; private set; }
    public MatchResult UserResult { get; private set; }
  
    [SerializeField]
    MatchData data;
    HashSet<IContender> participatedAthletes;

    public Match(MatchData data, 
      Func<IGroup, List<IContender>> contenderGetter)
    {
      this.Data = data;
      this.EndedSports = new ();
      this.participatedAthletes = new ();
      this.SportRecords = new ();
      this.CurrentState = new (State.NotStartable);
      this.CurrentSport = new (null);
      this.UserAthletes = new ();
      this.FillGroupContenders(contenderGetter);
      this.FillSportContenders();
    }

    public void SelectAthlete(DomAthEntity athlete, SportType sportType)
    {
      if (this.Data.IsSingleSport && 
        sportType != this.Data.SportType) {
        #if UNITY_EDITOR
        throw (new ArgumentException($"{nameof(SelectAthlete)}: {sportType} is same with {nameof(SportType)} {this.Data.SportType}"));
        #else 
        return ; 
        #endif
      }
      if (this.UserAthletes.Any(
          (registed) => registed.Value == athlete && 
          registed.Key != sportType)) {
        #if UNITY_EDITOR
        throw (new ArgumentException($"{nameof(SelectAthlete)}: {athlete} is already selected for other sport"));
        #else 
        return ; 
        #endif
      }
      this.UserAthletes[sportType] = athlete;
      if (this.IsStartable()) {
        this.CurrentState.Value = State.BeforeStart;
      }
      else {
        this.CurrentState.Value = State.NotStartable;
      }
    }

    public void UnSelectAthlete(SportType sportType)
    {
      this.UserAthletes.Remove(sportType);
      this.CurrentState.Value = State.NotStartable;
    }

    public void StartMatch()
    {
      var nextSport = this.GetNextSport();
      this.PrepareSport(nextSport);
      this.CurrentSport.Value = nextSport;
      this.CurrentState.Value = State.BeforeSport;
    }

    public bool IsLastSport()
    {
      if (this.Data.IsSingleSport) {
        return (this.EndedSports.Contains(this.Data.SportType));
      }
      if (this.CurrentSport.Value == null) {
        return (false);
      }
      int currentIndex = Array.IndexOf(
        MatchData.DefaultSports, this.CurrentSport.Value);
      if (currentIndex == -1 || currentIndex == MatchData.DefaultSports.Length - 1) {
        return (true);
      }
      return (false);
    }

    async public void StartCurrentSport()
    {
      if (this.CurrentSport.Value == null) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(StartCurrentSport)}: {nameof(this.CurrentSport)} is null"));
      #else
        return ;
      #endif
      }
      this.CurrentState.Value = State.InSport;
      var sportType = this.CurrentSport.Value.Value;
      int delay = (int)(INTERVAL_BETWEEN_STAGE_IN_SECOND * 1000f); 
      while (!this.SportRecords[sportType].IsEnded()) {
        await UniTask.Delay(delay);  
        this.SportRecords[sportType] = this.SportRecords[sportType].Progress();
      }
      await UniTask.Delay(delay);  
      this.CurrentState.Value = State.AfterSport;
    }

    public void EndCurrentSport()
    {
      #if UNITY_EDITOR
      if (this.CurrentSport.Value == null) {
        throw (new ApplicationException($"{nameof(EndCurrentSport)}: {nameof(CurrentSport)} is null"));
      }
      #endif
      var sport = this.CurrentSport.Value;
      if (sport != null) {
        this.EndedSports.Add(sport.Value);
      }
      if (!this.IsLastSport()) {
        var nextSport = this.GetNextSport();
        this.PrepareSport(nextSport);
        this.CurrentSport.Value = nextSport;
        this.CurrentState.Value = State.BeforeSport;
      }
      else {
        var results = this.GetResults();
        results.Sort(MatchResult.CompareMatchResult);
        this.Results = results;
        this.CurrentState.Value = State.Ended;
        this.CurrentSport.Value = null;
      }
    }

    List<MatchResult> GetResults()
    {
      var results = new List<MatchResult>();
      if (this.Data.IsSingleSport) {
        int count = this.participatedAthletes.Count + 1;
        foreach (var athlete in this.participatedAthletes) {
          results.Add(
            new MatchResult(
              match: this, athlete: athlete));
        }
        this.UserResult = new MatchResult (
            match: this, 
            athlete: new ConvertedDomesticAthlete(
              this.UserAthletes[this.Data.SportType]));
        results.Add(this.UserResult);
      }
      else {
        foreach (var country in this.Data.MemberGroups) {
          results.Add(
            new MatchResult(
              match: this, group: country));
        }
        this.UserResult = new MatchResult(
          match: this, 
          group: ConvertedDomesticAthlete.USER_TEAM);
        results.Add(this.UserResult);
      }
      return (results);
    }

    void PrepareSport(SportType sportType)
    {
      #if UNITY_EDITOR
      if (this.Data.IsSingleSport &&
        sportType != this.Data.SportType) {
        throw (new ArgumentException($"{nameof(PrepareSport)}: {sportType} is not selectable"));
      } 
      else if (this.EndedSports.Contains(sportType)) {
        throw (new ArgumentException($"{nameof(PrepareSport)}: {sportType} is ended"));
      }
      #endif
      var contenderCount = this.ContenderAthletesBySport[sportType].Count;
      var athletes = new IContender[contenderCount + 1];
      athletes[0] = new ConvertedDomesticAthlete(this.UserAthletes[sportType]);
      this.ContenderAthletesBySport[sportType].CopyTo(athletes, 1);
      this.SportRecords.Add(
        sportType, new MatchSportRecord (sportType, athletes));
      this.CurrentSport.Value = sportType;
    }

    bool IsStartable()
    {
      if (this.Data.IsSingleSport) {
        return (this.UserAthletes.ContainsKey(this.Data.SportType));
      }
      else {
        if (Array.FindIndex(MatchData.DefaultSports, 
            sport => !this.UserAthletes.ContainsKey(sport)) != -1) {
            return (false); 
          }
        return (true);
      }
    }

    void FillGroupContenders(
      Func<IGroup, List<IContender>> contenderGetter)
    {
      this.contenderAthletesByGroups = new ();
      foreach (var group in this.Data.MemberGroups) {
        this.contenderAthletesByGroups.Add(
          group, contenderGetter(group)); 
      }
    }

    void FillSportContenders()
    {
      this.ContenderAthletesBySport = new ();
      if (this.Data.IsSingleSport) {
        this.FillContenders(this.Data.SportType);
      }
      else {
        foreach (var sport in MatchData.DefaultSports) {
          this.FillContenders(sport); 
        }
      }
    }

    void FillContenders(SportType sportType)
    {
      List<IContender> contenders = new ();
      foreach (var group in this.Data.MemberGroups) {
        contenders.Add(this.SelectContender(sportType, group));
      }
      this.ContenderAthletesBySport[sportType] = new (contenders);
    }

    // TODO: 상대 선수 선택 알고리즘
    IContender SelectContender(SportType sportType, IGroup group)
    {
      var rand = new System.Random();
      var contenders = this.contenderAthletesByGroups[group];
      int index = rand.Next(0, contenders.Count);
      IContender selected = contenders[index];
      while (this.participatedAthletes.Contains(selected)) {
        index = rand.Next(0, contenders.Count);
        selected = contenders[index];
      }
      this.participatedAthletes.Add(selected);
      return (selected);
    }

    SportType GetNextSport()
    {
      if (this.Data.IsSingleSport) {
          return (this.Data.SportType);
      }
      if (this.CurrentSport.Value == null) {
        return (MatchData.DefaultSports[0]);
      }
      int currentIndex = Array.IndexOf(
        MatchData.DefaultSports, this.CurrentSport.Value);
      if (currentIndex == -1 || 
        currentIndex == MatchData.DefaultSports.Length - 1) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(GetNextSport)}: {nameof(this.CurrentSport)} index is {currentIndex}"));
        #else
        return (SportType.SkiJumping);
        #endif
      }
      return (MatchData.DefaultSports[currentIndex + 1]);
    }
  }
}
