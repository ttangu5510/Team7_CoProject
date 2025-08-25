using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using Zenject;
using DG.Tweening;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent), typeof(ContainerView))]
  public class MatchListPresenter : MonoBehaviour
  {
    const string MATCH_REGISTERED_NOTICE = "대회에 참가했습니다.\n대회 참가시 2주가 소요되며 선수들의 피로도가 증가합니다.";
    const string MATCH_UNREGISTERED_NOTICE = "대회에 참가를 취소했습니다.\n 대회 시작 1주 전까지 다시 참가 신청을 할 수 있습니다";

    public ReactiveProperty<bool> IsShowing { get; private set; }

    [Inject]
    IMatchController matchController;
    [Inject]
    ITimeFlowController timeFlowController;

    StatefulComponent view;
    StatefulComponent popup;
    ContainerView containerView;
    Queue<MatchData> scheduledMatches;
    ScrollRect scrollView;
    Transform container;
    CompositeDisposable disposables;
    HashSet<Button> subscribedButtons;

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
          this.view.SetState((int)StateRole.PopupHidden);
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
            .OnComplete(() => {
              this.view.SetState((int)StateRole.Hidden);
              });
          }
        })
        .AddTo(this.disposables);

      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.CloseButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.IsShowing.Value = false)
        .AddTo(this.disposables);

      this.popup = this.view.GetItem<InnerComponentReference>(
        (int)InnerComponentRole.Popup).InnerComponent;
      popup.GetItem<ButtonReference>(
        (int)ButtonRole.ConfirmButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.view.SetState((int)StateRole.PopupHidden))
        .AddTo(this.disposables);

      this.SubscribeTimeFlow();
      this.SubscribeMatch();
      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposables?.Dispose());
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
        this.FillPopupContent(false);
      }
      else {
        this.matchController.Register(match);
        this.FillPopupContent(true);
      }
      this.view.SetState((int)StateRole.PopupShown);
    }

    void FillPopupContent(bool isRegistered)
    {
      if (this.matchController.NextMatch.Value == null) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(FillPopupContent)}: {nameof(this.matchController.NextMatch)} is null"));
      #else
        return;
      #endif
      }
      var match = this.matchController.NextMatch.Value.Value;
      this.popup.SetRawTextByRole(
        (int)TextRole.Title,
        isRegistered ? "참가 취소":"대회 참가"); 
      this.popup.SetState(
        (int)(isRegistered ? StateRole.Registered: StateRole.UnRegistered));
      int year = this.timeFlowController.Year.Value % 100;

      this.popup.SetRawTextByRole(
        (int)TextRole.MatchTitle,
        $"{year}년 {match.Name} 대회");
      this.popup.SetRawTextByRole(
        (int)TextRole.Description,
        isRegistered ? MATCH_REGISTERED_NOTICE: MATCH_UNREGISTERED_NOTICE);
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
