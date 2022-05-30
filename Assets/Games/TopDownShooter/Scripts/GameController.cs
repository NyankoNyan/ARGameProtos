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

        private readonly DiContainer _container;
        readonly GameStateFactory _gameStateFactory;
        readonly ARFeatures _arFeatures;
        readonly SceneSetupFactory _sceneSetupFactory;
        private readonly Anchor.Factory _anchorFactory;
        private readonly Player.Factory _playerFactory;
        private readonly LevelSpawners _spawners;
        private readonly TargetingContoller _targetingContoller;
        private readonly ARRefs _arRefs;
        GameState _currentGameState;
        private SceneSetup _sceneSetup;

        bool _init = false;
        private Anchor _anchor;
        private Plane _anchorPlane;
        private Player _player;

        public GameState CurrentGameState { get => _currentGameState; }
        public SceneSetup SceneSetup { get => _sceneSetup; }

        public GameController(
            DiContainer container,
            GameStateFactory gameStateFactory,
            ARFeatures arFeatures,
            SceneSetupFactory sceneSetupFactory,
            Anchor.Factory anchorFactory,
            Player.Factory playerFactory,
            LevelSpawners spawners,
            TargetingContoller targetingContoller,
            ARRefs arRefs)
        {
            _container = container;
            _gameStateFactory = gameStateFactory;
            _arFeatures = arFeatures;
            _sceneSetupFactory = sceneSetupFactory;
            _anchorFactory = anchorFactory;
            _playerFactory = playerFactory;
            _spawners = spawners;
            _targetingContoller = targetingContoller;
            _arRefs = arRefs;
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
            if (Touch.activeFingers.Count == 0) {
                return;
            }

            Touch touch = Touch.activeFingers[0].currentTouch;
            if (touch.phase != TouchPhase.Began) {
                return;
            }

            if (EventSystem.current.IsPointerOverGameObject( touch.touchId )) {
                return;
            }

            CastPoint point = _sceneSetup.CastScreenRay( touch.screenPosition, false );

            if (point != null) {
                if (_arFeatures.IsARSupported) {
                    _anchor = _anchorFactory.Create( point );
                    _arRefs.Origin.MakeContentAppearAt( _anchor.transform, _anchor.transform.position, _anchor.transform.rotation );
                    _arRefs.Origin.transform.localScale = 2 * Vector3.one;
                    _anchorPlane = new Plane( point.Rotation * Vector3.up, point.Position );
                    _targetingContoller.SetPlane( _anchorPlane );
                }

                ChangeState( GameStates.Start );
            }

        }

        private void UpdateStart()
        {
            _player = _playerFactory.Create();

            if (_arFeatures.IsARSupported) {
                _player.transform.parent = _anchor.transform;
                _player.transform.position = _anchor.transform.position;
                _player.transform.rotation = _anchor.transform.rotation;
            } else {
                _player.transform.position = _spawners.transform.position;
                _player.transform.rotation = _spawners.transform.rotation;
            }

            ChangeState( GameStates.Play );
        }


        private void InitScene()
        {
            if (_arFeatures.IsARSupported) {
                _currentGameState = _gameStateFactory.CreateState( GameStates.Placement );
            } else {
                _currentGameState = _gameStateFactory.CreateState( GameStates.Start );
            }

            _sceneSetup = _sceneSetupFactory.Create();
            _container.BindInstance( _sceneSetup );
            _sceneSetup.Start();
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

        private readonly ARFeatures _arFeatures;

        public GameStatePlacement(ARFeatures arFeatures, GameController gameController)
        {
            _arFeatures = arFeatures;
        }

        public override GameStates State => GameStates.Placement;

        public override void Start()
        {

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
