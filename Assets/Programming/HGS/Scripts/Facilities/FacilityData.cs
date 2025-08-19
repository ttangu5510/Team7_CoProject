
namespace SHG
{
  public interface IFacilityData
  {
    public string Name { get; }
    int MAX_UPGRADED_STAGE { get; }
    int GetRequiredFameForUpgradeFrom(int stage);
    int GetUpgradeCostFrom(int stage);
  }
}
