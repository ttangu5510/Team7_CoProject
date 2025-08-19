using System.Collections.Generic;

namespace SHG
{
  public interface IFacilitiesController 
  {
    public List<IFacility> Facilities { get; }
    public Accomodation Accomodation { get; }
    public Lounge Lounge { get; }
    public TrainingCenter TrainingCenter { get; }
    public MedicalCenter MedicalCenter { get; }
    public ScoutCenter ScoutCenter { get; }

    public void Init(IEnumerable<IFacilityData> data, Dictionary<string, int> startStages = null);
  }
}
