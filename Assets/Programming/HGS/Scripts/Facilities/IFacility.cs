using UniRx;

namespace SHG
{
  public interface IFacility 
  {
    public string Name { get; }
    public ReactiveProperty<int> CurrentStage { get; }
    public bool IsUpgradable { get; }
    public void Upgrade();
    public ReactiveProperty<(ResourceType type, int amount)[]> ResourcesNeeded { get; }
  }
}
