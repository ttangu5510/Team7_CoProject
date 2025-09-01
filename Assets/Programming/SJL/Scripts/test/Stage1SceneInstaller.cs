using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Stage1SceneInstaller : MonoInstaller
{
    [SerializeField] GameObject target;
    public override void InstallBindings()  // InstallBindings 오버라이드
    {
        Container.Bind<GameObject>().FromInstance(target);
        
    }



}
