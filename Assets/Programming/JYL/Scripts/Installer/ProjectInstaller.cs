using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ISaveManager>()
            .To<SaveManager>()
            .AsSingle()
            .NonLazy();
    }
}
