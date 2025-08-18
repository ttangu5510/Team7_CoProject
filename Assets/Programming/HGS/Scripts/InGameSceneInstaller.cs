using Zenject;

namespace SHG
{
  public class InGameSceneInstaller : MonoInstaller
  {
    public override void InstallBindings()
    {
      this.Container.Bind<ITimeFlowController>()
        .To<TimeFlowController>()
        .AsSingle();
    }
  }
}
