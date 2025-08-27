using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using EditorAttributes;
using JYL;

namespace SHG 
{
  public class MatchTester : MonoBehaviour {
    [Inject]
    IMatchController matchController;
//    [Inject]
//    IAthleteController athleteController;
    [Inject]
    DomAthService domAthService;
    [Inject]
    ITimeFlowController timeFlowController;
    CompositeDisposable subscribeMatch;

    AthleteDummyData athleteDummyData;
    Match CurrentMatch 
    {
      get => this.currentMatch;
      set {
        this.currentMatch = value;
        if (value != null) {
          this.currentMatchText = JsonUtility.ToJson(value);
        }
        else {
          this.currentMatchText = string.Empty;
        }
      }
    }
    [SerializeField] [ReadOnly] [MessageBox(nameof(currentMatchText), conditionName: nameof(isShowingCurrentMatch), stringInputMode: StringInputMode.Dynamic)]
    Match currentMatch;
    [SerializeField] [HideInInspector]
    string currentMatchText;
    [SerializeField]
    List<MatchData> scheduledMatches;
    [SerializeField]
    List<MatchData> registeredMatch;
    List<IContenderAthlete> koreaContenders;
    [SerializeField] [ReadOnly]
    string[] sports = Enum.GetNames(typeof(SportType));

    [SerializeField] [ReadOnly]
    List<string> athletesNames;
    [SerializeField] [ReadOnly]
    string currentMatchAthleteText;
    [SerializeField] [ReadOnly]
    bool isMatchStartable;
    [SerializeField] [ReadOnly]
    string nextMatchName;
    bool isShowingCurrentMatch => this.currentMatch != null;

    // Start is called before the first frame update
    void Start() {

      this.athleteDummyData = new ();
      this.koreaContenders = new ();
      this.registeredMatch = new ();
//      this.athletesIds = this.athleteController.Athletes.ToList().ConvertAll(
//        athlete => athlete.id.ToString());
//
//      foreach (var athlete in this.athleteController.Athletes) {
//        this.koreaContenders.Add(new ConvertedDomesticAthlete(athlete));
//      }

      //(this.matchController as MatchController).ContenderGetter = this.GetContenderAthletes;
      this.athletesNames = this.domAthService.GetAllAthleteList()
        .ConvertAll(athlete => athlete.entityName);
      this.timeFlowController.WeekInYear.Subscribe(
        week => {
        if (this.matchController.NextMatch.Value != null) {
          var nextMatch = this.matchController.NextMatch.Value.Value;
          this.isMatchStartable = (
            this.timeFlowController.YearPassedAfterStart == nextMatch.DateOfEvent.Year &&
            week == nextMatch.DateOfEvent.Week &&
            this.registeredMatch.Contains(nextMatch));
        }
        else {
          this.isMatchStartable = false;
        }}
        );
      this.matchController.NextMatch.Subscribe(
        arg => {
          if (arg != null) {
            var nextMatch = arg.Value;
            this.nextMatchName = nextMatch.Name;
            this.isMatchStartable = (
              this.timeFlowController.YearPassedAfterStart == nextMatch.DateOfEvent.Year &&
              this.timeFlowController.WeekInYear.Value == nextMatch.DateOfEvent.Week);
          }
          else {
            this.nextMatchName = string.Empty;
            this.isMatchStartable = false;
          }
        });
      this.scheduledMatches = new (this.matchController.ScheduledMatches);
      this.matchController.ScheduledMatches
        .ObserveRemove()
        .Subscribe(removed => {
          this.scheduledMatches = new (this.matchController.ScheduledMatches);
          });

      this.matchController.CurrentMatch.Subscribe(
        match => {
          this.CurrentMatch = match;
          if (match != null) {
            this.subscribeMatch?.Dispose();
            this.subscribeMatch = new ();
            match.ObserveEveryValueChanged(
              match => UniRx.Unit.Default)
            .Subscribe(_ => 
              this.currentMatchText = JsonUtility.ToJson(this.currentMatch))
            .AddTo(this.subscribeMatch);
            
            match.UserAthletes.ObserveAdd()
            .Select(_ => UniRx.Unit.Default)
            .Merge(match.UserAthletes.ObserveRemove()
              .Select(_ => UniRx.Unit.Default))
            .Subscribe(_ => {
              this.currentMatchAthleteText = this.GetMatchAthleteText(match.UserAthletes);
              })
            .AddTo(this.subscribeMatch);
          }
          else {
            this.currentMatchText = string.Empty;
            this.currentMatchAthleteText = string.Empty;
          }
        });

      var onAdded = this.matchController.RegisteredMatches
        .ObserveAdd()
        .Select(_ => UniRx.Unit.Default);
      var onRemoved = this.matchController.RegisteredMatches
        .ObserveRemove()
        .Select(_ => UniRx.Unit.Default);
      onAdded.Merge(onRemoved)
        .Subscribe(_ => this.registeredMatch = this.matchController.RegisteredMatches.ToList());
    }

    string GetMatchAthleteText(ReactiveDictionary<SportType, DomAthEntity> matchAthletes)
    {
      var builder = new StringBuilder();
      foreach (var (sport, athlete) in matchAthletes) {
        builder.Append($"[{sport}: {athlete.id}] "); 
      }
      return (builder.ToString());
    }

    List<IContenderAthlete> GetContenderAthletes(Country country)
    {
      if (country.Name != "korea") {
        return (this.athleteDummyData.Althetes[country]);
      }
      else {
        return (this.koreaContenders);
      }
    }


    [Button(nameof(isMatchStartable), ConditionResult.EnableDisable)]
    void EnterMatch()
    {
      this.matchController.EnterNextMatch(); 
    }

    [Button]
    void RecruteAthlete(int index)
    {
      this.domAthService.RecruitAthlete(this.athletesNames[index]);
    }

    [Button]
    void RegisterMatchAt(int index)
    {
      var match = this.scheduledMatches[index];
      this.matchController.Register(match);
    }

    [Button]
    void UnRegisterMatchAt(int index)
    {
      var match = this.scheduledMatches[index];
      this.matchController.UnRegister(match);
    }

    [Button]
    void RegisterAthlete(int id, int sportIndex)
    {
      if (sportIndex < 0 || sportIndex >= Enum.GetValues(typeof(SportType)).Length) {
        throw (new ArgumentException($"{nameof(RegisterAthlete)}: {sportIndex} is in range of {nameof(SportType)}"));
      }
      var sportType = (SportType)sportIndex;
//      if (!this.athleteController.TryGetAthleteBy(
//          id, out DomAthEntity athlete)) {
//        throw (new ApplicationException(nameof(RegisterAthlete)));
//      }
//      this.currentMatch.SelectAthlete(
//        athlete, sportType);
    }

    [Button]
    void UnRegisterAthlete(int sportIndex)
    {
      if (sportIndex < 0 || sportIndex >= Enum.GetValues(typeof(SportType)).Length) {
        throw (new ArgumentException($"{nameof(UnRegisterAthlete)}: {sportIndex} is in range of {nameof(SportType)}"));
      }
      var sportType = (SportType)sportIndex;
      if (!this.currentMatch.UserAthletes.ContainsKey(sportType)) {
        throw (new ArgumentException($"{nameof(UnRegisterMatchAt)}: {sportType} is not regiested athlete")); 
        }
      this.currentMatch.UnSelectAthlete(sportType);
    }
  }
}
