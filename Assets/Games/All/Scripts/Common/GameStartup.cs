using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Games
{
    public class GameStartup : ITickable
    {
        readonly ARFeatures _arFeatures;
        [Inject]
        readonly Settings _settings = null;

        public GameStartup(ARFeatures arFeatures, Settings settings)
        {
            _arFeatures = arFeatures;
            _settings = settings;
        }

        public void Tick()
        {
            if (_arFeatures.IsReady) {
                _settings.NextScene.LoadScene();
            }
        }

        [Serializable]
        public class Settings
        {
            public MyBox.SceneReference NextScene;
        }
    }
}