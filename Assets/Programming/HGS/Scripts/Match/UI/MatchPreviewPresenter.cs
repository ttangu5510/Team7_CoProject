using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using StatefulUI.Runtime.Core;
using Zenject;
using UniRx;
using UniRx.Triggers;
using EditorAttributes;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent), typeof(ContainerView))]
  public class MatchPreviewPresenter : MonoBehaviour
  {
    [Inject]
    IMatchController matchController;
    [Inject]
    ITimeFlowController timeFlowController;

    StatefulComponent view;
    ContainerView containerView;
    CompositeDisposable disposables;
    Queue<GameDate> dateLeftThisYear;
    
    void Awake()
    {
      this.disposables = new ();
      this.view = this.GetComponent<StatefulComponent>();
      this.containerView = this.GetComponent<ContainerView>();
      this.dateLeftThisYear = new ();

      int thisYear = this.timeFlowController.YearPassedAfterStart;
      foreach (var date in this.timeFlowController.DateToEnd) {
        if (date.Year > thisYear) {
          break;
        }
        this.dateLeftThisYear.Enqueue(date); 
      }
    }

    // Start is called before the first frame update
    void Start()
    {
      this.UpdateContainer();
      this.matchController.ScheduledMatches
        .ObserveCountChanged()
        .Subscribe(_ => this.UpdateContainer())
        .AddTo(this.disposables);

      this.timeFlowController.DateToEnd
        .ObserveRemove()
        .Subscribe(removed => {
            if (this.dateLeftThisYear.Count == 0) {
              return; 
            }
            while (removed.Value >= this.dateLeftThisYear.Peek()) {
              this.dateLeftThisYear.Dequeue(); 
            }
            this.UpdateContainer();
          })
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
      var season = (Season)((date.Week - 1) / TimeFlowController.WEEK_FOR_SEASON);
      if (weekInSeason > 1) {
        view.SetRawTextByRole(
          (int)TextRole.WeekLabel,
          $"{weekInSeason}");
      }
      else {
        view.SetRawTextByRole(
          (int)TextRole.WeekLabel,
          $"{this.GetKoreanNameOf(season)} {weekInSeason}");
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
      }
      else {
        view.SetState((int)StateRole.MatchHidden);
      }
    }

    string GetKoreanNameOf(Season season) {
      switch (season) {
        case Season.Spring:
          return ("봄");
        case Season.Summer:
          return ("여름");
        case Season.Fall:
          return ("가을");
        case Season.Winter:
          return ("겨울");
      }
      return (string.Empty);
    }
  }
}
