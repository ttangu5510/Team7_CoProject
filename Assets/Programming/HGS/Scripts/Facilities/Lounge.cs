using System;
using UniRx;

namespace SHG
{
  [Serializable]
  public class Lounge: IFacility
  {
    [Serializable]
    public struct FacilityData : IFacilityData {
      public string Name => "휴게실";
      public int MAX_UPGRADED_STAGE => 4;
      public int[] NumberOfAthletes;
      public int[] RecoveryAmounts;
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
    public ReactiveProperty<int> NumberOfAthletes { get; private set; }
    public ReactiveProperty<int> RecoveryAmount { get; private set; }
    public bool IsUpgradable => (this.CurrentStage.Value < this.Data.MAX_UPGRADED_STAGE);
    public IFacility.FacilityType Type => IFacility.FacilityType.Lounge;
    public FacilityData Data { get; private set; }

    public int MaxUpgradeStage => (this.Data.MAX_UPGRADED_STAGE);

    public Lounge(FacilityData data, int startStage = 0)
    {
      this.Data = data;
      this.CurrentStage = new (startStage);
      this.ResourcesNeeded = new (this.GetResourceNeededFrom(startStage));
      this.NumberOfAthletes = new (data.NumberOfAthletes[startStage]);
      this.RecoveryAmount = new (data.RecoveryAmounts[startStage]);
    }

    public void Upgrade() {
      int stage = this.CurrentStage.Value + 1;
      this.CurrentStage.Value = stage;
      this.NumberOfAthletes.Value = this.Data.NumberOfAthletes[stage];
      this.RecoveryAmount.Value = this.Data.RecoveryAmounts[stage];
      this.ResourcesNeeded.Value = this.GetResourceNeededFrom(stage);
    }

    (ResourceType type, int amount)[] GetResourceNeededFrom(int stage)
    {
      if (stage < this.Data.MAX_UPGRADED_STAGE) {
        return (new (ResourceType type, int amount)[] {
          (ResourceType.Fame, 
           this.Data.GetRequiredFameForUpgradeFrom(stage)),
          (ResourceType.Money, 
           this.Data.GetUpgradeCostFrom(stage)) });
      }
      else {
        return (new (ResourceType type, int amount)[0]);
      }
    }
  }
}
