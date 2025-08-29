using System;
using System.Collections.Generic;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UnityEngine;
namespace SHG
{
  public class MatchViewResultScreen 
  {

    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    ContainerView container;

    public MatchViewResultScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;
      this.container = this.view.GetItem<ContainerReference>(
        (int)ContainerRole.RankingContainer).Container;

      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.NextButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => {
          if (this.parentState.Value == 
            MatchViewPresenter.ViewState.Result) {
          this.parentState.Value = MatchViewPresenter.ViewState.None;
          }});
    }

    public void UpdateView(Match match)
    {
      bool isDomestic = match.Data.IsSingleSport || 
        match.Data.MatchType == MatchType.Domestic;

      if (isDomestic) {
        this.view.SetState((int)StateRole.Domestic);
      }
      else {
        this.view.SetState((int)StateRole.International);
      }
      List<MatchResult> results = match.GetResults();
      results.Sort(this.CompareMatchResult);
      this.container.Clear();
      this.container.FillWithItems(
        results,
        (view, result) => this.UpdateRow(
          view, result, results.IndexOf(result) + 1));
    }

    void UpdateRow(StatefulComponent view, MatchResult result, int rank)
    {
      bool isDomestic = result.Type == MatchResult.ResultType.Domestic;

      if (isDomestic) {
        view.SetState((int)StateRole.Domestic);
        view.SetRawTextByRole(
          (int)TextRole.AthleteNameLabel,
          $"{result.GetDomesticAthlete().Name}");
        string rankText = rank switch {
          1 => "Gold",
          2 => "Silver",
          3 => "Bronze",
          _ => string.Empty
        };
        view.SetRawTextByRole(
          (int)TextRole.MedalLabel, rankText);;
      }
      else {
        view.SetState((int)StateRole.International);
        var medals = result.GetMedalCounts();
        view.SetRawTextByRole(
          (int)TextRole.MedalLabel,
          $"G: {medals[0]} S: {medals[1]} B: {medals[2]}");
        view.SetRawTextByRole(
          (int)TextRole.NationalityLabel, result.Country.Name);
      }
      view.SetRawTextByRole(
        (int)TextRole.RankLabel, $"{rank}위");
      int point = result.CalcPoint();
      view.SetRawTextByRole(
        (int)TextRole.TotalLabel, $"{point}");
    }

    int CompareMatchResult(MatchResult lhs, MatchResult rhs)
    {
      var lhsPoint = lhs.CalcPoint();
      var rhsPoint = rhs.CalcPoint();
      if (lhsPoint != rhsPoint) {
        return (lhsPoint > rhsPoint ? -1: 1);
      }
      var lhsRank = lhs.GetHighestRank();
      var rhsRank = rhs.GetHighestRank();
      return (lhsRank > rhsRank ? - 1: 1);
    }
  }
}
