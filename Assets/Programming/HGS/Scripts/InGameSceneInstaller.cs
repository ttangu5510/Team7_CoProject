using UnityEngine;
using Zenject;
using JYL;

namespace SHG
{
  public class InGameSceneInstaller : MonoInstaller
  {
    public override void InstallBindings() {

      /***************************************************/
      //   FIXME: Remove dummy data
      /*
        this.Container.Bind<DomAthService>()
        .AsSingle()
        .WithArguments(
          this.CreateDomesticAthleteService())
        .NonLazy();
      */

      /***************************************************/
      this.Container.Bind<IAthleteController>()
        .To<DummyAthleteController>()
        .AsSingle()
        .NonLazy();

      this.Container.Bind<ITimeFlowController>()
        .To<TimeFlowController>()
        .AsSingle()
        .NonLazy();

      /***************************************************/
      //    TODO: Load facilities data
      /***************************************************/

      this.Container.Bind<IFacilitiesController>()
        .To<FacilitiesController>()
        .AsSingle()
        .WithArguments(FacilityDummyData.AllData)
        .NonLazy();

      /***************************************************/
      //    TODO: Load resources data
      /***************************************************/

      this.Container.Bind<IResourceController>()
        .To<ResourceController>()
        .AsSingle()
        .WithArguments(ResourceDummyData.Data);

      var touchControllerObject = this.Container.InstantiatePrefab(
        Resources.Load("TouchController"));
      DontDestroyOnLoad(touchControllerObject);
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
        .WithArguments(MatchDummyData.DummyData);
    }

    
    // DomAthService CreateDomesticAthleteService()
    // {
    //   var saveObject = new GameObject(nameof(SaveManager));
    //   DontDestroyOnLoad(saveObject);
    //   var saveManager = saveObject.AddComponent<SaveManager>();
    //   saveManager.Initialize();
    //   var repository = new DomAthRepository(saveManager);
    //   return (new DomAthService(repository));
    //   return null;
    // }
  }
}
