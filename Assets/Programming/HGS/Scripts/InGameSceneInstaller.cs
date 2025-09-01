using UnityEngine;
using Zenject;

namespace SHG
{
  public class InGameSceneInstaller : MonoInstaller
  {
    public override void InstallBindings() {

      this.Container.Bind<IAthleteController>()
        .To<DummyAthleteController>()
        .AsSingle()
        .NonLazy();

      this.Container.Bind<ITimeFlowController>()
        .To<TimeFlowController>()
        .AsSingle()
        .NonLazy();

      this.Container.Bind<IContenderController>()
        .To<ContendersController>()
        .AsSingle()
        .NonLazy();

      /***************************************************/
      //    TODO: Load facilities data
      /***************************************************/

      this.Container.Bind<IFacilitiesController>()
        .To<FacilitiesController>()
        .AsSingle()
        .WithArguments(FacilityTable.AllData)
        .NonLazy();

      /***************************************************/
      //    TODO: Load resources data
      /***************************************************/

      this.Container.Bind<IResourceController>()
        .To<ResourceController>()
        .AsSingle()
        .WithArguments(ResourceTable.Data);

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
        .WithArguments(MatchTable.Data);
    }
  }
}
