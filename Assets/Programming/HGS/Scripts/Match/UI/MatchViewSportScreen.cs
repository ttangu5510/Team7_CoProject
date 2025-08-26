using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;

namespace SHG
{
  public class MatchViewSportScreen 
  {
    static Sprite[] SPORT_ICONS;

    ContainerView sportsGrid;
    ReactiveProperty<Nullable<SportType>> selectedSport;
    CompositeDisposable disposables;
    HashSet<Button> subscribedButtons;

    static MatchViewSportScreen()
    {
      SPORT_ICONS = new Sprite[Enum.GetValues(typeof(SportType)).Length];
      foreach (var sport in MatchData.DefaultSports) {
        string filePath = string.Empty;
        switch (sport) {
          case SportType.SkiJumping:
            filePath = "ski_icon";
            break;
          case SportType.Skeleton:
            filePath = "skeleton_icon";
            break;
          case SportType.FigureSkating:
            filePath = "figure_skate_icon";
            break;
          case SportType.SpeedSkating:
            filePath = "speed_skate_icon";
            break;
          default: 
            continue;
        }
        SPORT_ICONS[(int)sport] = Resources.Load<Sprite>(filePath);
      }
    }

    public MatchViewSportScreen(
      ReactiveProperty<Nullable<SportType>> selectedSport,
      ContainerView sportsGrid)
    {
      this.subscribedButtons = new ();
      this.selectedSport = selectedSport;
      this.sportsGrid = sportsGrid;
      this.disposables = new ();
    }

    public void FillSportsGrid(Match match)
    {
      bool isSingleSport = match.Data.IsSingleSport;
      this.sportsGrid.Clear();
      this.sportsGrid.FillWithItems(
        MatchData.DefaultSports,
        (view, sport) => {
          view.SetRawTextByRole( 
            (int)TextRole.SportLabel,
            MatchData.GetSportTypeString(sport));
          view.GetItem<ImageReference>(
            (int)ImageRole.SportIcon).Image.sprite = SPORT_ICONS[(int)sport];
          bool isRegistered = match.UserAthletes.ContainsKey(sport);
          if (isRegistered) {
            view.SetState((int)StateRole.Registered);
          }
          else if (isSingleSport && sport != match.Data.SportType) {
            view.SetState((int)StateRole.UnAvailable);
          }
          else {
            view.SetState((int)StateRole.Available);
          }
          view.GetItem<ImageReference>(
            (int)ImageRole.SportIcon).Image.sprite = SPORT_ICONS[(int)sport];
          view.GetItem<ImageReference>(
            (int)ImageRole.Background).Image.color = this.GetSportBackgroundColor(sport);
          
          var registerButton = view.GetItem<ButtonReference>((int)ButtonRole.RegisterButton).Button;
          if (!this.subscribedButtons.Contains(registerButton)) {
            view.GetItem<ButtonReference>((int)ButtonRole.RegisterButton).Button
              .OnClickAsObservable()
              .Subscribe(_ => {
                  this.selectedSport.Value = sport;
                })
              .AddTo(this.disposables);
              this.subscribedButtons.Add(registerButton);
          }
        });
    }

    Color GetSportBackgroundColor(SportType sport)
    {
      switch (sport) {
        case SportType.FigureSkating:
          return (new Color((float)170/255, (float)199/255, (float)221/255));
        case SportType.SkiJumping:
          return (new Color((float)234/255, (float)201/255, (float)201/255));
        case SportType.Skeleton:
          return (new Color((float)198/255, (float)245/255, (float)212/255));
        case SportType.SpeedSkating:
          return (new Color((float)244/255, (float)235/255, (float)199/255));
        default:
          return (Color.gray);
      }
    }

  }

}
