using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

namespace Games.ScaleTest
{
    public class AnchorCreator : ITickable, IInitializable
    {
        private readonly ARRaycastManager _raycastManager;
        private readonly ARSessionOrigin _arOrigin;
        private readonly GameObject _mainScene;
        private ARAnchor _anchor;

        public AnchorCreator(ARRaycastManager raycastManager, ARSessionOrigin arOrigin, GameObject mainScene)
        {
            _raycastManager = raycastManager;
            _arOrigin = arOrigin;
            _mainScene = mainScene;
        }

        public void Initialize()
        {
            EnhancedTouchSupport.Enable();
            _mainScene.SetActive( false );
        }

        public void Tick()
        {

            if (_anchor) {
                return;
            }

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

            // Raycast against planes and feature points
            const TrackableType trackableTypes =
                //TrackableType.FeaturePoint |
                TrackableType.PlaneWithinPolygon;

            // Perform the raycast
            List<ARRaycastHit> hits = new List<ARRaycastHit>();
            if (_raycastManager.Raycast( touch.screenPosition, hits, trackableTypes )) {
                // Raycast hits are sorted by distance, so the first one will be the closest hit.
                var hit = hits[0];

                //if (_anchor) {
                //    GameObject.Destroy( _anchor );
                //}

                GameObject newGO = new GameObject();
                newGO.transform.position = hit.pose.position;
                newGO.transform.rotation = hit.pose.rotation;
                _anchor = newGO.AddComponent<ARAnchor>();

                _arOrigin.MakeContentAppearAt( _anchor.transform, _anchor.transform.position, _anchor.transform.rotation );

                _mainScene.SetActive( true );
                _mainScene.transform.position = _anchor.transform.position;
                _mainScene.transform.rotation = _anchor.transform.rotation;
            }
        }

    }
}