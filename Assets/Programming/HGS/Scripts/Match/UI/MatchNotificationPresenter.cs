using System;
using System.Collections.Generic;
using UnityEngine;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UniRx.Triggers;
using Zenject;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent))]
  public class MatchNotificationPresenter : MonoBehaviour
  {
    const float QUOTES_DELAY_IN_SECOND = 1.0f;
    public enum NotificationType
    {
      None,
      Register,
      Opening
    }

    public ReactiveProperty<NotificationType> CurrentNotification { get; private set; }
    
    [Inject]
    ITimeFlowController timeFlowController;
    [Inject]
    IMatchController matchController;

    StatefulComponent view;
    CompositeDisposable disposables;
    Transform container;

    void Awake()
    {
      this.disposables = new ();
      this.view = this.GetComponent<StatefulComponent>();
      this.container = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.Container).Object.transform;
      this.CurrentNotification = new (NotificationType.None);
    }

    // Start is called before the first frame update
    void Start()
    {
      this.view.SetState((int)StateRole.Hidden);
      this.timeFlowController.WeekInYear
        .Select(_ => this.matchController.NextMatch.Value)
        .Merge(this.matchController.NextMatch)
        .Subscribe(this.UpdateNotificationType)
        .AddTo(this.disposables);
      this.CurrentNotification
        .Subscribe(this.UpdateNotification)
        .AddTo(this.disposables);
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.EnterMatchButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.OnClickEnterMatch())
        .AddTo(this.disposables);

      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposables?.Dispose());
    
    }

    void OnClickEnterMatch()
    {

    }

    void UpdateNotificationType(Nullable<MatchData> nextMatch)
    {
      if (nextMatch == null) {
        this.CurrentNotification.Value = NotificationType.None;
        return;
      }
      var matchDate = nextMatch.Value.DateOfEvent;
      var currentDate = new GameDate { 
        Year = this.timeFlowController.YearPassedAfterStart,
        Week = this.timeFlowController.WeekInYear.Value 
      }; 
      int weeksLeft = matchDate - currentDate;
      if (weeksLeft == 0 && 
        this.matchController.IsRegistered(nextMatch.Value)) {
        this.CurrentNotification.Value = NotificationType.Opening;
      }
      else if (weeksLeft == IMatchController.REGISTER_MATCH_DEAD_LINE_WEEKS) {
        this.CurrentNotification.Value = NotificationType.Register;
      }
      else {
        this.CurrentNotification.Value = NotificationType.None;
      }
    }

    void UpdateNotification(NotificationType notificationType)
    {
      if (notificationType == NotificationType.None) {
        this.view.SetState((int)StateRole.Hidden);
        // 다음 애니메이션을 위해 위치 조정
        this.container.DOLocalMoveY(
          endValue: -800f,
          duration: 0.5f)
          .Complete();
      }
      else {
        if (notificationType == NotificationType.Register) {
          this.FillRegisteredContent();
        }
        else if (notificationType == NotificationType.Opening) {
          this.FillOpeningContent();
        }
        this.view.SetState((int)StateRole.Shown);
        this.container.DOLocalMoveY(
          endValue: -400f,
          duration: 0.5f)
          .SetEase(Ease.InOutSine);
      }
    }

    void FillRegisteredContent()
    {
      var nextMatch = this.matchController.NextMatch.Value;
      if (nextMatch == null) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(FillRegisteredContent)}: {nameof(this.matchController.NextMatch)} is null"));
        #else
        return;
        #endif
      }
      var match = nextMatch.Value;
      // TODO: Character name, image
      this.view.SetRawTextByRole(
        (int)TextRole.CharacterNameLabel,
        "캐릭터 이름");
      var matchName = match.Name;
      var now = new GameDate { 
        Year = this.timeFlowController.YearPassedAfterStart,
             Week = this.timeFlowController.WeekInYear.Value };
      var weeksLeft = match.DateOfEvent - now;
      string content;
      if (!this.matchController.IsRegistered(nextMatch.Value)) {
        content = $"{weeksLeft}주 뒤 {matchName} 대회가 있습니다\n아직 대회 참가 신청을 안하셨네요.\n 대회에 참가할까요?";
      }
      else {
        content = $"{weeksLeft}주 뒤 {matchName} 대회가 있습니다\n대회에서 좋은 성적을 거둘 수 있도록 선수들의 컨디션 관리에 주의해 주세요";
      }
      this.ShowQuotes(content);
    }

    void FillOpeningContent()
    {
      var nextMatch = this.matchController.NextMatch.Value;
      if (nextMatch == null) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(FillRegisteredContent)}: {nameof(this.matchController.NextMatch)} is null"));
        #else
        return;
        #endif
      }
      var match = nextMatch.Value;
      // TODO: Character name, image
      this.view.SetRawTextByRole(
        (int)TextRole.CharacterNameLabel,
        "캐릭터 이름");
      var matchName = match.Name;
      var year = this.timeFlowController.Year.Value % 100;
      string content = $"오늘은 {year}년 {matchName} 대회가 있는 날입니다.\n경기장으로 이동합니다";
      this.ShowQuotes(content);
    }

    async void ShowQuotes(string content)
    {
      string[] lines = content.Split('\n');
      var delay = (int)(QUOTES_DELAY_IN_SECOND * 1000f);
      for (int i = 1; i <= lines.Length; i++) {
        this.view.SetRawTextByRole(
          (int)TextRole.CharacterQuotesLabel,
          string.Join('\n', lines[0..i]));
        await UniTask.Delay(delay);
      }
    }
  }
}
