using UnityEngine;
using Zenject;

public class TitleSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<UIManager>()
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
    }
}