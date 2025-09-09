using JYL;
using UnityEngine;
using Zenject;

public class TitleSceneInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IUiManager>()
            .To<UIManager>() // 이거를 다른 스크립트로 IUiManager 상속받은걸로 변경하면 됨
            .FromComponentInHierarchy()
            .AsSingle()
            .NonLazy();
    }
}