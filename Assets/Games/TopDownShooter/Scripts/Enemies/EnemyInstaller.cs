using System.Collections;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class EnemyInstaller : Installer<EnemyInstaller>
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<EnemyContoller>().AsSingle();
            Container.Bind<EnemyMover>().AsSingle();
            Container.BindInterfacesAndSelfTo<EnemyMeleeHit>().AsSingle();
        }
    }
}