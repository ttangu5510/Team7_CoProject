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
  using StatType = IAthleteController.StatType;

  public class MatchPrepareViewAthleteSelectionScreen 
  {
    ReactiveProperty<MatchPrepareViewPresenter.ViewState> parentState;
    ReactiveProperty<Nullable<SportType>> selectedSport;
    StatefulComponent view;
    ContainerView athleteContainer;
    ContainerView statContainer;
    Dictionary<Button, (DomAthEntity athlete, IDisposable subscription)> subscribedRegisterButtons;
    Dictionary<Button, (DomAthEntity athlete, IDisposable subscription)> subscribedCellButtons;
    HorizontalNavigator navigator;
    ReactiveProperty<DomAthEntity> athleteToShowStat;
    Button backButton;
    CompositeDisposable disposables;

    public MatchPrepareViewAthleteSelectionScreen(
      ReactiveProperty<MatchPrepareViewPresenter.ViewState> parentState, 
      ReactiveProperty<Nullable<SportType>> selectedSport,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.selectedSport = selectedSport;
      this.view = view;
      this.disposables = new ();
      this.athleteToShowStat = new (null);
      this.subscribedRegisterButtons = new ();
      this.subscribedCellButtons = new ();
      this.backButton = this.view.GetItem<ButtonReference>(
        (int)ButtonRole.BackButton).Button;

      this.navigator = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.Navigator).Object.GetComponent<HorizontalNavigator>();

      this.navigator.CurrentNavigationIndex
        .Subscribe(index => 
            this.backButton.gameObject.SetActive(index != 0))
        .AddTo(this.disposables);

      this.backButton.OnClickAsObservable()
        .Subscribe(_ => {
          if (this.navigator.CurrentNavigationIndex.Value > 0) {
            this.athleteToShowStat.Value = null;
          }})
        .AddTo(this.disposables);

      this.athleteContainer = view.GetItem<ContainerReference>(
        (int)ContainerRole.AthleteContainer).Container;

      this.statContainer = view.GetItem<ContainerReference>(
        (int)ContainerRole.StatContainer).Container;

      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.CloseButton).Button
        .OnClickAsObservable()
        .Subscribe(
          _ => parentState.Value = MatchPrepareViewPresenter.ViewState.AthleteList)
        .AddTo(this.disposables);

      this.athleteToShowStat.Subscribe(this.OnSelectedAthleteChanged)
      .AddTo(this.disposables);
    }

    public void UpdateList(
      Match match,
      SportType sportType,
      IList<DomAthEntity> athletes,
      ReactiveDictionary<SportType, DomAthEntity> registeredAthletes)
    {
      this.athleteContainer.Clear();
      this.athleteContainer.FillWithItems(
        athletes,
        (view, athlete) => {
          string nameText = $"{athlete.entityName} ({athlete.curAge})";
          view.SetRawTextByRole(
            (int)TextRole.AthleteNameLabel, nameText);

          string statText = $"평균 능력치: {IAthleteController.GetAverageStatTextOf(athlete.stats)}";
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

          this.UpdateRegisterButton(
            button: 
            view.GetItem<ButtonReference>(
              (int)ButtonRole.RegisterButton).Button,
            athlete: athlete,
            match: match,
            sportType: sportType);

          this.UpdateCellButton(
            button: view.GetItem<ButtonReference>
            ((int)ButtonRole.CellButton).Button,
            athlete: athlete);
        });
    }

    void UpdateAthleteStat(DomAthEntity athlete)
    {
      string nameText = $"{athlete.entityName} ({athlete.curAge}세)";
      this.view.SetRawTextByRole(
        (int)TextRole.AthleteNameLabel, nameText);
      string gradeText = $"등급: {athlete.affiliation}";
      this.view.SetRawTextByRole(
        (int)TextRole.AthleteGradeLabel, gradeText);
      string potentialText = $"최대 성장 가능성: {athlete.maxGrade}";
      this.view.SetRawTextByRole(
        (int)TextRole.AthletePotentionLabel, potentialText);
      int retireYear = athlete.retireAge - athlete.curAge.Value;
      string retireText = $"은퇴까지 {retireYear}년";
      this.view.SetRawTextByRole(
        (int)TextRole.AthleteRetireLabel, retireText);

      this.statContainer.Clear();
      this.statContainer.FillWithItems(
        (StatType[])Enum.GetValues(typeof(StatType)),
        (view, stat) => {
          view.SetRawTextByRole(
            (int)TextRole.NameLabel,
            IAthleteController.GetNameof(stat));
          view.GetItem<SliderReference>(
            (int)SliderRole.StatSlider).Slider.value = IAthleteController.GetValueOf(stat, athlete.stats) / 100f;
          
          view.SetRawTextByRole(
            (int)TextRole.StatLabel,
            IAthleteController.GetAverageStatTextOf(stat, athlete.stats));
        });
    }

    void UpdateRegisterButton(Button button, DomAthEntity athlete, Match match, SportType sportType)
    {
      if (this.subscribedRegisterButtons.TryGetValue(button,
          out (DomAthEntity athlete, IDisposable subscription) existRegisterButton)) {
        if (existRegisterButton.athlete == athlete) {
          return; 
        }
        else {
          existRegisterButton.subscription.Dispose();
        }
      }
      var subscription = button.OnClickAsObservable()
        .Subscribe(_ => 
          this.OnClickRegisterAthlete(athlete, match));
      this.subscribedRegisterButtons[button] = (athlete, subscription);
    }

    public void OnMatchEnded()
    {
      foreach (var (athlete, subscription) in this.subscribedRegisterButtons.Values) {
        subscription.Dispose();      
      }
      this.subscribedRegisterButtons.Clear();
    }

    void UpdateCellButton(Button button, DomAthEntity athlete)
    {

      if (this.subscribedCellButtons.TryGetValue(button, 
          out (DomAthEntity athlete, IDisposable subscription) exist)) {
        if (athlete == exist.athlete) {
          return ;
        }
        else {
          exist.subscription.Dispose();
        }
      }
      var cellButtonSubscription = button.OnClickAsObservable()
        .Subscribe(_ => 
          this.athleteToShowStat.Value = athlete);
      this.subscribedCellButtons[button] = (athlete, cellButtonSubscription);
    }

    void OnSelectedAthleteChanged(DomAthEntity selected)
    {
      if (selected != null) {
        this.UpdateAthleteStat(selected);
        if (this.navigator.CurrentNavigationIndex.Value < 1) {
          this.navigator.Push();
        } 
        this.view.SetState((int)StateRole.AthleteStat);
      }
      else {
        if (this.navigator.CurrentNavigationIndex.Value > 0)  {
          this.navigator.Pop();
        }
        this.view.SetState((int)StateRole.AthleteList);
      }
    }

    void OnClickRegisterAthlete(DomAthEntity athlete, Match match)
    {
      if (this.selectedSport.Value == null) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(OnClickRegisterAthlete)}: {nameof(this.selectedSport)} is null"));
      #else
        return ;
      #endif
      }
      this.parentState.Value = MatchPrepareViewPresenter.ViewState.AthleteList;
      match.SelectAthlete(athlete, this.selectedSport.Value.Value);
    }
  }
}
