using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings() // 어웨이크보다 빠름
    {
        Container.BindInterfacesAndSelfTo<SaveManager>()
            .AsSingle()
            .NonLazy();
    }
}
