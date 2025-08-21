
namespace SHG
{
  public static class ResourceDummyData 
  {
    public static IncomeForSeason TrainingGrants1
      = new IncomeForSeason { Incomes = new int[] {
        15_000, 15_000, 15_000, 15_000 
      }};

    public static IncomeForSeason TrainingGrants2
      = new IncomeForSeason { Incomes = new int[] {
        18_000, 18_000, 18_000, 25_000 
      }};

    public static IncomeForSeason TrainingGrants3
      = new IncomeForSeason { Incomes = new int[] {
        20_000, 20_000, 20_000, 29_000 
      }};

    public static IncomeForSeason TrainingGrants4
      = new IncomeForSeason { Incomes = new int[] {
        24_000, 24_000, 24_000, 33_000 
      }};

    public static IncomeForSeason CompetitionGrants1
      = new IncomeForSeason { Incomes = new int[] {
        5_000, 5_000, 5_000, 10_000 
      }};

    public static IncomeForSeason CompetitionGrants2
      = new IncomeForSeason { Incomes = new int[] {
        5_000, 5_000, 5_000, 11_000 
      }};

    public static IncomeForSeason CompetitionGrants3
      = new IncomeForSeason { Incomes = new int[] {
        5_000, 5_000, 5_000, 11_000 
      }};

    public static IncomeForSeason CompetitionGrants4
      = new IncomeForSeason { Incomes = new int[] {
        5_000, 5_000, 5_000, 15_000 
      }};
    
    public static IncomeForSeason QuestPrize
      = new IncomeForSeason { Incomes = new int[] {
        3_000, 3_000, 3_000, 6_000
      }};

    public static PersonnelMatainanceCost PersonnelCost 
      = new PersonnelMatainanceCost {
        GeneralAthlete = 500,
        NationalAthleteCandidate = 1_000,
        NationalAthlete = 2_000,
        Coach = 1_000
      };

    public static FacilityMaintainanceCost FacilityCost
      = new FacilityMaintainanceCost {CostByStage = new int[] {
        0, 2_000, 3_000, 5_000, 5_000
      }};

    public static ResourceData Data => new ResourceData {
      TrainingGrantByYears = new IncomeForSeason[] {
        TrainingGrants1, TrainingGrants2, TrainingGrants3, TrainingGrants4 },
      CompetitionGrantByYears = new IncomeForSeason[] {
        CompetitionGrants1, CompetitionGrants2, CompetitionGrants3, CompetitionGrants4
      },
      QuestPrizes = new IncomeForSeason[] {
        QuestPrize, QuestPrize, QuestPrize, QuestPrize 
      },
      PersonnelCost = PersonnelCost,
      FacilityCost = FacilityCost
    };
  }
}
