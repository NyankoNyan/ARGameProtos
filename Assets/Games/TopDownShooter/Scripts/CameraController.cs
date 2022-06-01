using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Games.TDS
{
    public class CameraController : ITickable
    {
        private readonly PlayerController.Settings _settings;
        private readonly Camera _camera;

        //ICameraProvider _cameraProvider;
        private bool _init;

        //public ICameraProvider CameraProvider { get => _cameraProvider; set => _cameraProvider = value; }

        public CameraController(PlayerController.Settings settings, Camera camera)
        {
            _settings = settings;
            _camera = camera;
        }

        public void Tick()
        {
            if (!_init) {
                _settings.CameraRotateInput.Enable();
                _settings.CameraMoveInput.Enable();
                InputSystem.Update();
                _init = true;
            }

            if (_settings.MoveType == MoveTypes.Target) {
                Move();
                Rotate();
            }
        }

        void Rotate()
        {

            Vector2 rotateInput = _settings.CameraRotateInput.ReadValue<Vector2>();
            Transform cameraTransform = _camera.transform;

            cameraTransform.Rotate( Vector3.up, rotateInput.x * Time.deltaTime * _settings.RotationSensibility, Space.World );
            cameraTransform.Rotate( cameraTransform.right, rotateInput.y * Time.deltaTime * _settings.RotationSensibility, Space.World );

        }

        private void Move()
        {
            Vector3 moveInput = _settings.CameraMoveInput.ReadValue<Vector3>();
            Transform cameraTransform = _camera.transform;

            cameraTransform.position += cameraTransform.rotation * moveInput * Time.deltaTime * _settings.MoveSpeed;
        }

    }
}
