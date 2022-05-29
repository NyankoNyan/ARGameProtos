using MyBox;
using System;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    [RequireComponent( typeof( Rigidbody ) )]
    public class Player : MonoBehaviour
    {
        [SerializeField, MustBeAssigned] Transform BulletFirePoint;

        Settings _settings;
        private BulletSpawnService _bulletSpawnService;
        private Rigidbody _rigidBody;
        Vector3 _speed;

        bool _onMove;
        private bool _onFire;
        private Vector3 _rotateTo;
        private bool _onWatch;
        private float _fireDelay;

        public Vector3 Up => transform.up;

        [Inject]
        public void Construct(
            Settings settings,
            BulletSpawnService bulletSpawnService
            )
        {
            _settings = settings;
            _bulletSpawnService = bulletSpawnService;

            _rigidBody = GetComponent<Rigidbody>();
        }

        public void Tick()
        {
            if (!_onMove) {
                BreakSpeed();
            }

            // Moving
            transform.position += _speed * Time.deltaTime;

            // Rotation
            if (_onWatch) {

                Vector3 rotatePlanar = Vector3.ProjectOnPlane( _rotateTo, transform.up );
                Quaternion newRotation = Quaternion.LookRotation( rotatePlanar, Up );
                transform.rotation = Quaternion.Lerp( newRotation, transform.rotation, _settings.RotationSmooth );

            } else {

                Vector3 rotatePlanar = Vector3.ProjectOnPlane( _speed, transform.up );

                if (rotatePlanar != Vector3.zero) {
                    Quaternion newRotation = Quaternion.LookRotation( rotatePlanar, Up );
                    transform.rotation = Quaternion.Lerp( newRotation, transform.rotation, _settings.RotationSmooth );
                }

            }

            // Shooting
            if (_onFire && _fireDelay <= 0) {
                _bulletSpawnService.Spawn( gameObject, BulletFirePoint.position, BulletFirePoint.rotation );
                _fireDelay = _settings.FireDelay;
            }

            _fireDelay -= Time.deltaTime;

            _onMove = false;
            _onFire = false;
            _onWatch = false;
        }

        public void MoveDirection(Vector3 direction)
        {
            _onMove = true;

            _speed += direction.normalized * _settings.Acceleration * Time.deltaTime;
            if (_speed.magnitude > _settings.MaxSpeed) {
                _speed = _speed.normalized * _settings.MaxSpeed;
            }
        }

        public void MovePosition(Vector3 point)
        {
            _onMove = true;

            Vector3 distanceTo = point - transform.position;
            float remainDistance = distanceTo.magnitude;

            if (remainDistance > _settings.MovePositionIgnoreDistance) {

                float stopTime = _speed.magnitude / _settings.Acceleration;
                float stopDistance = _settings.Acceleration * stopTime * stopTime / 2f;
                if (remainDistance <= stopDistance) {
                    BreakSpeed();
                } else {
                    MoveDirection( distanceTo );
                }
            } else {
                _speed = Vector3.zero;
            }
        }

        public void WatchTo(Vector3 direction)
        {
            _rotateTo = direction;
            _onWatch = true;
        }

        public void FireTo(Vector3 direction)
        {
            WatchTo( direction );
            _onFire = true;
        }

        private void BreakSpeed()
        {
            float accDelta = _settings.Acceleration * Time.deltaTime;
            if (_speed.magnitude < accDelta) {
                _speed = Vector3.zero;
            } else {
                _speed = _speed.normalized * ( _speed.magnitude - accDelta );
            }
        }

        public class Factory : PlaceholderFactory<Player> { }

        [Serializable]
        public class Settings
        {
            public float MaxSpeed = 10;
            public float Acceleration = 2;
            public float MovePositionIgnoreDistance = 1f;
            public float RotationSmooth = .5f;

            public float FireDelay = .1f;
        }
    }
}
