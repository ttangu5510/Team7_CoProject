using System;
using System.Text;
using UnityEngine;
using Zenject;
using EditorAttributes;
using UniRx;
using UniRx.Triggers;

namespace SHG
{
  using FacilityType = IFacility.FacilityType;

  public class FacilityTester : MonoBehaviour
  {

    [Inject]
    IFacilitiesController facilitiesController;

    [SerializeField]
    FacilityType facilityToSelect;

    [SerializeField] [ReadOnly]
    string Name;
    [SerializeField] [ReadOnly]
    int CurrentGrade;
    [SerializeField] [ReadOnly]
    bool IsUpgradable;
    [SerializeField] [ReadOnly]
    string resourcesString;

    IFacility selectedFacility;
    [SerializeField] [ReadOnly]
    Accomodation accomodation;
    [SerializeField] [ReadOnly]
    Lounge lounge;
    [SerializeField] [ReadOnly]
    TrainingCenter trainingCenter;
    [SerializeField] [ReadOnly]
    MedicalCenter medicalCenter;
    [SerializeField] [ReadOnly]
    ScoutCenter scoutCenter;
    CompositeDisposable disposables;

    void Start()
    {
      this.accomodation = this.facilitiesController.Accomodation;
      this.lounge = facilitiesController.Lounge;
      this.trainingCenter = this.facilitiesController.TrainingCenter;
      this.medicalCenter = this.facilitiesController.MedicalCenter;
      this.scoutCenter = this.facilitiesController.ScoutCenter;
      this.disposables = new ();

      foreach (var facility in this.facilitiesController.Facilities) {
        facility.CurrentStage
          .Where(_ => this.IsSelectedFacility(facility))
          .Subscribe(stage => {
              this.CurrentGrade = stage;
              this.IsUpgradable = facility.IsUpgradable;
            })
          .AddTo(this.disposables);
        facility.ResourcesNeeded
          .Where(_ => this.IsSelectedFacility(facility))
          .Subscribe(resources => this.resourcesString = this.MakeResourcesString(resources))
          .AddTo(this.disposables);
      }
      
      this.facilitiesController.Selected
        .Subscribe(this.OnSelectFacility)
        .AddTo(this.disposables);

      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposables.Dispose());
    }

    string MakeResourcesString(in (ResourceType type, int amount)[] resources)
    {
      var builder = new StringBuilder();
      foreach (var (type, amount) in resources) {
        builder.Append($"[{type}: {amount}] "); 
      }
      return (builder.ToString());
    }

    bool IsSelectedFacility(in IFacility facility)
    {
      var selected = this.facilitiesController.Selected.Value; 
      return (selected != null && selected.Value.type == facility.Type);
    }

    void OnSelectFacility((IFacility.FacilityType type, IFacility facility)? args)
    {
      if (args == null) {
        this.Name = String.Empty;
        this.CurrentGrade = -1;
        this.resourcesString = String.Empty;
        this.IsUpgradable = false;
      }
      else {
        var facility = args.Value.facility;
        this.Name = facility.Name;
        this.CurrentGrade = facility.CurrentStage.Value;
        this.resourcesString = this.MakeResourcesString(facility.ResourcesNeeded.Value);
        this.IsUpgradable = facility.IsUpgradable;
      }
    }

    [Button]
    void Select()
    {
      this.facilitiesController.SelectFacilityType(this.facilityToSelect);
    }

    [Button]
    void UnSelect()
    {
      this.facilitiesController.UnSelectFacility();
    }

    [Button]
    void Upgrade()
    {
      var selected = this.facilitiesController.Selected.Value;
      if (selected == null) {
        return ;
      }
      if (selected.Value.facility.IsUpgradable) {
        selected.Value.facility.Upgrade();
      }
    }
  }
}
