using System;
using System.Collections.Generic;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine;
namespace SHG
{
  public class MatchViewRecordScreen 
  {
    
    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    ContainerView rankingContainer;
    Dictionary<IContender, int> previousPositions;
    VerticalLayoutGroup rankingContainerLayout;

    public MatchViewRecordScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;
      this.previousPositions = new ();
      this.rankingContainer = this.view.GetItem<ContainerReference>(
        (int)ContainerRole.RankingContainer).Container;
      this.rankingContainerLayout = this.rankingContainer.Root.GetComponent<VerticalLayoutGroup>();
    }

    public void UpdateHeader(in Match match)
    {
      this.view.SetRawTextByRole(
        (int)TextRole.MatchTitle, match.Data.Name);
      bool isDomestic = match.Data.IsDomestic;

      if (isDomestic) {
        this.view.SetState((int)StateRole.Domestic);
      }
      else {
        this.view.SetState((int)StateRole.International);
      }
    }

    public void OnSportChanged(SportType sportType, in Match match)
    {
      this.previousPositions.Clear();
      this.view.SetRawTextByRole(
        (int)TextRole.SportLabel, 
        MatchData.GetSportTypeString(sportType));

      if (match.SportRecords.TryGetValue(
          sportType, out MatchSportRecord record)) {
        this.UpdateScoreBoard(record, match);
      }
#if UNITY_EDITOR
      else {
        throw (new ApplicationException($"{nameof(OnSportChanged)}: fail to find {sportType} in {nameof(match.SportRecords)}"));
      }
#endif
    }

    public void UpdateScoreBoard(MatchSportRecord record, Match match)
    {
      this.rankingContainer.Clear(); 
      this.rankingContainerLayout.enabled = true;
      this.rankingContainer.FillWithItems(
        record.RecordsByAthletes,
        (view, recordWithAthlete) => {
          int index = this.GetIndexOf(recordWithAthlete.athlete, record);
          int rank = record.Type == 
          MatchSportRecord.ProgressType.AllAtOnce ? recordWithAthlete.record.Rank: index;

          if (match.Data.IsDomestic) {
            view.SetState((int)StateRole.Domestic);
          }
          else {
            view.SetState((int)StateRole.International);
          }
          view.SetRawTextByRole(
            (int)TextRole.RankLabel, rank > 0 ?
            $"{rank}위": string.Empty);

          view.SetRawTextByRole(
            (int)TextRole.GroupLabel, 
            recordWithAthlete.athlete.Group.Name);

          view.SetRawTextByRole(
            (int)TextRole.AthleteNameLabel,
            recordWithAthlete.athlete.Name);

          string recordText = record.CurrentStage > 1 ? 
            string.Format("{0:N}", 
              recordWithAthlete.record.NormalizedValue): string.Empty;
          view.SetRawTextByRole(
            (int)TextRole.RecordLabel, recordText);

          if (record.Type == MatchSportRecord.ProgressType.AllAtOnce) {
            if (this.previousPositions.TryGetValue(
                recordWithAthlete.athlete, out int previousIndex)) {
              int indexOffset = index - previousIndex;
              if (indexOffset != 0) {
                float posY = view.transform.localPosition.y;
                this.MoveToPreviousPosition(
                  transform: view.transform as RectTransform,
                  currentIndex: index,
                  indexOffset: indexOffset);
                this.AnimateScoreCell(
                  transform: view.transform as RectTransform,
                  localPosY: posY);
                this.previousPositions[recordWithAthlete.athlete] = index;
              }
            }
            else {
              this.previousPositions.Add(recordWithAthlete.athlete, index);
            }
          }
        });
      if (record.Type == MatchSportRecord.ProgressType.AllAtOnce) {
        this.DisableLayout();
      }
    }

    async void DisableLayout()
    {
      await UniTask.Yield();
      this.rankingContainerLayout.enabled = false;
    }

    async void MoveToPreviousPosition(RectTransform transform, int currentIndex, int indexOffset)
    {
      await UniTask.WaitUntil(this.IsLayoutDiabled);
      float startY = transform.localPosition.y;
      float offsetForIndex = startY / (float)(currentIndex + 1);
      float offsetY =  indexOffset * offsetForIndex;
      transform.localPosition = new Vector3(
        transform.localPosition.x,
        startY - offsetY,
        transform.localPosition.z
        );
    }

    bool IsLayoutDiabled()
    {
      return (!this.rankingContainerLayout.enabled);
    }

    async void AnimateScoreCell(RectTransform transform, float localPosY)
    {
      await UniTask.WaitUntil(this.IsLayoutDiabled);
      await UniTask.Yield();
      transform.DOLocalMoveY(
        endValue: localPosY,
          duration: 0.5f
          );
    }

    int GetIndexOf(IContender athlete, MatchSportRecord record)
    {
      return (record.RecordsByAthletes.FindIndex(
        recordsByAthlete => recordsByAthlete.athlete == athlete
        ));
    }
  }
}
