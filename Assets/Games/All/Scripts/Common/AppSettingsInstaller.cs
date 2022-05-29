using MyBox;
using System;
using UnityEngine;
using Zenject;

namespace Games
{
    [CreateAssetMenu( fileName = "AppSettingsInstaller", menuName = "Installers/AppSettingsInstaller" )]
    public class AppSettingsInstaller : ScriptableObjectInstaller<AppSettingsInstaller>
    {
        public GameStartup.Settings StartupSettings;
        public GameModeSettings GameModeSettings;

        public override void InstallBindings()
        {
            Container.BindInstance( StartupSettings );
            Container.BindInstance( GameModeSettings );
        }
    }

    [Serializable]
    public class GameMode
    {
        public string Name;
        [Scene] public string SceneDefault;
        [Scene] public string SceneAR;
    }

    [Serializable]
    public class GameModeSettings
    {
        public GameMode[] GameModes;
    }
}