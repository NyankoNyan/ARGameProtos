using UnityEngine;
using Zenject;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

namespace Games.TDS
{
    public class AnchorCreator
    {
        private readonly ARRaycastManager _raycastManager;
        private readonly ARSessionOrigin _arOrigin;
        private readonly TargetingContoller _targetingContoller;
        private readonly Transform _sceneRoot;
        private ARAnchor _anchor;

        public AnchorCreator(
            ARRaycastManager raycastManager, 
            ARSessionOrigin arOrigin, 
            TargetingContoller targetingContoller,
            Transform sceneRoot
            )
        {
            _raycastManager = raycastManager;
            _arOrigin = arOrigin;
            _targetingContoller = targetingContoller;
            _sceneRoot = sceneRoot;
        }

        public bool TouchPlaceUpdate()
        {
            if (Touch.activeFingers.Count == 0) {
                return false;
            }

            Touch touch = Touch.activeFingers[0].currentTouch;
            if (touch.phase != TouchPhase.Began) {
                return false;
            }

            if (EventSystem.current.IsPointerOverGameObject( touch.touchId )) {
                return false;
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

                if (_anchor) {
                    GameObject.Destroy( _anchor );
                }

                GameObject newGO = new GameObject();
                newGO.transform.position = hit.pose.position;
                newGO.transform.rotation = hit.pose.rotation;
                _anchor = newGO.AddComponent<ARAnchor>();

                _arOrigin.MakeContentAppearAt( _anchor.transform, _anchor.transform.position, _anchor.transform.rotation );

                _sceneRoot.transform.position = hit.pose.position;
                _sceneRoot.transform.rotation = hit.pose.rotation;

                _targetingContoller.SetPlane( new Plane( hit.pose.rotation * Vector3.up, hit.pose.position ) );

                return true;
            } else {
                return false;
            }
        }

    }
}