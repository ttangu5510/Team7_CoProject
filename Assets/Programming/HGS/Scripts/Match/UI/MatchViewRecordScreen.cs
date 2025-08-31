using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;

namespace SHG
{
  public class MatchViewRecordScreen 
  {
    
    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;
    ContainerView rankingContainer;

    public MatchViewRecordScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;
      this.rankingContainer = this.view.GetItem<ContainerReference>(
        (int)ContainerRole.RankingContainer).Container;
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

      this.rankingContainer.FillWithItems(
        record.RecordsByAthletes,
        (view, recordWithAthlete) => {
      
          int rank = record.Type == 
          MatchSportRecord.ProgressType.AllAtOnce ? recordWithAthlete.record.Rank: this.GetRankOf(recordWithAthlete.athlete, record);

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
        });
    }

    int GetRankOf(IContender athlete, MatchSportRecord record)
    {
      return (record.RecordsByAthletes.FindIndex(
        recordsByAthlete => recordsByAthlete.athlete == athlete
        ) + 1);
    }
  }
}
