using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UnityEngine.UI;
using JYL;

namespace SHG
{
  public class MatchViewAthleteListScreen 
  {
    
    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    ContainerView contenderContainer;
    ScrollRect scrollView;
    CompositeDisposable disposables;

    public MatchViewAthleteListScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState, 
      StatefulComponent view,
      ContainerView contenderContainer)
    {
      this.disposables = new ();
      this.parentState = parentState;
      this.view = view;
      this.contenderContainer = contenderContainer;
      this.scrollView = view.GetItem<ObjectReference>(
        (int)ObjectRole.ScrollView).Object.GetComponent<ScrollRect>();
  
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.UserAthleteButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => 
          parentState.Value = MatchViewPresenter.ViewState.AthleteSelect)
        .AddTo(this.disposables);
      
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.BackButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => 
          parentState.Value = MatchViewPresenter.ViewState.SportSelect)
        .AddTo(this.disposables);
    }

    public void UpdateHeader(Match match, SportType sportType)
    {

      int year = (ITimeFlowController.START_YEAR + match.Data.DateOfEvent.Year - 1) % 100;
      this.view.SetRawTextByRole(
        (int)TextRole.MatchTitle,
        $"{year}년 {match.Data.Name}");

      this.view.SetRawTextByRole(
        (int)TextRole.SportLabel, 
        MatchData.GetSportTypeString(sportType));

      if (match.UserAthletes.TryGetValue(
          sportType, out DomAthEntity athlete)) {
        this.view.SetState((int)StateRole.Registered);
        this.view.SetRawTextByRole(
          (int)TextRole.AthleteNameLabel,
          athlete.entityName);
        this.view.SetRawTextByRole(
          (int)TextRole.NationalityLabel,
          "대한민국");
        this.view.SetRawTextByRole(
          (int)TextRole.StatLabel,
          Match.GetAverageStatTextOf(athlete.stats));
      }
      else {
        this.view.SetState((int)StateRole.UnRegistered);
      }
    }

    public void UpdateList(Match match, SportType sportType)
    {
      this.contenderContainer.Clear();
      this.contenderContainer.FillWithItems(
        match.ContenderAthletesBySport[sportType],
        (view, contender) => {
          view.SetRawTextByRole(
            (int)TextRole.AthleteNameLabel,
            contender.Name); 
          view.SetRawTextByRole(
            (int)TextRole.NationalityLabel,
            contender.Country.Name);
          view.SetRawTextByRole(
            (int)TextRole.StatLabel,
            Match.GetAverageStatTextOf(contender.Stats));
        });
      this.scrollView.verticalNormalizedPosition = 1.0f;
    }
  }
}
