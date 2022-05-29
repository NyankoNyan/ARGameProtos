using System.Collections;
using UnityEngine.XR.ARFoundation;
using Zenject;

namespace Games
{
    public class ARFeatures : IInitializable
    {
        bool _isReady;
        bool _checkSupportStarted;
        readonly AsyncProcessor _asyncProcessor;
        bool _isARSupported;

        public bool IsReady => _isReady;
        public bool IsARSupported => _isARSupported;

        public ARFeatures(AsyncProcessor asyncProcessor)
        {
            _asyncProcessor = asyncProcessor;
        }

        private IEnumerator CheckDeviceSupport()
        {
            if (ARSession.state == ARSessionState.None
                || ARSession.state == ARSessionState.CheckingAvailability) {
                yield return ARSession.CheckAvailability();
            }

            _isARSupported = ARSession.state != ARSessionState.Unsupported;

            _isReady = true;
        }

        public void Initialize()
        {
            _asyncProcessor.StartCoroutine( CheckDeviceSupport() );
        }

        class Factory : PlaceholderFactory<ARFeatures>
        {
        }
    }
}