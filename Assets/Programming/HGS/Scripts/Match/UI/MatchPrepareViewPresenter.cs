using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UniRx.Triggers;
using Zenject;
using JYL;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent))]
  public class MatchPrepareViewPresenter : MonoBehaviour
  {
    public enum ViewState
    {
      None,
      SportSelect = StateRole.SportSelect,
      AthleteList = StateRole.AthleteList,
      AthleteSelect = StateRole.AthleteSelect
    }

    public ReactiveProperty<bool> IsShowing { get; private set; }

    [Inject]
    IMatchController matchController;
//    [Inject]
    IAthleteController athleteController => DummyAthleteController.Instance;
    [Inject]
    DomAthService domAthService;

    MatchPrepareViewSportScreen sportScreen;
    MatchPrepareViewAthleteListScreen athleteListScreen;
    MatchPrepareViewAthleteSelectionScreen athleteSelectionScreen;

    StatefulComponent view;
    CompositeDisposable disposables;
    CompositeDisposable matchSubscription;
    ReactiveProperty<Nullable<SportType>> selectedSport;
    ReactiveProperty<ViewState> currentState;
    Button startButton;

    void Awake()
    {
      this.disposables = new ();
      this.selectedSport = new (null);
      this.view = this.GetComponent<StatefulComponent>();
      this.startButton = this.view.GetItem<ButtonReference>(
        (int)ButtonRole.ConfirmButton).Button;
    }

    // Start is called before the first frame update
    void Start()
    {
      this.InitStates();
      this.InitScreens();
      this.SubscribeMatch();
      this.SubscribeSportSelection();
      this.startButton.OnClickAsObservable()
        .Subscribe(_ => {
            var match = this.matchController.CurrentMatch.Value;
            if (match != null) {
              match.StartMatch();
            }});
      this.view.SetState((int)StateRole.Hidden);

      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposables?.Dispose());
    }

    void InitStates()
    {
      this.currentState = new (ViewState.None);
      this.currentState
        .Subscribe(state => {
          if (state != ViewState.None) {
            this.view.SetState((int)state);
          }
          this.OnViewStateChanged();
          })
      .AddTo(this.disposables);
      this.IsShowing = new (false);
      this.IsShowing
        .Subscribe(show => {
          if (show) {
            this.view.SetState((int)StateRole.Shown);
            this.currentState.Value = ViewState.SportSelect;
          }
          else {
            this.view.SetState((int)StateRole.Hidden);
            this.currentState.Value = ViewState.None;
          }})
        .AddTo(this.disposables);
    }

    void InitScreens()
    {
      this.sportScreen = new MatchPrepareViewSportScreen(
        selectedSport: this.selectedSport,
        sportsGrid: this.view.GetItem<ContainerReference>(
        (int)ContainerRole.SportGrid).Container);

      this.athleteListScreen = new MatchPrepareViewAthleteListScreen(
        parentState: this.currentState,
        view: this.view.GetItem<InnerComponentReference>(
          (int)InnerComponentRole.AthleteListScreen).InnerComponent,
        contenderContainer: this.view.GetItem<ContainerReference>(
          (int)ContainerRole.AthleteContainer).Container);

      this.athleteSelectionScreen = new MatchPrepareViewAthleteSelectionScreen(
        parentState: this.currentState,
        selectedSport: this.selectedSport,
        view: this.view.GetItem<InnerComponentReference>(
          (int)InnerComponentRole.AthleteSelectionScreen).InnerComponent
        );

    }

    void SubscribeSportSelection()
    {
      this.selectedSport
        .Subscribe(selected => {
            if (selected != null) {
              this.currentState.Value = ViewState.AthleteList;
            } 
            else if (this.currentState.Value != ViewState.None) {
              this.currentState.Value = ViewState.SportSelect;
            }
          })
        .AddTo(this.disposables);
    }

    void SubscribeMatch()
    {
      this.matchController.CurrentMatch
        .Subscribe(match => {
            if (match == null) {
              this.matchSubscription?.Dispose();
              this.matchSubscription = null;
              this.IsShowing.Value = false;
              this.athleteSelectionScreen.OnMatchEnded();
              return;
            }
            this.IsShowing.Value = true;
            this.matchSubscription = new ();

            match.UserAthletes
            .ObserveAdd()
            .Select(_ => UniRx.Unit.Default)
            .Merge(match.UserAthletes.ObserveRemove()
              .Select(_ => UniRx.Unit.Default))
            .Subscribe(_ => this.OnUserAthleteChanged(match))
            .AddTo(this.matchSubscription);

            match.CurrentState
              .Subscribe(state => {
                if (state == Match.State.NotStartable) {
                  this.view.SetState((int)StateRole.UnAvailable);
                }
                else if (state == Match.State.BeforeStart){
                  this.view.SetState((int)StateRole.Available);
                }
                else {
                  this.IsShowing.Value = false;

                }}) 
              .AddTo(this.matchSubscription);
          })
        .AddTo(this.disposables);
    }

    void OnUserAthleteChanged(Match match)
    {
      switch (this.currentState.Value) {
        case ViewState.SportSelect:
          this.sportScreen.FillSportsGrid(match);
          break;
        case ViewState.AthleteList:
          var sportType = this.selectedSport.Value;
          if (sportType == null) {
          #if UNITY_EDITOR
            throw (new ApplicationException($"{nameof(OnUserAthleteChanged)}: {nameof(this.selectedSport)} is null"));
          #else
            return;
          #endif
          }
          this.athleteListScreen.UpdateHeader(match, sportType.Value);
          break;
      }

    }

    void OnViewStateChanged()
    {
      var match = this.matchController.CurrentMatch.Value;
      if (this.currentState.Value != ViewState.None && 
        match == null) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(OnViewStateChanged)}: {nameof(this.matchController.CurrentMatch)} is null"));
        #else
        return ;
        #endif
      }
      switch (this.currentState.Value) {
        case ViewState.SportSelect:
          this.view.SetRawTextByRole(
            (int)TextRole.MatchTitle, match.Data.Name);
          this.selectedSport.Value = null;
          this.sportScreen.FillSportsGrid(match);
          break;
        case ViewState.AthleteList:
          var sportType = this.selectedSport.Value;
          if (sportType == null) {
          #if UNITY_EDITOR
            throw (new ApplicationException($"{nameof(OnViewStateChanged)}: {nameof(this.selectedSport)} is null"));
          #else
            return;
          #endif
          }
          this.athleteListScreen.UpdateHeader(match, sportType.Value);
          this.athleteListScreen.UpdateList(match, sportType.Value);
          break;
        case ViewState.AthleteSelect:
          if (this.selectedSport.Value == null) {
          #if UNITY_EDITOR
            throw (new ApplicationException($"{nameof(OnViewStateChanged)}: {nameof(this.selectedSport)} is null"));
          #else
            return;
          #endif
          }
          var recruitedAthletes = this.domAthService.GetRecruitedAthleteList();
          
          this.athleteSelectionScreen.UpdateList(
            match: match,
            sportType: this.selectedSport.Value.Value,
            athletes: recruitedAthletes.Count > 0 ? recruitedAthletes: 
            this.athleteController.Athletes.Take(5).ToList(),
            registeredAthletes: match.UserAthletes);
          break;
      } 
    }
  }
}
