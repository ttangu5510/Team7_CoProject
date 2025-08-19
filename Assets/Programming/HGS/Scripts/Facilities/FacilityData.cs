
namespace SHG
{
  public interface IFacilityData
  {
    int MAX_UPGRADED_STAGE { get; }
    int GetRequiredFameForUpgradeFrom(int stage);
    int GetUpgradeCostFrom(int stage);
  }
}
