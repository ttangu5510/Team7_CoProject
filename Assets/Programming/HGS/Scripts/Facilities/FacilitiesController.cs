using System;
using System.Collections.Generic;
using UniRx;

namespace SHG
{
  using FacilityType = IFacility.FacilityType;

  public class FacilitiesController : IFacilitiesController
  {
    List<IFacilityData> data;
    public IFacility[] Facilities { get; private set; }
    public Accomodation Accomodation { get; private set; }
    public Lounge Lounge { get; private set; }
    public TrainingCenter TrainingCenter { get; private set; }
    public MedicalCenter MedicalCenter { get; private set; }
    public ScoutCenter ScoutCenter { get; private set; }
    public ReactiveProperty<(FacilityType, IFacility)?> Selected { get; private set; }

    public FacilitiesController()
    {
      this.Selected = new (null);
    }

    public void Init(IEnumerable<IFacilityData> data, Dictionary<string, int> startStages = null)
    {
      this.data = new ();
      this.data.AddRange(data);
      this.InitFacilites(startStages);
    }

    public void SelectFacilityType(FacilityType type)
    {
      IFacility facility = this.Facilities[(int)type];
      this.Selected.Value = (type, facility);
    }

    public void UnSelectFacility()
    {
      this.Selected.Value = null;
    }

    public T GetFacilityBy<T>(FacilityType type) where T: IFacility
    {
      return ((T)Array.Find(this.Facilities, facility => facility is T));
    }

    void InitFacilites(Dictionary<string, int> stages = null)
    {
      this.Facilities = new IFacility[System.Enum.GetValues(typeof(FacilityType)).Length];

      var accomodationData = this.FindData<Accomodation.Data>();
      this.Accomodation = new Accomodation(
        data: accomodationData,
        startStage: this.GetStartStageFrom(stages, accomodationData));
      this.Facilities[(int)this.Accomodation.Type] = this.Accomodation;

      var loungeData = this.FindData<Lounge.Data>(); 
      this.Lounge = new Lounge(
        data: loungeData,
        startStage: this.GetStartStageFrom(stages, loungeData));
      this.Facilities[(int)this.Lounge.Type] = this.Lounge;

      var trainingCenterData = this.FindData<TrainingCenter.Data>();
      this.TrainingCenter = new TrainingCenter(
        data: trainingCenterData,
        startStage: this.GetStartStageFrom(stages, trainingCenterData));
      this.Facilities[(int)this.TrainingCenter.Type] = this.TrainingCenter;

      var medicalCenterData = this.FindData<MedicalCenter.Data>();
      this.MedicalCenter = new MedicalCenter(
        data: medicalCenterData, 
        startStage: this.GetStartStageFrom(stages, medicalCenterData));
      this.Facilities[(int)this.MedicalCenter.Type] = this.MedicalCenter;


      var scoutCenterData = this.FindData<ScoutCenter.Data>();
      this.ScoutCenter = new ScoutCenter(
        data: scoutCenterData,
        startStage: this.GetStartStageFrom(stages, scoutCenterData));
      this.Facilities[(int)this.ScoutCenter.Type] = this.ScoutCenter;

    }

    T FindData<T>() where T: IFacilityData
    {
      return ((T)this.data.Find(data => data is T));
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
