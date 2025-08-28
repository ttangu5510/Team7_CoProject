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
    public Dictionary<SportType, ReactiveCollection<IContenderAthlete>> ContenderAthletesBySport { get; private set; }
    public ReactiveDictionary<SportType, MatchSportRecord> SportRecords;
    Dictionary<Country, List<IContenderAthlete>> contenderAthletesByContries;
  
    [SerializeField]
    MatchData data;
    HashSet<IContenderAthlete> participatedAthletes;

    public Match(MatchData data, 
      Func<Country, List<IContenderAthlete>> contenderGetter)
    {
      this.Data = data;
      this.EndedSports = new ();
      this.participatedAthletes = new ();
      this.SportRecords = new ();
      this.CurrentState = new (State.NotStartable);
      this.CurrentSport = new (null);
      this.UserAthletes = new ();
      this.FillCountryContenders(contenderGetter);
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
      while (this.SportRecords[sportType].CurrentStage 
        < Match.TOTAL_STAGE) {
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
        this.CurrentState.Value = State.Ended;
        this.CurrentSport.Value = null;
      }
    }

    public List<MatchResult> GetResults()
    {
      var results = new List<MatchResult>();
      if (this.Data.IsSingleSport) {
        int count = this.participatedAthletes.Count + 1;
        foreach (var athlete in this.participatedAthletes) {
          results.Add(
            new MatchResult(
              match: this, athlete: athlete));
        }
        results.Add(
          new MatchResult(
            match: this, 
            athlete: new ConvertedDomesticAthlete(
              this.UserAthletes[this.Data.SportType])));
      }
      else {
        foreach (var country in this.Data.MemberContries) {
          results.Add(
            new MatchResult(
              match: this, country: country));
        }
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
      var athletes = new IContenderAthlete[contenderCount + 1];
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

    void FillCountryContenders(
      Func<Country, List<IContenderAthlete>> contenderGetter)
    {
      this.contenderAthletesByContries = new ();
      foreach (var country in this.Data.MemberContries) {
        this.contenderAthletesByContries.Add(
          country, contenderGetter(country)); 
      }
    }

    void FillSportContenders()
    {
      this.ContenderAthletesBySport = new ();
      if (this.Data.IsSingleSport) {
        this.FillSingleSportContenders(this.Data.SportType);
      }      
      else {
        foreach (var sport in MatchData.DefaultSports) {
          this.FillContendersForSport(sport, this.Data.MatchType == MatchType.Domestic); 
        }
      }
    }

    void FillSingleSportContenders(SportType sportType)
    {
      List<IContenderAthlete> contenders = new ();
      foreach (var country in this.Data.MemberContries) {
        for (int i = 0; i < MatchData.NumberOfAthletesInSingleSport; i++) {
          contenders.Add(this.SelectContender(sportType, country)); 
        }
      }
      this.ContenderAthletesBySport[sportType] = new (contenders);
    }

    void FillContendersForSport(SportType sportType, bool isDomestic)
    {
      if (isDomestic) {
        var korea = this.Data.MemberContries[0];
        List<IContenderAthlete> contenders = new ();
        for (int i = 0; i < DOMESTIC_CONTENDER_COUNT; i++) {
          contenders.Add(this.SelectContender(SportType.SpeedSkating, korea));
        }  
        foreach (var sport in MatchData.DefaultSports) {
          this.ContenderAthletesBySport[sport] = new (contenders);
        }
      }
      else {
        List<IContenderAthlete> contenders = new ();
        foreach (var country in this.Data.MemberContries) {
          contenders.Add(this.SelectContender(sportType, country));
        }
        this.ContenderAthletesBySport[sportType] = new (contenders);
      }
    }

    // TODO: 상대 선수 선택 알고리즘
    IContenderAthlete SelectContender(SportType sportType, Country country)
    {
      var rand = new System.Random();
      var contenders = this.contenderAthletesByContries[country];
      int index = rand.Next(0, contenders.Count);
      IContenderAthlete selected = contenders[index];
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
