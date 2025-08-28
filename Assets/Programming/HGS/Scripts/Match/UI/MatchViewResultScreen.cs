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
      List<MatchResult> results = match.GetResults();
      Debug.Log($"results: {results.Count}");
      foreach (var result in results) {
        Debug.Log($"country: {result.Country.Name}, type: {result.Type}"); 
        if (result.Type == MatchResult.ResultType.Domestic) {
          Debug.Log($"rank: {result.GetDomesticRank()}");
        }
        var medals = result.GetMedalCounts();
        Debug.Log($"({medals[0]}, {medals[1]}, {medals[2]})");
      }
    }
  }
}
