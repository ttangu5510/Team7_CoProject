using System;
using System.Collections.Generic;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;

namespace SHG
{
  public class MatchViewResultScreen 
  {
    const string GOLD_MEDAL_ICON = "<sprite index=0>";
    const string SILVER_MEDAL_ICON = "<sprite index=1>";
    const string BRONZE_MEDAL_ICON = "<sprite index=2>";

    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    ContainerView container;
    ScrollRect scrollView;

    public MatchViewResultScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;
      this.container = this.view.GetItem<ContainerReference>(
        (int)ContainerRole.RankingContainer).Container;

      this.scrollView = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.ScrollView).Object.GetComponent<ScrollRect>();

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
      this.scrollView.verticalNormalizedPosition = 1f;
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
          1 => GOLD_MEDAL_ICON,
          2 => SILVER_MEDAL_ICON,
          3 => BRONZE_MEDAL_ICON,
          _ => string.Empty
        };
        view.SetRawTextByRole(
          (int)TextRole.MedalLabel, rankText);
      }
      else {
        view.SetState((int)StateRole.International);
        var medals = result.GetMedalCounts();
        view.SetRawTextByRole(
          (int)TextRole.MedalLabel,
          $"{GOLD_MEDAL_ICON} {medals[0]} {SILVER_MEDAL_ICON} {medals[1]} {BRONZE_MEDAL_ICON} {medals[2]}");
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
