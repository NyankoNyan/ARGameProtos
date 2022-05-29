using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Zenject;

namespace Games.TDS
{
    public class Anchor:MonoBehaviour
    {
        private ARAnchor _arAnchor;
        private CastPoint _castPoint;

        [Inject]
        public void Construct(CastPoint castPoint)
        {
            _castPoint = castPoint;
        }

        private void Start()
        {
            transform.position = _castPoint.Position;
            transform.rotation = _castPoint.Rotation;

            _arAnchor = GetComponent<ARAnchor>();
            if (!_arAnchor) {
                _arAnchor = gameObject.AddComponent<ARAnchor>();
            }
        }

        public class Factory : PlaceholderFactory<CastPoint, Anchor>{ }
    }
}
