using ModestTree;
using MyBox;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Zenject;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Games.TDS
{

    public class SceneSetupAR : SceneSetup, IDisposable
    {

        [Inject] Settings _settings;
        private GameObject _sessionOrigin;
        private ARRaycastManager _raycastManager;
        private ARPlaneManager _planeManager;
        private Camera _camera;
        private readonly GameController _gameController;
        private readonly ARRefs _arRefs;

        public SceneSetupAR(GameController gameController,ARRefs arRefs)
        {
            _gameController = gameController;
            _arRefs = arRefs;
        }

        public override void Start()
        {
            GameObject.Instantiate( _settings.ARSession );
            _sessionOrigin = GameObject.Instantiate( _settings.ARSessionOrigin );
            _camera = Camera.main;
            _arRefs.Origin = _sessionOrigin.GetComponent<ARSessionOrigin>();
            _raycastManager = _sessionOrigin.GetComponent<ARRaycastManager>();
            _planeManager = _sessionOrigin.GetComponent<ARPlaneManager>();
            _gameController.StateChangedEvent += OnStateChanged;

            Assert.IsNotNull( _camera );
        }

        private void OnStateChanged(GameController.StateChangedArgs args)
        {
            switch (args.NewState) {
                case GameStates.Start:
                    SetARPlanesActive( false );
                    break;
                case GameStates.Placement:
                    SetARPlanesActive( true );
                    break;
            }
        }

        private void SetARPlanesActive(bool active)
        {
            if (_planeManager) {
                _planeManager.enabled = active;
                foreach (var plane in _planeManager.trackables) {
                    plane.gameObject.SetActive( active );
                }
            }
        }

        public override Camera Camera => _camera;

        public override CastPoint CastScreenRay(Vector2 screenPos, bool normalized)
        {

            Vector2 normalizedPos;
            if (normalized) {
                normalizedPos = new Vector2( screenPos.x * Screen.width, screenPos.y * Screen.height );
            } else {
                normalizedPos = screenPos;
            }
            // Raycast against planes and feature points
            const TrackableType trackableTypes =
                //TrackableType.FeaturePoint |
                TrackableType.PlaneWithinPolygon;

            // Perform the raycast
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (_raycastManager.Raycast( normalizedPos, hits, trackableTypes )) {
                // Raycast hits are sorted by distance, so the first one will be the closest hit.
                var hit = hits[0];

                return new CastPoint() {
                    Position = hit.pose.position,
                    Rotation = hit.pose.rotation
                };
            }

            return null;
        }

        public void Dispose()
        {
            _gameController.StateChangedEvent -= OnStateChanged;
        }

        [Serializable]
        public class Settings
        {
            [MustBeAssigned] public GameObject ARSession;
            [MustBeAssigned] public GameObject ARSessionOrigin;
        }

        public class Factory : PlaceholderFactory<SceneSetupAR> { }
    }
}
