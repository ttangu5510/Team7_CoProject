using System;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;

namespace SHG
{
  public class MatchViewRewardScreen 
  {
    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    ContainerView medalContainer;
    public Button ConfirmButton { get; private set; }

    static MatchResult.MedalType[] ALL_MEDALS = new MatchResult.MedalType[] {
      MatchResult.MedalType.Gold,
      MatchResult.MedalType.Silver,
      MatchResult.MedalType.Bronze,
    };

    public MatchViewRewardScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.view = view;
      this.parentState = parentState;
      this.medalContainer = view.GetItem<ContainerReference>(
        (int)ContainerRole.MedalContainer).Container;
      this.ConfirmButton = view.GetItem<ButtonReference>(
        (int)ButtonRole.ConfirmButton).Button;
    }

    public void UpdateView(Match match)
    {
      var medalCounts = match.UserResult.GetMedalCounts();
      var point = match.UserResult.CalcPoint();
      var rank = match.Results.IndexOf(match.UserResult) + 1;
      view.SetRawTextByRole(
        (int)TextRole.MatchTitle, match.Data.Name);
      view.SetRawTextByRole(
        (int)TextRole.RankLabel, $"{rank}등");

      this.medalContainer.Clear(); 
      this.medalContainer.FillWithItems(
        ALL_MEDALS, 
        (view, medal) => this.UpdateMedalCell(
          view: view,
          medalType: medal, 
          count: medalCounts[ALL_MEDALS.IndexOf(medal)]));
      this.view.SetRawTextByRole(
        (int)TextRole.TotalLabel, $"{point}점");

      foreach (var reward in match.Data.Rewards) {
        this.UpdateRewardCell(match, reward.type, reward.amount); 
      }
    }

    void UpdateRewardCell(Match match, ResourceType resourceType, int amount)
    {
      var index = Array.FindIndex(
        match.Data.Rewards,
        reward => reward.type == resourceType);
      string unit = resourceType switch {
        ResourceType.Money => "G",
        ResourceType.Coin => "개",
        _ => string.Empty
      };
      TextRole role = resourceType switch {
        ResourceType.Money => TextRole.MoneyLabel,
        ResourceType.Fame => TextRole.FameLabel,
        ResourceType.Coin => TextRole.CoinLabel
      };
      this.view.SetRawTextByRole(
        (int)role, $"{amount}{unit}");
    }

    void UpdateMedalCell(StatefulComponent view, MatchResult.MedalType medalType, int count)
    {
      switch (medalType) {
        case MatchResult.MedalType.Gold:
          view.SetState((int)StateRole.Gold);
          break;
        case MatchResult.MedalType.Silver:
          view.SetState((int)StateRole.Silver);
          break;
        case MatchResult.MedalType.Bronze:
          view.SetState((int)StateRole.Bronze);
          break;
      }
      var point = (int)medalType * count;

      view.SetRawTextByRole(
        (int)TextRole.CountLabel, $"{count}개");
      view.SetRawTextByRole(
        (int)TextRole.PointLabel, $"{point}점");
    }
  }
}
