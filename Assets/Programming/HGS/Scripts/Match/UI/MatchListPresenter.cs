using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using Zenject;
using DG.Tweening;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent), typeof(ContainerView))]
  public class MatchListPresenter : MonoBehaviour
  {
    [Inject]
    IMatchController matchController;
    [Inject]
    ITimeFlowController timeFlowController;
    StatefulComponent view;
    ContainerView containerView;
    Queue<MatchData> scheduledMatches;
    ScrollRect scrollView;
    Transform container;
    CompositeDisposable disposables;
    HashSet<Button> subscribedButtons;
    public ReactiveProperty<bool> IsShowing { get; private set; }

    public void Show()
    {
      this.IsShowing.Value = true;
    }

    void Awake()
    {
      this.view = this.GetComponent<StatefulComponent>(); 
      this.containerView = this.GetComponent<ContainerView>();
      this.container = this.transform.Find("Container");
      this.scrollView = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.ScrollView).Object.GetComponent<ScrollRect>();
      this.scheduledMatches = new ();
      this.subscribedButtons = new ();
      this.disposables = new ();
    }

    // Start is called before the first frame update
    void Start()
    {
      this.view.SetState((int)StateRole.Hidden);
      this.IsShowing = new (false);
      this.IsShowing
        .Subscribe(show => {
          if (show) {
            this.view.SetState((int)StateRole.Shown);
            this.container.transform     
            .DOLocalMoveY(
              endValue: -300f,
              duration: 0.5f)
            .SetEase(Ease.OutSine);
          }
          else {
            this.container.transform
            .DOLocalMoveY(
              endValue: -600f,
              duration:0.5f)
            .SetEase(Ease.InSine)
            .OnComplete(() => this.view.SetState((int)StateRole.Hidden));
          }
        })
        .AddTo(this.disposables);

      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.CloseButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.IsShowing.Value = false)
        .AddTo(this.disposables);

      this.SubscribeTimeFlow();
      this.SubscribeMatch();
    }

    void SubscribeTimeFlow()
    {
      this.timeFlowController.Year
        .Subscribe(_ => {
          this.scheduledMatches.Clear();
          int thisYear = this.timeFlowController.YearPassedAfterStart;
          foreach (var match in this.matchController.ScheduledMatches) {
            if (match.DateOfEvent.Year > thisYear) {
              break;
            }
            if (match.DateOfEvent.Year == thisYear) {
              this.scheduledMatches.Enqueue(match);
            }
          }
          this.UpdateContainer(); 
          })
        .AddTo(this.disposables);

      this.timeFlowController.WeekInYear
        .Subscribe(week => {
          var now = new GameDate { 
            Year = this.timeFlowController.YearPassedAfterStart,
            Week = week };
          while (this.scheduledMatches.Count > 0 &&
            this.scheduledMatches.Peek().DateOfEvent < now) {
            this.scheduledMatches.Dequeue();
          }
          this.UpdateContainer();
          })
        .AddTo(this.disposables);
    }

    void SubscribeMatch()
    {
      this.matchController.RegisteredMatches
        .ObserveAdd()
        .Select(_ => UniRx.Unit.Default)
        .Merge(this.matchController.RegisteredMatches
          .ObserveRemove()
          .Select(_ => UniRx.Unit.Default))
        .Subscribe(_ => this.UpdateContainer());
    }

    void UpdateContainer()
    {
      this.containerView.Clear();
      this.containerView.FillWithItems(
        this.scheduledMatches,
        (view, match) => this.FillMatchRow(view, match));
      this.scrollView.verticalNormalizedPosition = 1f;
    }

    void FillMatchRow(StatefulComponent view, MatchData match)
    {
      if (match.IsMandatory) {
        view.SetState((int)StateRole.MandatoryMatch);
      }
      else {
        if (this.matchController.IsRegistered(match)) {
          view.SetState((int)StateRole.Registered);
        }
        else {
          view.SetState((int)StateRole.UnRegistered);
        }
        var button = view.GetItem<ButtonReference>(
          (int)ButtonRole.RegisterButton).Button;
        if (!this.subscribedButtons.Contains(button)) {
          button
            .OnClickAsObservable()
            .Subscribe(_ => this.OnClickRegisterButtonFor(match))
            .AddTo(this.disposables);
          this.subscribedButtons.Add(button);
        }
      }
      view.SetRawTextByRole((int)TextRole.Title, match.Name);
      view.SetRawTextByRole(
        (int)TextRole.Description,
        this.GetMatchDateText(match.DateOfEvent));
      view.SetRawTextByRole(
        (int)TextRole.RegisterButtonLabel,
        this.GetRegisterButtonText(match));
    }

    void OnClickRegisterButtonFor(in MatchData match)
    {
      if (this.matchController.IsRegistered(match)) {
        this.matchController.UnRegister(match);
      }
      else {
        this.matchController.Register(match);
      }
    }

    string GetMatchDateText(in GameDate date)
    {
      var season = (Season)((date.Week - 1) / ITimeFlowController.WEEK_FOR_SEASON);
      string seasonText = GameDate.GetKoreanNameOf(season);
      int year = date.Year % 100;
      var now = new GameDate { 
        Year = this.timeFlowController.YearPassedAfterStart,
        Week = this.timeFlowController.WeekInYear.Value };
      var weeksLeft = date - now;
      return ($"날짜: {year}년 {date.Week}~{date.Week + 1}주차 ({weeksLeft}주 뒤)");
    }

    string GetRegisterButtonText(in MatchData match)
    {
      if (match.IsMandatory) {
        return ("강제 참여");
      }
      if (this.matchController.IsRegistered(match)) {
        return ("참가취소");
      }
      else {
        return ("참가하기");
      }
    }
  }
}
