
namespace SHG
{
  public static class FacilityDummyData 
  {
    public static Accomodation.Data Accomodation = 
      new Accomodation.Data {
        NumberOfAthletes = new int[] { 8, 12, 16, 20, 30 },
        MinimumFameForUpgrade = new int[] { 300, 500, 1_000, 3_000 },
        CostForUpgrade = new int[] { 10_000, 30_000, 50_000, 100_000 },
      };

    public static Lounge.Data Lounge =
      new Lounge.Data {
        NumberOfAthletes = new int[] { 2, 4, 6, 8, 8 },
        RecoveryAmounts = new int[] { 30, 50, 55, 60, 70 },
        MinimumFameForUpgrade = new int[] { 0, 0, 0, 2_500 },
        CostForUpgrade = new int[] { 10_000, 30_000, 50_000, 100_000 },
      };

    public static MedicalCenter.Data MedicalCenter = 
      new MedicalCenter.Data {
        MinimumFameForUpgrade = new int[] { 0, 0, 0, 2_500 },
        CostForUpgrade = new int[] { 10_000, 30_000, 50_000, 100_000 },
        NumberOfAthletes = new int[] { 2, 4, 6, 8, 8 },
        RecoveryAmounts = new int [] { 0, 0, 0, 0, 30 }
      };

    public static ScoutCenter.Data ScoutCenter = 
      new ScoutCenter.Data {
        ChancesForNationalGradeAthlete = new float[] { 0.01f, 0.04f, 0.07f, 0.1f, 0.15f },
        BonusChancesForRecruitCoach = new float[] { 0f, 0.04f, 0.07f, 0.1f, 0.15f },
        CostForUpgrade = new int[] { 0, 0, 0, 100_000 },
        MinimumFameForUpgrade = new int[] { 300, 500, 1_000, 3_000 },
      };
  }
}
