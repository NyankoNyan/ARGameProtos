using System;
using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class PCTestCamera : ICameraProvider, IDisposable
    {
        private CameraController _cameraController;
        private Camera _camera;

        public PCTestCamera(Camera camera, CameraController cameraController)
        {
            _cameraController = cameraController;
            _camera = camera;

            _cameraController.CameraProvider = this;
        }

        public Camera Camera => _camera;

        public void Dispose()
        {
            _cameraController.CameraProvider = null;
        }

        public class Factory : PlaceholderFactory<Camera, PCTestCamera> { }
    }
}
