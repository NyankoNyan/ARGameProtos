using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class TargetingContoller
    {
        private Plane? _plane;
        private readonly DiContainer _container;

        public TargetingContoller(DiContainer container)
        {
            _container = container;
        }

        public void SetPlane(Plane? plane)
        {
            _plane = plane;
        }

        public CastPoint CastScreenRay(Vector2 screenPos, bool normalized)
        {
            SceneSetup sceneSetup = _container.TryResolve<SceneSetup>();
            if (sceneSetup != null) {
                if (_plane != null) {

                    return CastScreenRayOnPlane( screenPos, normalized, sceneSetup );
                } else {
                    return sceneSetup.CastScreenRay( screenPos, normalized );
                }
            } else {
                return null;
            }
        }

        private CastPoint CastScreenRayOnPlane(Vector2 screenPos, bool normalized, SceneSetup sceneSetup)
        {
            Ray castRay;

            if (normalized) {
                castRay = sceneSetup.Camera.ScreenPointToRay( new Vector2( screenPos.x * Screen.width, screenPos.y * Screen.height ) );
            } else {
                castRay = sceneSetup.Camera.ScreenPointToRay( screenPos );
            }

            if (_plane.Value.Raycast( castRay, out float enter )) {
                return new CastPoint() {
                    Position = castRay.GetPoint( enter ),
                    Rotation = Quaternion.FromToRotation( Vector3.up, _plane.Value.normal )
                };
            } else {
                return null;
            }
        }
    }
}
