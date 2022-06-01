using System;
using UnityEngine;
using Zenject;
using MyBox;
using UnityEngine.XR.ARFoundation;

namespace Games.TDS
{
    public class TDSInstaller : MonoInstaller
    {
        [SerializeField] Settings _settings;
        public override void InstallBindings()
        {
            InstallGameEngine();

            InstallScene();
            InstallGameStates();
            InstallPlayer();
            InstallUI();

            Container.BindInstance( _settings.ARSettings ).AsSingle();
            if (_settings.ARSettings.AREnable) {
                Container.Bind<AnchorCreator>().AsSingle();
                Container.BindInstance( _settings.ARSettings.Origin ).AsSingle();
                Container.BindInstance( _settings.ARSettings.PlaneManager ).AsSingle();
                Container.BindInstance( _settings.ARSettings.RaycastManager ).AsSingle();
                Container
                    .BindInstance( _settings.SceneRoot )
                    .AsSingle()
                    .WhenInjectedInto<AnchorCreator>();

                Container.Bind<PlanesSwitcher>().AsSingle();
            }

            Container.BindInstance( _settings.Camera ).AsSingle();

            Container
                .BindFactory<CastPoint, Anchor, Anchor.Factory>()
                .FromNewComponentOnNewGameObject()
                .WithGameObjectName( "Anchor" );

            Container
                .BindFactory<Bullet, Bullet.Factory>()
                .FromPoolableMemoryPool<Bullet, Bullet.Pool>( poolBinder => poolBinder
                    .WithInitialSize( 20 )
                    .FromNewComponentOnNewPrefab( _settings.BulletPrefab )
                    .WithGameObjectName( "Bullet" )
                    .UnderTransform( _settings.SceneRoot )
                );

            Container
                .BindFactory<Enemy, Enemy.Factory>()
                .FromPoolableMemoryPool<Enemy, Enemy.Pool>( poolBuilder => poolBuilder
                     .WithInitialSize( 10 )
                     .FromSubContainerResolve()
                     .ByNewPrefabInstaller<EnemyInstaller>( _settings.EnemyPrefab )
                     .WithGameObjectName( "Enemy" )
                     .UnderTransform( _settings.SceneRoot )
                );
        }

        private void InstallGameEngine()
        {
            Container.BindInterfacesAndSelfTo<GameController>().AsSingle().NonLazy();

            Container
                .Bind<BulletSpawnService>()
                .AsSingle();

            Container.Bind<HitService>().AsSingle();

            Container.BindInterfacesAndSelfTo<EnemySpawner>().AsSingle().NonLazy();
        }

        private void InstallUI()
        {
            Container
                .Bind<TargetingContoller>()
                .AsSingle();

            Container
                .BindInterfacesAndSelfTo<TargetingCursor>()
                .AsSingle()
                .NonLazy();

            Container.BindInstance( _settings.UIContext );

            Container
                .BindInterfacesAndSelfTo<UISetup>()
                .AsSingle()
                .NonLazy();
        }

        private void InstallPlayer()
        {
            Container
                .BindFactory<Player, Player.Factory>()
                .FromComponentInNewPrefab( _settings.PlayerPrefab )
                .WithGameObjectName( "Player" )
                .UnderTransform( _settings.SceneRoot );

            Container.BindInterfacesAndSelfTo<PlayerController>().AsSingle();

            Container.Bind<PlayerRegistry>().AsSingle();
        }

        private void InstallScene()
        {
            if (!_settings.ARSettings.AREnable) {
                Container.BindInterfacesAndSelfTo<CameraController>().AsSingle().NonLazy();
            }
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
            public Transform SceneRoot;
            [MustBeAssigned] public Player PlayerPrefab;
            [MustBeAssigned] public GameObject BulletPrefab;
            public GameObject EnemyPrefab;
            public Camera Camera;

            public UISetup.Preset UIContext;

            public ARSettings ARSettings;
        }

        
    }
    [Serializable]
    public class ARSettings
    {
        public bool AREnable;
        public ARSessionOrigin Origin;
        public ARRaycastManager RaycastManager;
        public ARPlaneManager PlaneManager;
        public float SceneScale;
    }
}