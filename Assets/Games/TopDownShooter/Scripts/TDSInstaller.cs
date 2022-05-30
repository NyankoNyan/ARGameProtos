using System;
using UnityEngine;
using Zenject;
using MyBox;

namespace Games.TDS
{
    public class TDSInstaller : MonoInstaller
    {
        [SerializeField] Settings _settings;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();

            InstallScene();
            InstallGameStates();

            Container.BindInstance( _settings.Spawners ).AsSingle();

            InstallPlayer();

            Container.BindInterfacesAndSelfTo<CameraController>().AsSingle();
            Container.BindFactory<Camera, PCTestCamera, PCTestCamera.Factory>();

            Container
                .BindFactory<CastPoint, Anchor, Anchor.Factory>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName( "Anchor" );

            Container
                .Bind<TargetingContoller>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<TargetingCursor>()
                .AsSingle()
                .NonLazy();

            Container
                .BindFactory<Bullet, Bullet.Factory>()
                .FromPoolableMemoryPool<Bullet, Bullet.Pool>( poolBinder => poolBinder
                    .WithInitialSize( 20 )
                    .FromNewComponentOnNewPrefab( _settings.BulletPrefab )
                    .WithGameObjectName( "Bullet" )
                    .UnderTransformGroup( "Bullets" )
                );

            Container
                .Bind<BulletSpawnService>()
                .AsSingle();

            Container.BindInstance( _settings.UIContext );

            Container
                .BindInterfacesAndSelfTo<UISetup>()
                .AsSingle()
                .NonLazy();

            Container
                .BindFactory<Enemy, Enemy.Factory>()
                .FromPoolableMemoryPool<Enemy, Enemy.Pool>( poolBuilder => poolBuilder
                     .WithInitialSize( 10 )
                     .FromSubContainerResolve()
                     .ByNewPrefabInstaller<EnemyInstaller>( _settings.EnemyPrefab )
                     .WithGameObjectName( "Enemy" )
                     .UnderTransformGroup( "Enemies" )
                );

            Container.Bind<HitService>().AsSingle();

            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle().NonLazy();

            Container.Bind<ARRefs>().AsSingle();
        }

        private void InstallPlayer()
        {
            Container
                .BindFactory<Player, Player.Factory>()
                .FromComponentInNewPrefab( _settings.PlayerPrefab )
                .WithGameObjectName( "Player" );

            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle();

            Container.Bind<PlayerRegistry>().AsSingle();
        }

        private void InstallScene()
        {
            Container.Bind<SceneSetupFactory>().AsSingle();

            Container.BindFactory<SceneSetupAR, SceneSetupAR.Factory>().AsSingle().WhenInjectedInto<SceneSetupFactory>();
            Container.BindFactory<SceneSetupPCTest, SceneSetupPCTest.Factory>().AsSingle().WhenInjectedInto<SceneSetupFactory>();
        }

        private void InstallGameStates()
        {
            Container.Bind<GameStateFactory>().AsSingle();

            Container.BindFactory<GameStatePlacement, GameStatePlacement.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<GameStateStart, GameStateStart.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<GameStatePlay, GameStatePlay.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<GameStatePause, GameStatePause.Factory>().WhenInjectedInto<GameStateFactory>();
            Container.BindFactory<GameStateEnd, GameStateEnd.Factory>().WhenInjectedInto<GameStateFactory>();
        }

        [Serializable]
        public class Settings
        {
            [MustBeAssigned] public LevelSpawners Spawners;
            [MustBeAssigned] public Player PlayerPrefab;
            [MustBeAssigned] public GameObject BulletPrefab;
            public GameObject EnemyPrefab;

            public UISetup.Context UIContext;
        }
    }
}