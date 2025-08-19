using System.Collections.Generic;
using UniRx;

namespace SHG
{
  public interface IFacilitiesController 
  {
    public IFacility[] Facilities { get; }
    public Accomodation Accomodation { get; }
    public Lounge Lounge { get; }
    public TrainingCenter TrainingCenter { get; }
    public MedicalCenter MedicalCenter { get; }
    public ScoutCenter ScoutCenter { get; }
    public ReactiveProperty<(IFacility.FacilityType type, IFacility facility)?> Selected { get; }

    public void Init(IEnumerable<IFacilityData> data, Dictionary<string, int> startStages = null);
    T GetFacilityBy<T>(IFacility.FacilityType type) where T: IFacility;
    public void SelectFacilityType(IFacility.FacilityType type);
    public void UnSelectFacility();
  }
}
