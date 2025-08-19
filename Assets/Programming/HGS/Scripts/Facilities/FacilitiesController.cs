using System.Collections.Generic;

namespace SHG
{
  public class FacilitiesController : IFacilitiesController
  {
    List<IFacilityData> data;
    public List<IFacility> Facilities { get; private set; }
    public Accomodation Accomodation { get; private set; }
    public Lounge Lounge { get; private set; }
    public TrainingCenter TrainingCenter { get; private set; }
    public MedicalCenter MedicalCenter { get; private set; }
    public ScoutCenter ScoutCenter { get; private set; }

    public FacilitiesController()
    {
    }

    public void Init(IEnumerable<IFacilityData> data, Dictionary<string, int> startStages = null)
    {
      this.data = new ();
      this.data.AddRange(data);
      this.InitFacilites(startStages);
    }

    void InitFacilites(Dictionary<string, int> stages = null)
    {
      this.Facilities = new ();
      var accomodationData = this.FindData<Accomodation.Data>();
      this.Accomodation = new Accomodation(
        data: accomodationData,
        startStage: this.GetStartStageFrom(stages, accomodationData));
      this.Facilities.Add(this.Accomodation);
      var loungeData = this.FindData<Lounge.Data>(); 
      this.Lounge = new Lounge(
        data: loungeData,
        startStage: this.GetStartStageFrom(stages, loungeData));
      this.Facilities.Add(this.Lounge);
      var medicalCenterData = this.FindData<MedicalCenter.Data>();
      this.MedicalCenter = new MedicalCenter(
        data: medicalCenterData, 
        startStage: this.GetStartStageFrom(stages, medicalCenterData));
      this.Facilities.Add(this.MedicalCenter);
      var scoutCenterData = this.FindData<ScoutCenter.Data>();
      this.ScoutCenter = new ScoutCenter(
        data: scoutCenterData,
        startStage: this.GetStartStageFrom(stages, scoutCenterData));
      this.Facilities.Add(this.ScoutCenter);
    }

    T FindData<T>() where T: IFacilityData
    {
      return ((T)this.data.Find( data => data is T));
    }

    int GetStartStageFrom(Dictionary<string, int> stages, IFacilityData data)
    {
      if (stages != null &&
        stages.TryGetValue(data.Name, out int stage)) {
        return (stage);
      }
      return (0);
    }
  }
}
