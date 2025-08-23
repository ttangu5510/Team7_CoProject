using System;
using UniRx;

namespace SHG
{
  [Serializable]
  public class TrainingCenter : IFacility
  {
    [Serializable]
    public struct FacilityData : IFacilityData {
      public int MAX_UPGRADED_STAGE => 4;
      public string Name => "훈련 센터";
      public int[] BonusStats;
      public int[] MinimumFameForUpgrade;
      public int[] CostForUpgrade;

      public int GetRequiredFameForUpgradeFrom(int stage) {
        return (this.MinimumFameForUpgrade[stage]);
      }

      public int GetUpgradeCostFrom(int stage) {
        return (this.CostForUpgrade[stage]);
      }
    }

    public string Name => (this.Data.Name);
    public ReactiveProperty<int> CurrentStage { get; private set; }
    public ReactiveProperty<(ResourceType type, int amount)[]> ResourcesNeeded { get; private set; }
    public ReactiveProperty<int> BonusStat { get; private set; }
    public bool IsUpgradable => (this.CurrentStage.Value < this.Data.MAX_UPGRADED_STAGE);
    public IFacility.FacilityType Type => IFacility.FacilityType.TrainingCenter;
    public int MaxUpgradeStage => (this.Data.MAX_UPGRADED_STAGE);
    public FacilityData Data { get; private set; }

    public TrainingCenter(FacilityData data, int startStage = 0)
    {
      this.Data = data;
      this.CurrentStage = new (startStage);
      this.BonusStat = new (data.BonusStats[startStage]);
      this.ResourcesNeeded = new (this.GetResourceNeededFrom(startStage));
    }

    public void Upgrade() {
      int stage = this.CurrentStage.Value + 1;
      this.CurrentStage.Value = stage;
      this.ResourcesNeeded.Value = this.GetResourceNeededFrom(stage);
      this.BonusStat.Value = this.Data.BonusStats[stage];
    }

    (ResourceType type, int amount)[] GetResourceNeededFrom(int stage)
    {
      if (stage < this.Data.MAX_UPGRADED_STAGE) {
        return (new (ResourceType type, int amount)[] {
          (ResourceType.Fame, 
           this.Data.GetRequiredFameForUpgradeFrom(stage)),
          (ResourceType.Money, this.Data.GetUpgradeCostFrom(stage)) });
      }
      else {
        return (new (ResourceType type, int amount)[0]);
      }
    }
  }
}
