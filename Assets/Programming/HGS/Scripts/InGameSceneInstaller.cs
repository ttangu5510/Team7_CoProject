using UnityEngine;
using Zenject;

namespace SHG
{
  public class InGameSceneInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {

      /***************************************************/
      //   FIXME: Remove Dummy data
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

      var touchControllerObject = new GameObject(nameof(TouchController));
      DontDestroyOnLoad(touchControllerObject);
      TouchController touchController = touchControllerObject.AddComponent<TouchController>();

      this.Container.Bind<TouchController>()
        .FromInstance(touchController)
        .AsSingle();
    }
  }
}
