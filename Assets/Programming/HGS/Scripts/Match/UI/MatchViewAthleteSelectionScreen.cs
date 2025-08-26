using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using JYL;

namespace SHG
{
  public class MatchViewAthleteSelectionScreen 
  {
    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    ContainerView containerView;
    Dictionary<Button, (DomAthEntity athlete, IDisposable subscription)> subscribedButtons;

    public MatchViewAthleteSelectionScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState, 
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;
      this.subscribedButtons = new ();
      this.containerView = view.GetItem<ContainerReference>(
        (int)ContainerRole.AthleteContainer).Container;
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.CloseButton).Button
        .OnClickAsObservable()
        .Subscribe(
          _ => parentState.Value = MatchViewPresenter.ViewState.AthleteList);
    }

    public void UpdateList(
      Match match,
      SportType sportType,
      ReactiveCollection<DomAthEntity> athletes,
      ReactiveDictionary<SportType, DomAthEntity> registeredAthletes)
    {
      this.containerView.Clear();
      this.containerView.FillWithItems(
        athletes,
        (view, athlete) => {
          string nameText = $"{athlete.entityName} ({athlete.curAge})";
          view.SetRawTextByRole(
            (int)TextRole.AthleteNameLabel, nameText);

          string statText = $"평균 능력치: {Match.GetAverageStatTextOf(athlete.stats)}";
          view.SetRawTextByRole(
            (int)TextRole.StatLabel, statText);

          bool isRegistered = registeredAthletes.Values.Contains(athlete);
          if (isRegistered) {
            view.SetState((int)StateRole.Registered);
            view.SetRawTextByRole(
              (int)TextRole.RegisterButtonLabel,
              "배치완료");
          }
          else {
            view.SetState((int)StateRole.UnRegistered);
            view.SetRawTextByRole(
              (int)TextRole.RegisterButtonLabel,
              "배치하기");
          }

          var button = view.GetItem<ButtonReference>(
            (int)ButtonRole.RegisterButton).Button;
          if (this.subscribedButtons.TryGetValue(button,
              out (DomAthEntity athlete, IDisposable subscription) exist)) {
            if (exist.athlete == athlete) {
              return; 
            }
            else {
              exist.subscription.Dispose();
            }
          }
          var subscription =  button.OnClickAsObservable()
            .Subscribe(_ => this.OnClickRegisterAthlete(athlete, match, sportType));
          this.subscribedButtons[button] = (athlete, subscription);
        });
    }

    void OnClickRegisterAthlete(DomAthEntity athlete, Match match, SportType sport)
    {
      this.parentState.Value = MatchViewPresenter.ViewState.AthleteList;
      match.SelectAthlete(athlete, sport);
    }
  }
}
