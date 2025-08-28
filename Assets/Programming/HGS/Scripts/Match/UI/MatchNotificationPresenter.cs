using System;
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
    const string MATCH_PARTICIPATE_FAILED_NOTICE = "현재 대회 참가를 위한 선수 인원 조건을 만족하지 못했습니다. 대회 참가 신청을 했음에도 참여 불가능한 상태일 경우 대회에서 몰수패를 당합니다. 다음에는 선수단 관리에 주의해 주세요";
    const string MATCH_PARTICIPATE_NOTICE = "이 대회에 참가하시겠습니까?\n대회 참가시 2주가 소요되며 선수들의 피로도가 증가합니다.";

    public enum NotificationType
    {
      None,
      Register,
      Opening
    }
    const float QUOTES_DELAY_IN_SECOND = 1.0f;

    public ReactiveProperty<NotificationType> CurrentNotification { get; private set; }
    
    [Inject]
    ITimeFlowController timeFlowController;
    [Inject]
    IMatchController matchController;

    StatefulComponent view;
    CompositeDisposable disposables;
    Transform container;
    StatefulComponent popup;

    void Awake()
    {
      this.disposables = new ();
      this.view = this.GetComponent<StatefulComponent>();
      this.container = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.Container).Object.transform;
      this.CurrentNotification = new (NotificationType.None);
      this.popup = this.view.GetItem<InnerComponentReference>(
        (int)InnerComponentRole.Popup).InnerComponent;
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
        (int)ButtonRole.QuoteButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.OnClickQuote())
        .AddTo(this.disposables);

      this.popup.GetItem<ButtonReference>(
        (int)ButtonRole.CloseButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => {
            this.view.SetState((int)StateRole.PopupHidden);
            this.view.SetState((int)StateRole.Hidden);
          })
        .AddTo(this.disposables);

      this.popup.GetItem<ButtonReference>(
        (int)ButtonRole.ConfirmButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.OnClickConfirm())
        .AddTo(this.disposables);

      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposables?.Dispose());
    }

    void OnClickConfirm()
    {
      var match = this.matchController.NextMatch.Value.Value;
      switch (this.CurrentNotification.Value) {
        case (NotificationType.Register):
          if (this.matchController.NextMatch.Value == null) {
          #if UNITY_EDITOR
            throw (new ApplicationException(
                $"{nameof(OnClickConfirm)}: {nameof(this.matchController.NextMatch)} is null"));
          #else
            return;
          #endif
          }
          this.matchController.Register(match); 
          break;
        case (NotificationType.Opening):
          if (this.matchController.IsParticipatable(match)) {
            this.matchController.EnterNextMatch();
          }
          break;
      }
      this.view.SetState((int)StateRole.PopupHidden);
      this.view.SetState((int)StateRole.Hidden);
    }

    void OnClickQuote()
    {
      switch (this.CurrentNotification.Value) {
        case NotificationType.None:
          #if UNITY_EDITOR
          throw (new ApplicationException(nameof(OnClickQuote)));
          #else
          return;
          #endif
        case NotificationType.Register:
          var match = this.matchController.NextMatch.Value.Value;
          if (!match.IsMandatory && 
            !this.matchController.IsRegistered(match)) {
            this.FillRegisterPopupContent(match);
            this.view.SetState((int)StateRole.PopupShown);
          }
          else {
            this.view.SetState((int)StateRole.Hidden);
          }
          break;
        case NotificationType.Opening:
          this.FillOpeningPopupContent();
          this.view.SetState((int)StateRole.PopupShown);
          break;
      }
    }

    void FillRegisterPopupContent(in MatchData match)
    {
      this.popup.SetState((int)StateRole.Register);
      int year = this.timeFlowController.Year.Value % 100;
      this.popup.SetRawTextByRole(
        (int)TextRole.Title,
        "대회 참가");
      this.popup.SetRawTextByRole(
        (int)TextRole.MatchTitle,
        $"{year}년 {match.Name}");
      this.popup.SetRawTextByRole(
        (int)TextRole.Description,
        MATCH_PARTICIPATE_NOTICE);
    }

    void FillOpeningPopupContent()
    {
      if (this.matchController.NextMatch.Value == null) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(FillOpeningPopupContent)}: {nameof(this.matchController.NextMatch)} is null"));
      #else
        return;
      #endif
      }
      var match = this.matchController.NextMatch.Value.Value;
      this.popup.SetState((int)StateRole.Opening);
      if (this.matchController.IsParticipatable(match)) {
        this.popup.SetRawTextByRole(
          (int)TextRole.Title, "대회 입장 알림");   
        int year = this.timeFlowController.Year.Value % 100;
        this.popup.SetRawTextByRole(
          (int)TextRole.Description,
          $"{year}년 {match.Name} 대회가 오늘 개막했습니다\n대회장으로 이동합니다");
      }
      else {
        this.popup.SetRawTextByRole(
          (int)TextRole.Title, "대회 참가 불가");
        this.popup.SetRawTextByRole(
          (int)TextRole.Description,
          MATCH_PARTICIPATE_FAILED_NOTICE);
      }
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
        (nextMatch.Value.IsMandatory ||
        this.matchController.IsRegistered(nextMatch.Value))) {
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
      this.view.SetState((int)StateRole.PopupHidden);
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
      if (!match.IsMandatory && 
        !this.matchController.IsRegistered(nextMatch.Value)) {
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
