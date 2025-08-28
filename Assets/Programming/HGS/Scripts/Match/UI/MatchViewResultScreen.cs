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
      this.container.Clear();
      this.container.FillWithItems(results, this.UpdateRow);
      foreach (var result in results) {
        Debug.Log($"country: {result.Country.Name}, type: {result.Type}"); 
        if (result.Type == MatchResult.ResultType.Domestic) {
          Debug.Log($"rank: {result.GetDomesticRank()}");
        }
        var medals = result.GetMedalCounts();
        Debug.Log($"({medals[0]}, {medals[1]}, {medals[2]})");
      }
    }

    void UpdateRow(StatefulComponent view, MatchResult result)
    {
      bool isDomestic = result.Type == MatchResult.ResultType.Domestic;

      if (isDomestic) {
        view.SetState((int)StateRole.Domestic);
      }
      else {
        view.SetState((int)StateRole.International);
      }
    }
  }
}
