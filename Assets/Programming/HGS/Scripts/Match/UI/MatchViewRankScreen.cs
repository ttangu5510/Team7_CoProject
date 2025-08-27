using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UnityEngine;

namespace SHG
{
  public class MatchViewRankScreen 
  {
    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    Match currentMatch;

    public MatchViewRankScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.NextButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.OnClickNext());
    }

    public void UpdateView(in Match match)
    {
      if (match.CurrentSport.Value == null) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(UpdateView)}: {nameof(match.CurrentSport)} is null"));
        #else
        return;
        #endif
      }
      this.currentMatch = match;
      var sportType = match.CurrentSport.Value.Value;
      this.view.SetRawTextByRole(
        (int)TextRole.SportLabel,
        MatchData.GetSportTypeString(sportType)); 
      int rank = this.GetUserRank(match, sportType);
      this.view.SetRawTextByRole(
        (int)TextRole.RankLabel, $"{rank}위");
    }

    int GetUserRank(in Match match, SportType sportType)
    {
      var sportRecord = match.SportRecords[sportType]; 
      var athlete = match.UserAthletes[sportType];
      var record = sportRecord.GetRecordOf(athlete);
      return (record.Rank);
    }

    void OnClickNext()
    {
      if (this.currentMatch == null) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(OnClickNext)}: {nameof(this.currentMatch)} is null"));
      #else 
        return;
      #endif
      } 
      if (this.currentMatch.IsLastSport()) {
        this.parentState.Value = MatchViewPresenter.ViewState.Result;
      }
      else {
        this.parentState.Value = MatchViewPresenter.ViewState.Record;
      }
      this.currentMatch.EndCurrentSport();
    }
  }
}
