using MyBox;
using UnityEngine;
using Zenject;

namespace Games
{
    public class BootstrapInstaller : MonoInstaller
    {
        [SerializeField, MustBeAssigned] AsyncProcessor asyncProcessor;

        public override void InstallBindings()
        {            
            Container
                .Bind<AsyncProcessor>()
                .FromComponentInNewPrefab(asyncProcessor)
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<ARFeatures>()
                .AsSingle();
        }
    }
}