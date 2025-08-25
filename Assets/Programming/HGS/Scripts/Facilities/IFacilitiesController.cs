using System;
using UniRx;

namespace SHG
{
  /// <summary>
  /// 시설들을 관리하는 역할
  /// </summary>
  public interface IFacilitiesController 
  {
    /// <summary>
    /// 전체 시설을 담고 있는 컨테이너
    /// </summary>
    public IFacility[] Facilities { get; }
    /********************** 각 시설들 **********************/
    public Accomodation Accomodation { get; }
    public Lounge Lounge { get; }
    public TrainingCenter TrainingCenter { get; }
    public MedicalCenter MedicalCenter { get; }
    public ScoutCenter ScoutCenter { get; }
    /****************************************************/
    /// <summary>
    /// 선택된 시설이 있거나 없음을 알려주는 기능
    /// </summary>
    public ReactiveProperty<(IFacility.FacilityType type, IFacility facility)?> Selected { get; }
    /// <summary>
    /// 선택된 시설이 바뀌지 않았지만 상태에 변화가 있을 경우 알려주는 기능
    /// </summary>
    public IObservable<IFacility> SelectedFacilityStream { get; }

    /// <summary>
    /// 원하는 타입에 해당하는 시설을 가져오는 기능
    /// </summary>
    T GetFacilityBy<T>(IFacility.FacilityType type) where T: IFacility;
    /// <summary>
    /// 특정 타입의 시설을 선택하는 기능
    /// </summary>
    public void SelectFacilityType(IFacility.FacilityType type);
    /// <summary>
    /// 현재 선택된 시설을 해제하는 기능
    /// </summary>
    public void UnSelectFacility();
  }
}
