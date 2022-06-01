using ModestTree;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Zenject;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Games.TDS
{
    public class GameController : ITickable
    {
        #region Game events
        public class StateChangedArgs
        {
            public GameStates NewState;
        }
        public delegate void StateChanged(StateChangedArgs args);
        public event StateChanged StateChangedEvent;
        #endregion

        readonly GameStateFactory _gameStateFactory;
        private readonly Player.Factory _playerFactory;
        private readonly ARSettings _arSettings;

        private readonly AnchorCreator _anchorCreator;
        private readonly PlanesSwitcher _planesSwitcher;
        GameState _currentGameState;

        bool _init = false;
        private Player _player;

        public GameState CurrentGameState { get => _currentGameState; }

        public GameController(
            GameStateFactory gameStateFactory,
            Player.Factory playerFactory,
            [InjectOptional]
            AnchorCreator anchorCreator,
            [InjectOptional]
            PlanesSwitcher planesSwitcher,
            ARSettings arSettings
            )
        {
            _gameStateFactory = gameStateFactory;
            _playerFactory = playerFactory;
            _arSettings = arSettings;
            _anchorCreator = anchorCreator;
            _planesSwitcher = planesSwitcher;
        }

        public void Tick()
        {
            if (!_init) {
                InitScene();
                _init = true;
            }

            switch (_currentGameState.State) {
                case GameStates.Placement:
                    UpdatePlacement();
                    break;
                case GameStates.Start:
                    UpdateStart();
                    break;
                case GameStates.Play:
                    break;
                case GameStates.Pause:
                    break;
                case GameStates.End:
                    break;
            }

            _currentGameState?.Update();
        }

        private void UpdatePlacement()
        {
            Assert.IsEqual( _arSettings.AREnable, true );
            if (_anchorCreator.TouchPlaceUpdate()) {
                _planesSwitcher.SetPlanesActive( false );
                _arSettings.Origin.transform.localScale = _arSettings.SceneScale * Vector3.one;
                ChangeState( GameStates.Start );
            }
        }

        private void UpdateStart()
        {
            _player = _playerFactory.Create();

            _player.transform.position = _player.transform.parent.position;
            _player.transform.rotation = _player.transform.parent.rotation;

            ChangeState( GameStates.Play );
        }


        private void InitScene()
        {
            if (_arSettings.AREnable) {
                _currentGameState = _gameStateFactory.CreateState( GameStates.Placement );
            } else {
                _currentGameState = _gameStateFactory.CreateState( GameStates.Start );
            }

            _currentGameState.Start();

            EnhancedTouchSupport.Enable();
        }

        private void ChangeState(GameStates state)
        {

            if (_currentGameState != null) {
                _currentGameState.Dispose();
                _currentGameState = null;
            }

            _currentGameState = _gameStateFactory.CreateState( state );
            _currentGameState.Start();

            StateChangedEvent?.Invoke(
                new StateChangedArgs() {
                    NewState = _currentGameState.State
                } );
        }
    }

    public enum GameStates
    {
        Placement,
        Start,
        Play,
        Pause,
        End
    }

    public abstract class GameState : IDisposable
    {
        public abstract GameStates State { get; }

        public virtual void Dispose() { }
        public virtual void Start() { }
        public virtual void Update() { }
    }

    public class GameStatePlacement : GameState
    {
        private readonly UISetup.Preset _uiPreset;

        public GameStatePlacement(UISetup.Preset uiPreset)
        {
            _uiPreset = uiPreset;
        }

        public override GameStates State => GameStates.Placement;

        public override void Start()
        {
            _uiPreset.FireStick.SetActive( false );
        }
        public override void Dispose()
        {
            _uiPreset.FireStick.SetActive( true );
        }

        public class Factory : PlaceholderFactory<GameStatePlacement> { }
    }

    public class GameStateStart : GameState
    {
        public override GameStates State => GameStates.Start;
        public class Factory : PlaceholderFactory<GameStateStart> { }
    }

    public class GameStatePlay : GameState
    {
        private readonly TargetingCursor _targetingCursor;

        public GameStatePlay(TargetingCursor targetingCursor)
        {
            _targetingCursor = targetingCursor;
        }

        public override GameStates State => GameStates.Play;


        public override void Start()
        {
            _targetingCursor.Active = true;
        }
        public class Factory : PlaceholderFactory<GameStatePlay> { }
    }

    public class GameStatePause : GameState
    {
        public override GameStates State => GameStates.Pause;
        public class Factory : PlaceholderFactory<GameStatePause> { }
    }

    public class GameStateEnd : GameState
    {
        public override GameStates State => GameStates.End;
        public class Factory : PlaceholderFactory<GameStateEnd> { }
    }

    public class GameStateFactory
    {
        private GameStatePlacement.Factory _fPlacement;
        private GameStateStart.Factory _fStart;
        private GameStatePlay.Factory _fPlay;
        private GameStatePause.Factory _fPause;
        private GameStateEnd.Factory _fEnd;

        public GameStateFactory(
            GameStatePlacement.Factory fPlacement,
            GameStateStart.Factory fStart,
            GameStatePlay.Factory fPlay,
            GameStatePause.Factory fPause,
            GameStateEnd.Factory fEnd)
        {
            _fPlacement = fPlacement;
            _fStart = fStart;
            _fPlay = fPlay;
            _fPause = fPause;
            _fEnd = fEnd;
        }

        public GameState CreateState(GameStates state)
        {
            switch (state) {
                case GameStates.Placement:
                    return _fPlacement.Create();
                case GameStates.Start:
                    return _fStart.Create();
                case GameStates.Play:
                    return _fPlay.Create();
                case GameStates.Pause:
                    return _fPause.Create();
                case GameStates.End:
                    return _fEnd.Create();
            }

            throw Assert.CreateException();
        }
    }
}
