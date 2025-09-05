using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using EditorAttributes;
using JYL;

namespace SHG
{
  public class SaveAdaptor : MonoBehaviour
  {
    [Inject]
    ISaveManager saveManager;
    [Inject]
    IResourceController resourceController;
    [Inject]
    ITimeFlowController timeFlowController;
    [Inject] 
    IFacilitiesController facilityController;

    void Awake()
    {
      if (this.saveManager == null || this.saveManager.GetCurrentSave() == null) {

        Debug.LogError($"{nameof(ISaveManager.GetCurrentSave)} Failed");
        Destroy(this.gameObject);
      }
    }

    void Start()
    {
      this.timeFlowController.BeforeProgress += this.SaveProgress;
      this.OnDestroyAsObservable()
        .Subscribe(_ => this.timeFlowController.BeforeProgress -= this.SaveProgress);
      this.resourceController.Money
        .Subscribe(money => 
            this.saveManager.GetCurrentSave().currencies.gold = money)
        .AddTo(this);
      this.resourceController.Fame
        .Subscribe(fame => 
          this.saveManager.GetCurrentSave().currencies.fame = fame)
        .AddTo(this);
      this.resourceController.Coin
        .Subscribe(coin => 
          this.saveManager.GetCurrentSave().currencies.trainingCoin = coin)
        .AddTo(this);
      this.timeFlowController.WeekInYear
        .Subscribe(week =>
          this.saveManager.GetCurrentSave().time.week = week)
        .AddTo(this);
      this.timeFlowController.CurrentSeason
        .Subscribe(season => this.saveManager.GetCurrentSave().time.season = (JWS.Season)season);
      this.timeFlowController.Year
        .Subscribe(year =>
          this.saveManager.GetCurrentSave().time.yearCycle = year)
        .AddTo(this);
      this.facilityController.Accomodation.CurrentStage
        .Skip(1)
        .Subscribe(stage => this.OnFacilityStageChanged(this.facilityController.Accomodation, stage))
        .AddTo(this);
    }

    void OnFacilityStageChanged(IFacility facility, int stage)
    {
      var save = this.saveManager.GetCurrentSave();
      int index = save.buildings.FindIndex(
        building => building.buildingId == facility.Name);
      if (index != -1) {
        save.buildings[index].level = stage;
      }
      else {
        save.buildings.Add(new JWS.BuildingState {
          buildingId = facility.Name,
          level = stage
          });
      }
    }
    
    [Button]
    void AutoSave()
    {
      this.saveManager.AutoSave();
    }

    [Button]
    void GetCurrentSave()
    {
      var currentSave = this.saveManager.GetCurrentSave();
      if (currentSave != null) {
        Debug.Log($"currentSave: {currentSave}");
      }
      else {
        Debug.LogError($"{nameof(currentSave)} is null");
      }
    }

    void SaveProgress()
    {
      this.saveManager.AutoSave();
    }
  }
}
