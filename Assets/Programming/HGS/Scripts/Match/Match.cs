using System;
using System.Linq;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using JYL;

namespace SHG
{
  [Serializable]
  public class Match 
  {
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
    [SerializeField]
    public ReactiveDictionary<SportType, DomAthEntity> UserAthletes { get; set; }
    public ReactiveProperty<bool> IsMatchStartable { get; private set; }
    public Dictionary<SportType, ReactiveCollection<IContenderAthlete>> ContenderAthletesBySport { get; private set; }
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
      this.IsMatchStartable = new (false);
      this.CurrentSport = new (null);
      this.UserAthletes = new ();
      this.FillCountryContenders(contenderGetter);
      this.FillSportContenders();
    }

    //TODO: Calc stats 
    public static string GetAverageStatTextOf(AthleteStats stat) {
      return ("B");
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
      this.IsMatchStartable.Value = this.IsStartable();
    }

    public void UnSelectAthlete(SportType sportType)
    {
      this.UserAthletes.Remove(sportType);
      this.IsMatchStartable.Value = false;
    }

    public void StartSport(SportType sportType)
    {
      #if UNITY_EDITOR
      if (this.Data.IsSingleSport &&
        sportType != this.Data.SportType) {
        throw (new ArgumentException($"{nameof(StartSport)}: {sportType} is not selectable"));
      } 
      else if (this.EndedSports.Contains(sportType)) {
        throw (new ArgumentException($"{nameof(StartSport)}: {sportType} is ended"));
      }
      #endif
      this.CurrentSport.Value = sportType;
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
      this.CurrentSport.Value = null;
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
          this.FillContendersForSport(sport); 
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

    void FillContendersForSport(SportType sportType)
    {
      List<IContenderAthlete> contenders = new ();
      foreach (var country in this.Data.MemberContries) {
         contenders.Add(this.SelectContender(sportType, country));
      }
      this.ContenderAthletesBySport[sportType] = new (contenders);
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
  }
}
