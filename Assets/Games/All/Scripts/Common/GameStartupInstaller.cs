using UnityEngine;
using Zenject;

namespace Games
{
    public class GameStartupInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<GameStartup>()
                .AsSingle()
                .NonLazy();
        }
    }
}