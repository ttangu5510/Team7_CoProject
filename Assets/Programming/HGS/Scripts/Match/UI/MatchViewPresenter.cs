using UnityEngine;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using Zenject;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent))]
  public class MatchViewPresenter : MonoBehaviour
  {
    public enum ViewState
    {
      None,
      Record = StateRole.Record,
      Rank = StateRole.Rank,
      Result = StateRole.Result,
      Reward = StateRole.Reward
    }

    [Inject]
    IMatchController matchController;
    [Inject]
    IResourceController resourceController;

    MatchViewRecordScreen recordScreen;
    MatchViewRankScreen rankScreen;
    MatchViewResultScreen resultScreen;

    StatefulComponent view;
    ReactiveProperty<ViewState> currentState;
    CompositeDisposable matchSubscription;

    void Awake()
    {
      this.currentState = new (ViewState.None);
      this.view = this.GetComponent<StatefulComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
      this.currentState
        .Subscribe(this.OnViewStateChanged)
        .AddTo(this);
      this.InitScreens();
      this.matchController.CurrentMatch.Subscribe(this.OnMatchChanged);
    }

    void InitScreens()
    {
      this.recordScreen = new MatchViewRecordScreen(
        parentState: this.currentState,
        view: this.view.GetItem<InnerComponentReference>(
          (int)InnerComponentRole.RecordScreen).InnerComponent);

      this.rankScreen = new MatchViewRankScreen(
        parentState: this.currentState, 
        view: this.view.GetItem<InnerComponentReference>(
          (int)InnerComponentRole.RankScreen).InnerComponent);

      this.resultScreen = new MatchViewResultScreen(
        parentState: this.currentState,
        view: this.view.GetItem<InnerComponentReference>(
          (int)InnerComponentRole.ResultScreen).InnerComponent);
    }

    void OnViewStateChanged(ViewState state)
    {
      if (state != ViewState.None) {
        this.view.SetState((int)state);
        this.view.SetState((int)StateRole.Shown);
      }
      else {
        this.view.SetState((int)StateRole.Hidden);
      }
    }

    void OnMatchChanged(Match match)
    {
      if (match == null) {
        this.matchSubscription?.Dispose();
        this.matchSubscription = null;
        this.currentState.Value = ViewState.None;
        return ;
      }
      this.matchSubscription = new ();

      match.CurrentState.Subscribe(
        state => this.OnMatchStateChanged(state, match))
      .AddTo(this.matchSubscription);

      match.CurrentSport
        .Subscribe(currentSport => {
            if (currentSport == null) {
              return ;
            }
            this.recordScreen.OnSportChanged(currentSport.Value, match);
          })
        .AddTo(this.matchSubscription);

      match.SportRecords
        .ObserveReplace()
        .Subscribe(replacedEvent => 
          this.recordScreen.UpdateScoreBoard(replacedEvent.NewValue))
        .AddTo(this.matchSubscription);

      match.SportRecords
        .ObserveAdd()
        .Subscribe(addedEvent => this.recordScreen.UpdateScoreBoard(addedEvent.Value))
        .AddTo(this.matchSubscription);
    }

    void OnMatchStateChanged(Match.State state, Match match)
    {
      switch (state) {
        case Match.State.BeforeStart:
        case Match.State.NotStartable:
          this.currentState.Value = ViewState.None;
          return ;
        case Match.State.InSport:
          this.recordScreen.UpdateHeader(match);
          this.currentState.Value = ViewState.Record;
          break;
        case Match.State.BeforeSport:
          match.StartCurrentSport();
          break;
        case Match.State.AfterSport:
          if (match.CurrentSport.Value != null) {
            this.currentState.Value = ViewState.Rank;
            this.rankScreen.UpdateView(match);
          }
          break; 
        case Match.State.Ended:
          this.currentState.Value = ViewState.Result;
          this.resultScreen.UpdateView(match);
          this.matchController.EndCurrentMatch();
          this.resourceController.AddMoney(
            1000, IncomeType.CompetitionGrant);
          this.resourceController.AddFame(300);
          break;
      }
    }
  }
}
