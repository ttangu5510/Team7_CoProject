using System;
using UniRx;

namespace SHG
{
  public class ScoutCenter : IFacility
  {
    [Serializable]
    public struct Data : IFacilityData {
      public int MAX_UPGRADED_STAGE => 4;
      public string Name => "스카우트 센터";
      public float[] ChancesForNationalGradeAthlete;
      public float[] BonusChancesForRecruitCoach;
      public int[] CostForUpgrade;
      public int[] MinimumFameForUpgrade;

      public int GetRequiredFameForUpgradeFrom(int stage) {
        return (this.MinimumFameForUpgrade[stage]);
      }

      public int GetUpgradeCostFrom(int stage) {
        return (this.CostForUpgrade[stage]);
      }
    }

    public string Name => (this.data.Name);
    public bool IsUpgradable => (this.CurrentStage.Value < this.data.MAX_UPGRADED_STAGE);
    public ReactiveProperty<int> CurrentStage { get; private set; }
    public ReactiveProperty<float> ChanceForNationalGradeAthlete;
    public ReactiveProperty<float> BonusChanceForRecruitCoach;
    public ReactiveProperty<(ResourceType type, int amount)[]> ResourcesNeeded { get; private set; }
    public IFacility.FacilityType Type => IFacility.FacilityType.ScoutCenter;
    Data data;

    public ScoutCenter(Data data, int startStage = 0)
    {
      this.data = data; 
      this.CurrentStage = new (startStage);
      this.ResourcesNeeded = new (this.GetResourceNeededFrom(startStage));
      this.ChanceForNationalGradeAthlete = new (data.ChancesForNationalGradeAthlete[startStage]);
      this.BonusChanceForRecruitCoach = new (data.BonusChancesForRecruitCoach[startStage]);
    }

    public void Upgrade() {
      int stage = this.CurrentStage.Value + 1;
      this.CurrentStage.Value = stage;
      this.ResourcesNeeded.Value = this.GetResourceNeededFrom(stage);
      this.ChanceForNationalGradeAthlete.Value = this.data.ChancesForNationalGradeAthlete[stage];
      this.BonusChanceForRecruitCoach.Value = this.data.BonusChancesForRecruitCoach[stage];
    }
    
    (ResourceType type, int amount)[] GetResourceNeededFrom(int stage)
    {
      if (stage < this.data.MAX_UPGRADED_STAGE) {
        return (new (ResourceType type, int amount)[] {
          (ResourceType.Fame, 
           this.data.GetRequiredFameForUpgradeFrom(stage)),
          (ResourceType.Money, this.data.GetUpgradeCostFrom(stage)) });
      }
      else {
        return (new (ResourceType type, int amount)[0]);
      }
    }
  }

}
