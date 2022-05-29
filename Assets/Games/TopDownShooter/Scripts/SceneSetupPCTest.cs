using ModestTree;
using MyBox;
using System;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class SceneSetupPCTest : SceneSetup
    {
        [Inject] Settings _settings;
        private GameObject _room;
        private Camera _camera;
        private PCTestCamera.Factory _pcTestCamera;

        public SceneSetupPCTest(PCTestCamera.Factory pcTestCamera)
        {
            _pcTestCamera = pcTestCamera;
        }

        public override void Start()
        {
            _room = GameObject.Instantiate( _settings.Room );
            _camera = Camera.main;

            _pcTestCamera.Create( _camera );

            Assert.IsNotNull( _camera );
        }

        public override Camera Camera => _camera;

        public override CastPoint CastScreenRay(Vector2 screenPos, bool normalized)
        {
            Ray castRay;
            if (normalized) {
                castRay = _camera.ScreenPointToRay( screenPos );
            } else {
                castRay = _camera.ScreenPointToRay( new Vector2( screenPos.x / Screen.width, screenPos.y / Screen.width ) );
            }
            RaycastHit[] hits = Physics.RaycastAll( new Ray( _camera.transform.position, _camera.transform.forward ) );
            foreach (RaycastHit hit in hits) {
                if (hit.transform.tag == _settings.FloorTag) {
                    return new CastPoint() {
                        Position = hit.point,
                        Rotation = Quaternion.FromToRotation( Vector3.up, hit.normal )
                    };
                }
            }
            return null;
        }

        [Serializable]
        public class Settings
        {
            [MustBeAssigned] public GameObject Room;
            [Tag] public string FloorTag = "Floor";
        }
        public class Factory : PlaceholderFactory<SceneSetupPCTest> { }
    }

}
