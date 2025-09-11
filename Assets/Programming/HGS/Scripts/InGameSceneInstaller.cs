using System.Collections.Generic;
using UnityEngine;
using Zenject;
using JYL;

namespace SHG
{
  public class InGameSceneInstaller : MonoInstaller
  {
    public override void InstallBindings() {

      var saveManager = this.Container.Resolve<ISaveManager>();
      var save = saveManager != null ? 
        saveManager.GetCurrentSave(): null;
      if (save == null) {
        Debug.LogError($"{nameof(ISaveManager.GetCurrentSave)} failed");
      }

      this.Container.Bind<IAthleteController>()
        .To<DummyAthleteController>()
        .AsSingle()
        .NonLazy();

      int year = ITimeFlowController.START_YEAR;
      int week = ITimeFlowController.START_WEEK;
      if (save != null) {
        year += save.time.yearCycle - 1;
        week += save.time.week - 1;
      }

      this.Container.Bind<ITimeFlowController>()
        .To<TimeFlowController>()
        .AsSingle()
        .WithArguments(year, week)
        .NonLazy();

      this.Container.Bind<IContenderController>()
        .To<ContendersController>()
        .AsSingle()
        .NonLazy();

      Dictionary<string, int> facilitieStages = new ();
      if (save != null && save.buildings != null) {
        foreach (var building in save.buildings) {
          facilitieStages.Add(building.buildingId, building.level);
        }
      }

      this.Container.Bind<IFacilitiesController>()
        .To<FacilitiesController>()
        .AsSingle()
        .WithArguments(
          FacilityTable.AllData, 
          facilitieStages)
        .NonLazy();
      
      int money = 0;
      int fame = 0;
      int coin = 0;
      if (save != null) {
        money = save.currencies.gold;
        fame = save.currencies.fame;
        coin = save.currencies.trainingCoin;
      }

      this.Container.Bind<IResourceController>()
        .To<ResourceController>()
        .AsSingle()
        .WithArguments(
          ResourceTable.Data,
          money, fame, coin);

      var touchControllerObject = this.Container.InstantiatePrefab(
        Resources.Load("TouchController"));
      TouchController touchController = touchControllerObject.GetComponent<TouchController>();

      this.Container.Bind<TouchController>()
        .FromInstance(touchController)
        .AsSingle();

      /***************************************************/
      //    TODO: Load match data
      /***************************************************/

      this.Container.Bind<IMatchController>()
        .To<MatchController>()
        .AsSingle()
        .WithArguments(
          MatchTable.Data);

            this.Container.Bind<IUiManager>()
                .To<UIManager>()
                .FromComponentInHierarchy()
                .AsSingle();
    }
  }
}
