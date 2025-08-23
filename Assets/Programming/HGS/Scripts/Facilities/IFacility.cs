using UniRx;

namespace SHG
{
  /// <summary>
  /// 모든 시설에 대한 공통 인터페이스
  /// </summary>
  public interface IFacility 
  {
    /// <summary>
    /// 시설에 해당하는 타입, 현재는 각 타입에 시설 한개씩이 존재하는 것으로 상정
    /// </summary>
    public enum FacilityType
    {
      Accomodation,
      Lounge,
      TrainingCenter,
      MedicalCenter,
      ScoutCenter
    }

    public string Name { get; }
    /// <summary>
    /// 업그레이드 단계를 알려주는 기능
    /// </summary>
    public ReactiveProperty<int> CurrentStage { get; }
    /// <summary>
    /// 다음 업그레이드시 필요한 자원들을 알려주는 기능
    /// </summary>
    public ReactiveProperty<(ResourceType type, int amount)[]> ResourcesNeeded { get; }
    /// <summary>
    /// 시설이 업그레이드 가능한 상태인지를 나타내는 기능
    /// </summary>
    public bool IsUpgradable { get; }
    public FacilityType Type { get; }
    /// <summary>
    /// 시설을 다음 단계로 업그레이드하는 기능
    /// </summary>
    public void Upgrade();
    public int MaxUpgradeStage { get; }
    /// <summary>
    /// 시설을 업그레이드 하면 소요되는 시간
    /// </summary>
    public virtual int WeeksForUpgrade => 1;
  }
}
