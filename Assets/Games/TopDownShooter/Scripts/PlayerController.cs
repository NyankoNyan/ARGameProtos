﻿using MyBox;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Games.TDS
{
    public class PlayerController : ITickable
    {
        Vector3 _moveDirection;
        private CastPoint _castPoint;
        private readonly DiContainer _container;
        private readonly Settings _settings;
        private readonly LevelSpawners _spawners;
        private readonly Player.Factory _playerFactory;
        private readonly TargetingContoller _targetingController;
        private Player _player;

        public MoveTypes MoveType => _settings.MoveType;
        public CastPoint CastPoint { get => _castPoint; }
        public Vector2 MoveDirection { get => _moveDirection; }
        public Player Player { get => _player; set => _player = value; }

        public PlayerController(
            DiContainer container,
            Settings settings,
            LevelSpawners spawners,
            Player.Factory playerFactory,
            TargetingContoller targetingContoller)
        {
            _container = container;
            _settings = settings;
            _spawners = spawners;
            _playerFactory = playerFactory;
            _targetingController = targetingContoller;

            _settings.CharacterMoveInput.Enable();
            _settings.FireInput.Enable();
        }

        public void Tick()
        {
            if (!_player) {
                return;
            }

            MovePlayer();

            SceneSetup sceneSetup = _container.TryResolve<SceneSetup>();
            if (sceneSetup != null) {
                Vector2 fireDirection = _settings.FireInput.ReadValue<Vector2>();
                if (fireDirection != Vector2.zero) {
                    Vector3 fireNormalized = Normalize2DInputFromView( sceneSetup.Camera, fireDirection );
                    if (fireDirection.magnitude >= _settings.FireSensitivity) {
                        _player.FireTo( fireNormalized );
                    } else {
                        _player.WatchTo( fireNormalized );
                    }
                }
            }


            _player.Tick();
        }

        private void MovePlayer()
        {
            _moveDirection = Vector3.zero;
            _castPoint = null;

            SceneSetup sceneSetup = _container.TryResolve<SceneSetup>();

            if (sceneSetup != null) {
                if (_settings.MoveType == MoveTypes.Direction) {

                    Vector2 inputDirection = _settings.CharacterMoveInput.ReadValue<Vector2>();
                    _moveDirection = Normalize2DInputFromView( sceneSetup.Camera, inputDirection );

                    if (_moveDirection != Vector3.zero) {
                        _player.MoveDirection( _moveDirection );
                    }

                } else if (_settings.MoveType == MoveTypes.Target) {

                    _castPoint = _targetingController.CastScreenRay( new Vector2( .5f, .5f ), true );

                    if (_castPoint != null) {
                        _player.MovePosition( _castPoint.Position );
                    }

                }
            }
        }

        private Vector3 Normalize2DInputFromView(Camera camera, Vector2 inputDirection)
        {
            Vector3 cameraViewOnFloor = Vector3.ProjectOnPlane( camera.transform.forward, _player.Up );
            Quaternion extRotation = Quaternion.LookRotation( cameraViewOnFloor.normalized, _player.Up );

            return extRotation * new Vector3( inputDirection.x, 0, inputDirection.y );
        }

        [Serializable]
        public class Settings
        {
            public MoveTypes MoveType;

            [Header( "Direction/CharacterMoveInput" )]
            public InputAction CharacterMoveInput;
            [Header( "Target/CameraMoveInput" )]
            public InputAction CameraMoveInput;
            public float MoveSpeed = 1f;
            [Header( "Target/CameraRotateInput" )]
            public InputAction CameraRotateInput;
            public float RotationSensibility = 1;
            [Header( "Fire" )]
            public InputAction FireInput;
            public float FireSensitivity = 0.5f;
        }

    }
}
