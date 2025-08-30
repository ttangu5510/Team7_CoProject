using System.Collections;
using System.Collections.Generic;
using JYL;
using SHG;
using UnityEngine;
using Zenject;

namespace JYL
{
    public class GameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // 선수 레포지토리
            Container.Bind<IDomAthRepository>()
                .To<DomAthRepository>()
                .AsSingle()
                .NonLazy();
            
            // 코치 레포지토리
            Container.Bind<ICoachRepository>()
                .To<CoachRepository>()
                .AsSingle()
                .NonLazy();
            
            // 상대 선수 레포지토리
            Container.Bind<IForAthRepository>()
                .To<ForAthRepository>()
                .AsSingle()
                .NonLazy();
            
            // 선수 서비스
            Container.Bind<DomAthService>()
                .To<DomAthService>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
            
            // 코치 서비스
            Container.Bind<CoachService>()
                .To<CoachService>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
    
            // 상대 선수 서비스
            Container.Bind<ForAthService>()
                .To<ForAthService>()
                .FromComponentInHierarchy()
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
    
            var touchControllerObject = this.Container.InstantiatePrefab(
                Resources.Load("TouchController"));
            DontDestroyOnLoad(touchControllerObject);
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
                .WithArguments(MatchDummyData.DummyData);
            
            // UI Manager 컨테이너에 주입
            Container.Bind<UIManager>()
                .To<UIManager>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}
