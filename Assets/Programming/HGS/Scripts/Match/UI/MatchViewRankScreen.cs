using System;
using System.Collections.Generic;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace SHG
{
  public class MatchViewRankScreen 
  {
    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    Match currentMatch;
    Image[] flagIcons;
    const float FLAG_OFFSET_Y = 600f;
    const float FLAG_ANIMATION_DURATION = 1.5f;
    const float FLAG_ANIMATION_INTERVAL = 0.8f;

    public MatchViewRankScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.NextButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.OnClickNext());
      this.flagIcons = new Image[3];
      this.flagIcons[0] = this.view.GetItem<ImageReference>(
        (int)ImageRole.FirstFlag).Image;
      this.flagIcons[1] = this.view.GetItem<ImageReference>(
        (int)ImageRole.SecondFlag).Image;
      this.flagIcons[2] = this.view.GetItem<ImageReference>(
        (int)ImageRole.ThirdFlag).Image;
    }

    public void UpdateView(in Match match)
    {
      if (match.CurrentSport.Value == null) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(UpdateView)}: {nameof(match.CurrentSport)} is null"));
        #else
        return;
        #endif
      }
      this.currentMatch = match;
      var sportType = match.CurrentSport.Value.Value;
      this.view.SetRawTextByRole(
        (int)TextRole.SportLabel,
        MatchData.GetSportTypeString(sportType)); 
      int rank = this.GetUserRank(match, sportType);
      this.view.SetRawTextByRole(
        (int)TextRole.RankLabel, $"{rank}위");
      var recordsByAthletes = match.SportRecords[sportType].RecordsByAthletes;
      this.UpdateFlags(recordsByAthletes, match);
    }

    async void UpdateFlags(
      List<(IContender athlete, MatchSportRecord.AthleteRecord record)> recordsByAthletes,
      Match match)
    {
      for (int i = 0; i < this.flagIcons.Length; i++) {
        this.PrepareFlagIcon(
          flagIcon: this.flagIcons[i], 
          athlete: recordsByAthletes[i].athlete,
          match: match);
      }
      for (int i = 2; i >= 0; --i) {
        await UniTask.WaitForSeconds(FLAG_ANIMATION_INTERVAL);
        this.AnimateFlagIcon(
          flagIcon: this.flagIcons[i]);
      }
    }

    void PrepareFlagIcon(Image flagIcon, IContender athlete, Match match)
    {
      if (match.Data.IsDomestic ||
      athlete.Group == ConvertedDomesticAthlete.USER_TEAM
      ) {
        flagIcon.sprite = ContendersController.FLAG_ICONS["korea"];
      } else if (ContendersController.FLAG_ICONS.TryGetValue(
            athlete.Group.Name, out Sprite sprite)) {
        flagIcon.sprite = sprite; 
      }
      var rect = flagIcon.transform as RectTransform;
      rect.localPosition = new Vector3(
        rect.localPosition.x,
        rect.localPosition.y + FLAG_OFFSET_Y,
        rect.localPosition.z);
    }

    void AnimateFlagIcon(Image flagIcon) 
    {
      var rect = flagIcon.transform as RectTransform;
      rect.DOLocalMoveY(
        endValue:  rect.localPosition.y - FLAG_OFFSET_Y,
        duration: FLAG_ANIMATION_DURATION)
        .SetEase(Ease.OutSine);
    }

    int GetUserRank(in Match match, SportType sportType)
    {
      var sportRecord = match.SportRecords[sportType]; 
      var athlete = match.UserAthletes[sportType];
      var record = sportRecord.GetRecordOf(athlete);
      return (record.Rank);
    }

    void OnClickNext()
    {
      if (this.currentMatch == null) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(OnClickNext)}: {nameof(this.currentMatch)} is null"));
      #else 
        return;
      #endif
      } 
      this.currentMatch.EndCurrentSport();
    }
  }
}
