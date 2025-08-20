using UniRx;

namespace SHG
{
  public interface IFacility 
  {
    public enum FacilityType
    {
      Accomodation,
      Lounge,
      TrainingCenter,
      MedicalCenter,
      ScoutCenter
    }

    public string Name { get; }
    public ReactiveProperty<int> CurrentStage { get; }
    public ReactiveProperty<(ResourceType type, int amount)[]> ResourcesNeeded { get; }
    public bool IsUpgradable { get; }
    public FacilityType Type { get; }
    public void Upgrade();
    public int MaxUpgradeStage { get; }
  }
}
