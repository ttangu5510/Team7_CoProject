using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IDomAthRepository>()
            .To<DomAthRepository>()
            .AsSingle()
            .NonLazy();
        
        Container.Bind<ICoachRepository>()
            .To<CoachRepository>()
            .AsSingle()
            .NonLazy();
    }
}
