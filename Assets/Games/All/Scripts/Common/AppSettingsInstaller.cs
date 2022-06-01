using MyBox;
using System;
using System.Collections.Generic;
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
        public SceneReference SceneDefault;
        public SceneReference SceneAR;
    }

    [Serializable]
    public class GameModeSettings
    {
        public GameMode[] GameModes;
    }
}