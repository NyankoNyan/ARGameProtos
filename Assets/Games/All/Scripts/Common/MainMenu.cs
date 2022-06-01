using Zenject;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Games
{
    public class MainMenu
    {
        private GameModeSettings _gameModeSettings;
        private Button.Factory _buttonFactory;
        private ARFeatures _arFeature;
        private ButtonList _buttonList;

        public MainMenu(ButtonList buttonList, GameModeSettings gameModeSettings, Button.Factory buttonFactory, ARFeatures arFeature)
        {
            _gameModeSettings = gameModeSettings;
            _buttonFactory = buttonFactory;
            _arFeature = arFeature;
            _buttonList = buttonList;

            for (int i = 0; i < _gameModeSettings.GameModes.Length; i++) {
                GameMode gameMode = _gameModeSettings.GameModes[i];
                var sceneName = _arFeature.IsARSupported ? gameMode.SceneAR.SceneName : gameMode.SceneDefault.SceneName;
                Button newButton = _buttonFactory.Create( sceneName, gameMode.Name, ChangeScene );
                RectTransform rect = newButton.GetComponent<RectTransform>();
                rect.localPosition = new Vector2( 0, buttonList.TopOffset + buttonList.VerticalOffset * i );
            }
        }

        public void ChangeScene(string scene)
        {
            SceneManager.LoadScene( scene, LoadSceneMode.Single );
        }
    }
}