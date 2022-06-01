using UnityEngine;
using Zenject;

namespace Games.TDS
{
    [CreateAssetMenu( fileName = "TDSSettingsInstaller", menuName = "Installers/TDSSettingsInstaller" )]
    public class TDSSettingsInstaller : ScriptableObjectInstaller<TDSSettingsInstaller>
    {
        //public SceneSetupAR.Settings SceneARSettings;
        //public SceneSetupPCTest.Settings ScenePCSettings;
        public Player.Settings PlayerSettings;
        public PlayerController.Settings PlayerControllerSettings;
        public TargetingCursor.Settings CursorSettings;
        public Bullet.Settings BulletSettings;
        public UISetup.Settings UISettings;
        public HitService.Settings HitSettings;
        public EnemySpawner.Settings EnemySpawnSettings;
        public Enemy.Settings EnemySettings;

        public override void InstallBindings()
        {
            //Container.BindInstance( SceneARSettings );
            //Container.BindInstance( ScenePCSettings );
            Container.BindInstance( PlayerSettings );
            Container.BindInstance( PlayerControllerSettings );
            Container.BindInstance( CursorSettings );
            Container.BindInstance( BulletSettings );
            Container.BindInstance( UISettings );
            Container.BindInstance( HitSettings );
            Container.BindInstance( EnemySpawnSettings );
            Container.BindInstance( EnemySettings );
        }
    }
}