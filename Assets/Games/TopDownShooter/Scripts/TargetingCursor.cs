using System;
using UnityEngine;
using Zenject;
using MyBox;

namespace Games.TDS
{
    public class TargetingCursor : ITickable
    {
        private readonly TargetingContoller _targetingContoller;
        private readonly Settings _settings;
        private GameObject _cursor;

        public bool Active { get; set; }

        public TargetingCursor(
            TargetingContoller targetingContoller,
            Settings settings
            )
        {
            _targetingContoller = targetingContoller;
            _settings = settings;
        }

        public void Tick()
        {
            if (!_cursor) {
                _cursor = GameObject.Instantiate( _settings.CursorPrefab );
                _cursor.SetActive( false );
            }

            if (Active) {
                CastPoint castPoint = _targetingContoller.CastScreenRay( Vector2.one * 0.5f, true );
                if (castPoint != null) {
                    _cursor.SetActive( true );
                    _cursor.transform.position = castPoint.Position;
                    _cursor.transform.rotation = castPoint.Rotation;
                } else {
                    _cursor.SetActive( false );
                }
            }
        }

        [Serializable]
        public class Settings
        {
            [MustBeAssigned]  public GameObject CursorPrefab;
        }
    }
}