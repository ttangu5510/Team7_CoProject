using System;

namespace SHG
{
  using Season = ITimeFlowController.Season;

  [Serializable]
  public struct IncomeForSeason
  {
    public int[] Incomes;

    public int GetIncomeFor(Season season)
    {
      return (this.Incomes[(int)season]);
    }
  }

  [Serializable]
  public struct PersonnelMatainanceCost
  {
    public int GeneralAthlete;
    public int NationalAthleteCandidate;
    public int NationalAthlete;
    public int Coach;

    public int GetTotal(int generalAthlete, int nationalAthleteCandidate, int nationalAthlete, int coach)
    {
      return (
        (this.GeneralAthlete * generalAthlete) +
        (this.NationalAthleteCandidate * nationalAthleteCandidate) +
        (this.NationalAthlete * nationalAthlete) +
        (this.Coach * coach)
        );
    }
  }

  [Serializable]
  public struct FacilityMaintainanceCost
  {
    public int[] CostByStage; 

    public int GetCostFor(int stage)
    {
      return (this.CostByStage[stage]);
    }
  }

  [Serializable]
  public struct ResourceData
  {
    public IncomeForSeason[] TrainingGrantByYears;
    public IncomeForSeason[] CompetitionGrantByYears;
    public IncomeForSeason[] QuestPrizes;
    public PersonnelMatainanceCost PersonnelCost;
    public FacilityMaintainanceCost FacilityCost;
  }
}
