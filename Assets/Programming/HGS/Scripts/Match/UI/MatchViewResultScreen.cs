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
          this.parentState.Value = MatchViewPresenter.ViewState.Reward;
          }});
    }

    public void UpdateView(Match match)
    {
      bool isDomestic = match.Data.IsDomestic;
      if (match.Data.IsSingleSport) {
        this.view.SetState((int)StateRole.SingleSportMatch);
      }
      else if (match.Data.IsDomestic) {
        this.view.SetState((int)StateRole.Domestic);
      }
      else {
        this.view.SetState((int)StateRole.International);
      }
      this.container.Clear();
      this.container.FillWithItems(
        match.Results,
        (view, result) => this.UpdateRow(
          view: view, 
          result: result, 
          match: match,
          rank: match.Results.IndexOf(result) + 1));
      this.scrollView.verticalNormalizedPosition = 1f;
    }

    void UpdateRow(StatefulComponent view, MatchResult result, Match match, int rank)
    {
      if (match.Data.IsSingleSport) {
        view.SetState((int)StateRole.SingleSportMatch);
      }
      else if (match.Data.IsDomestic) {
        view.SetState((int)StateRole.Domestic);
      }
      else {
        view.SetState((int)StateRole.International);
      }

      if (match.Data.IsSingleSport) {
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
        var medals = result.GetMedalCounts();
        view.SetRawTextByRole(
          (int)TextRole.MedalLabel,
          $"{GOLD_MEDAL_ICON} {medals[0]} {SILVER_MEDAL_ICON} {medals[1]} {BRONZE_MEDAL_ICON} {medals[2]}");
      }
      view.SetRawTextByRole(
        (int)TextRole.GroupLabel, result.Group.Name);
      view.SetRawTextByRole(
        (int)TextRole.RankLabel, $"{rank}위");
      int point = result.CalcPoint();
      view.SetRawTextByRole(
        (int)TextRole.TotalLabel, $"{point}");
    }
  }
}
