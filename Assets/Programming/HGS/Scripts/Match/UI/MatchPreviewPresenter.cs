using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using Zenject;
using UniRx;
using UniRx.Triggers;
using LightScrollSnap;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent), typeof(ContainerView), typeof(ScrollSnap))]
  public class MatchPreviewPresenter : MonoBehaviour
  {
    [Inject]
    IMatchController matchController;
    [Inject]
    ITimeFlowController timeFlowController;

    StatefulComponent view;
    ContainerView containerView;
    ScrollSnap scroll;
    CompositeDisposable disposables;
    Queue<GameDate> dateLeftThisYear;
    MatchListPresenter matchListPresenter;
    
    void Awake()
    {
      this.disposables = new ();
      this.view = this.GetComponent<StatefulComponent>();
      this.matchListPresenter = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.MatchListView).Object.GetComponent<MatchListPresenter>();
      this.containerView = this.GetComponent<ContainerView>();
      this.scroll = this.GetComponent<ScrollSnap>();
      this.dateLeftThisYear = new ();
    }

    // Start is called before the first frame update
    void Start()
    {
      this.timeFlowController.Year
        .Subscribe(year => {
            int thisYear = this.timeFlowController.YearPassedAfterStart;
            foreach (var date in this.timeFlowController.DateToEnd) {
              if (date.Year > thisYear) {
                break;
              }
              this.dateLeftThisYear.Enqueue(date); 
            }
          })
      .AddTo(this.disposables);
      this.UpdateContainer();
      this.matchController.ScheduledMatches
        .ObserveCountChanged()
        .Subscribe(_ => this.UpdateContainer())
        .AddTo(this.disposables);


      this.timeFlowController.DateToEnd
        .ObserveRemove()
        .Subscribe(removed => {
            while (this.dateLeftThisYear.Count > 0 &&
              removed.Value >= this.dateLeftThisYear.Peek()) {
              this.dateLeftThisYear.Dequeue(); 
            }
            this.UpdateContainer();
          })
        .AddTo(this.disposables);

      this.scroll.OnItemClicked
        .AsObservable()
        .Subscribe(_ => this.matchListPresenter.Show())
        .AddTo(this.disposables);

      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposables?.Dispose());
    }

    void UpdateContainer()
    {
      this.containerView.Clear();
      this.containerView.FillWithItems(
        this.dateLeftThisYear,
        (view, date) => {
          this.UpdateDateSection(view, date);
          this.UpdateMatchSection(view, date);
        });
    }

    void UpdateDateSection(StatefulComponent view, in GameDate date)
    {
      int weekInSeason = (date.Week - 1) % 10 + 1;
      var season = (Season)((date.Week - 1) / ITimeFlowController.WEEK_FOR_SEASON);
      if (weekInSeason > 1) {
        view.SetRawTextByRole(
          (int)TextRole.WeekLabel,
          $"{weekInSeason}");
      }
      else {
        view.SetRawTextByRole(
          (int)TextRole.WeekLabel,
          $"{GameDate.GetKoreanNameOf(season)} {weekInSeason}");
      }
      switch (season) {
        case Season.Spring:
          view.SetState((int)StateRole.Spring);
          break;
        case Season.Summer:
          view.SetState((int)StateRole.Summer);
          break;
        case Season.Fall:
          view.SetState((int)StateRole.Fall);
          break;
        case Season.Winter:
          view.SetState((int)StateRole.Winter);
          break;
      }
    }

    void UpdateMatchSection(StatefulComponent view, in GameDate date)
    {
      if (this.matchController.TryGetMatchFor(
          date, out MatchData match)) {
        view.SetRawTextByRole(
          (int)TextRole.MatchLabel,
          match.Name); 
        view.SetState((int)StateRole.MatchShown);
        if (match.IsSingleSport) {
          view.SetState(
            (int)StateRole.SingleSportMatch);
        }
        else if (match.IsMandatory) {
          view.SetState(
            (int)StateRole.MandatoryMatch);
        }
        else {
          view.SetState(
            (int)StateRole.AutonomyMatch);
        }
        view.SetState((int)StateRole.MatchShown);
      }
      else {
        view.SetState((int)StateRole.MatchHidden);
      }
    }
  }
}
