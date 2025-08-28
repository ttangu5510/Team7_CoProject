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
    public ReactiveProperty<(FacilityType type, IFacility facility)?> Selected { get; private set; }
    public IObservable<IFacility> SelectedFacilityStream { get; private set; }
    IObserver<IFacility> selectedFacilityObserver;

    public FacilitiesController(IEnumerable<IFacilityData> data, Dictionary<string, int> startStages = null)
    {
      this.Selected = new (null);
      this.SelectedFacilityStream = Observable.Create<IFacility>(
        observer => {
          this.selectedFacilityObserver = observer;
          return (Disposable.Empty);
        });
      this.data = new ();
      this.data.AddRange(data);
      this.InitFacilites(startStages);
      this.SubscribeChanges();
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

      var accomodationData = this.FindData<Accomodation.FacilityData>();
      this.Accomodation = new Accomodation(
        data: accomodationData,
        startStage: this.GetStartStageFrom(stages, accomodationData));
      this.Facilities[(int)this.Accomodation.Type] = this.Accomodation;

      var loungeData = this.FindData<Lounge.FacilityData>(); 
      this.Lounge = new Lounge(
        data: loungeData,
        startStage: this.GetStartStageFrom(stages, loungeData));
      this.Facilities[(int)this.Lounge.Type] = this.Lounge;

      var trainingCenterData = this.FindData<TrainingCenter.FacilityData>();
      this.TrainingCenter = new TrainingCenter(
        data: trainingCenterData,
        startStage: this.GetStartStageFrom(stages, trainingCenterData));
      this.Facilities[(int)this.TrainingCenter.Type] = this.TrainingCenter;

      var medicalCenterData = this.FindData<MedicalCenter.FacilityData>();
      this.MedicalCenter = new MedicalCenter(
        data: medicalCenterData, 
        startStage: this.GetStartStageFrom(stages, medicalCenterData));
      this.Facilities[(int)this.MedicalCenter.Type] = this.MedicalCenter;


      var scoutCenterData = this.FindData<ScoutCenter.FacilityData>();
      this.ScoutCenter = new ScoutCenter(
        data: scoutCenterData,
        startStage: this.GetStartStageFrom(stages, scoutCenterData));
      this.Facilities[(int)this.ScoutCenter.Type] = this.ScoutCenter;

    }

    void SubscribeChanges()
    {
      foreach (IFacility facility in this.Facilities) {
        this.AttachSelectedObserver(
        facility.CurrentStage
          .Merge(facility.ResourcesNeeded.Select(_ => 0)),
          facility: facility
          );
      }
      this.AttachSelectedObserver(
        observable: this.Accomodation.MaxNumberOfAthletes
        .Merge(this.Accomodation.NumberOfAthletes),
        facility: this.Accomodation);

      this.AttachSelectedObserver(
        observable: this.Lounge.NumberOfAthletes
        .Merge(this.Lounge.RecoveryAmount),
        facility: this.Lounge);

      this.AttachSelectedObserver(
        observable: this.TrainingCenter.BonusStat,
        facility: this.TrainingCenter);
      
      this.AttachSelectedObserver(
        observable: this.MedicalCenter.NumberOfAthletes
        .Merge(this.MedicalCenter.RecoveryAmount),
        facility: this.MedicalCenter);

      this.AttachSelectedObserver(
        observable: this.ScoutCenter.ChanceForNationalGradeAthlete
        .Merge(this.ScoutCenter.BonusChanceForRecruitCoach),
        facility: this.ScoutCenter);
    }

    void AttachSelectedObserver<T>(IObservable<T> observable, IFacility facility)
    {
      observable
        .Where(_ => this.IsSelectedFacility(facility.Type))
        .Subscribe(_ => this.selectedFacilityObserver.OnNext(facility));
    }

    bool IsSelectedFacility(FacilityType type)
    {
      return (this.Selected.Value != null && 
        this.Selected.Value.Value.type == type);
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
