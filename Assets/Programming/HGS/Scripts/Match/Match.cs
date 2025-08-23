using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using JYL;

namespace SHG
{
  [Serializable]
  public class Match 
  {
    [SerializeField]
    public MatchData Data { get; private set; }
    [SerializeField]
    public ReactiveProperty<Nullable<SportType>> CurrentSport { get; private set; } 
    [SerializeField]
    public ReactiveCollection<SportType> EndedSports;
    [SerializeField]
    public ReactiveDictionary<SportType, DomAthEntity> UserAthletes { get; set; }
    public ReactiveProperty<bool> IsMatchStartable { get; private set; }
    Dictionary<SportType, ReactiveCollection<IContenderAthlete>> contenderAthletesBySport;
    Dictionary<Country, List<IContenderAthlete>> contenderAthletesByContries;
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
      this.UserAthletes = new ();
      this.FillCountryContenders(contenderGetter);
      this.FillSportContenders();
    }

    public void SelectAthlete(DomAthEntity athlete, SportType sportType)
    {
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
        sportType != this.Data.SportType.Value) {
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
        return (this.UserAthletes.ContainsKey(this.Data.SportType.Value));
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
      this.contenderAthletesBySport = new ();
      if (this.Data.IsSingleSport) {
        this.FillSingleSportContenders(this.Data.SportType.Value);
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
    }

    void FillContendersForSport(SportType sportType)
    {
      List<IContenderAthlete> contenders = new ();
      foreach (var country in this.Data.MemberContries) {
         contenders.Add(this.SelectContender(sportType, country));
      }
      this.contenderAthletesBySport[sportType] = new (contenders);
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
