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
    }

    public void OnSportChanged(SportType sportType, in Match match)
    {
      this.view.SetRawTextByRole(
        (int)TextRole.SportLabel, 
        MatchData.GetSportTypeString(sportType));

      if (match.SportRecords.TryGetValue(
          sportType, out MatchSportRecord record)) {
        this.UpdateScoreBoard(record);
        match.StartCurrentSport(); 
      }
#if UNITY_EDITOR
      else {
        throw (new ApplicationException($"{nameof(OnSportChanged)}: fail to find {sportType} in {nameof(match.SportRecords)}"));
      }
#endif
    }

    public void UpdateScoreBoard(MatchSportRecord record)
    {
      this.rankingContainer.Clear(); 
      // TODO: Sort records 
      this.rankingContainer.FillWithItems(
        record.RecordsByAthletes,
        (view, recordWithAthlete) => {

          int rank = recordWithAthlete.record.Rank;
          view.SetRawTextByRole(
            (int)TextRole.RankLabel, rank > 0 ?
            $"{recordWithAthlete.record.Rank}위": string.Empty);

          view.SetRawTextByRole(
            (int)TextRole.NationalityLabel, 
            recordWithAthlete.athlete.Country.Name);

          view.SetRawTextByRole(
            (int)TextRole.AthleteNameLabel,
            recordWithAthlete.athlete.Name);

          string recordText = record.CurrentStage > 1 ? 
            string.Format("{0:N}", record.GetNormalizedRecordValueOf(
                recordWithAthlete.record)): string.Empty;
          view.SetRawTextByRole(
            (int)TextRole.RecordLabel, recordText);
        });
    }
  }
}
