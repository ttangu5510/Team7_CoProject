
namespace SHG
{
  /// <summary>
  /// 시설에 대한 초기 데이터를 관리하는 기능
  /// </summary>
  public interface IFacilityData
  {
    /// <summary>
    /// 시설에 대한 한글 이름
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 최대로 업그레이드 할 수 있는 단계
    /// </summary>
    int MAX_UPGRADED_STAGE { get; }

    /// <summary>
    /// 현재 단계에서 다음 단계로 업그레이드 하기 위해 필요한 명성치를 알려주는 기능
    /// </summary>
    int GetRequiredFameForUpgradeFrom(int stage);

    /// <summary>
    /// 현재 단계에서 다음 단계로 업그레이드 하기 위해 필요한 골드를 알려주는 기능
    /// </summary>
    int GetUpgradeCostFrom(int stage);
  }
}
