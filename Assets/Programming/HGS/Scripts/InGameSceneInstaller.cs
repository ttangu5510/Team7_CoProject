using Zenject;

namespace SHG
{
  public class InGameSceneInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      this.Container.Bind<ITimeFlowController>()
        .To<TimeFlowController>()
        .AsSingle()
        .NonLazy();

      /***************************************************/
      // FIXME: Load facilities data
      FacilitiesController facilitiesController = new ();
      facilitiesController.Init(FacilityDummyData.AllData);
      /***************************************************/

      this.Container.Bind<IFacilitiesController>()
        .FromInstance(facilitiesController)
        .AsSingle();
    }
  }
}
