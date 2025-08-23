using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UniRx;
using EditorAttributes;
using Void = EditorAttributes.Void;
using JYL;

namespace SHG 
{
  public class MatchTester : MonoBehaviour {
    [Inject]
    IMatchController matchController;
    [Inject]
    IAthleteController athleteController;
    [Inject]
    ITimeFlowController timeFlowController;

    AthleteDummyData athleteDummyData;
    [SerializeField]
    string nextMatchName;
    [SerializeField]
    Match currentMatch;
    [SerializeField] [ReadOnly]
    bool isMatchWeek;
    [SerializeField]
    List<MatchData> scheduledMatches;
    [SerializeField]
    List<IContenderAthlete> koreaContenders;

    // Start is called before the first frame update
    void Start() {

      this.athleteDummyData = new ();
      this.koreaContenders = new ();

      foreach (var athlete in this.athleteController.Athletes) {
        this.koreaContenders.Add(new ConvertedDomesticAthlete(athlete));
      }

      (this.matchController as MatchController).ContenderGetter = this.GetContenderAthletes;
      this.timeFlowController.WeekInYear.Subscribe(
        week => {
        if (this.matchController.NextMatch.Value != null) {
          var nextMatch = this.matchController.NextMatch.Value.Value;
          this.isMatchWeek = (
            this.timeFlowController.YearPassedAfterStart == nextMatch.DateOfEvent.Year &&
            week == nextMatch.DateOfEvent.Week);
        }
        else {
          this.isMatchWeek = false;
        }}
        );
      this.matchController.NextMatch.Subscribe(
        arg => {
          if (arg != null) {
            var nextMatch = arg.Value;
            this.nextMatchName = nextMatch.Name;
            this.isMatchWeek = (
              this.timeFlowController.YearPassedAfterStart == nextMatch.DateOfEvent.Year &&
              this.timeFlowController.WeekInYear.Value == nextMatch.DateOfEvent.Week);
          }
          else {
            this.nextMatchName = string.Empty;
            this.isMatchWeek = false;
          }
        });
      this.scheduledMatches = new (this.matchController.ScheduledMatches);
      this.matchController.ScheduledMatches
        .ObserveRemove()
        .Subscribe(removed => {
          this.scheduledMatches = new (this.matchController.ScheduledMatches);
          });

      this.matchController.CurrentMatch.Subscribe(
        match => this.currentMatch = match);
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


    [Button(nameof(isMatchWeek), ConditionResult.EnableDisable)]
    void StartMatch()
    {
      this.matchController.StartNextMatch(); 
    }
  }
}
