using UnityEngine;
using Zenject;

namespace Games.TDS
{
    public class TargetingContoller
    {
        private Plane? _plane;
        private readonly Camera _camera;

        public TargetingContoller(Camera camera)
        {
            _camera = camera;
        }

        public void SetPlane(Plane? plane)
        {
            _plane = plane;
        }

        public CastPoint CastScreenRay(Vector2 screenPos, bool normalized)
        {
            if (_plane != null) {
                return CastScreenRayOnPlane( screenPos, normalized );
            } else {
                return CastOnFloor( screenPos, normalized );
            }
        }


        private CastPoint CastOnFloor(Vector2 screenPos, bool normalized)
        {
            Ray castRay;
            if (normalized) {
                castRay = _camera.ScreenPointToRay( screenPos );
            } else {
                castRay = _camera.ScreenPointToRay( new Vector2( screenPos.x / Screen.width, screenPos.y / Screen.width ) );
            }
            RaycastHit[] hits = Physics.RaycastAll( new Ray( _camera.transform.position, _camera.transform.forward ) );
            foreach (RaycastHit hit in hits) {
                if (hit.transform.tag == "Floor") {
                    return new CastPoint() {
                        Position = hit.point,
                        Rotation = Quaternion.FromToRotation( Vector3.up, hit.normal )
                    };
                }
            }
            return null;
        }

        private CastPoint CastScreenRayOnPlane(Vector2 screenPos, bool normalized)
        {
            Ray castRay;

            if (normalized) {
                castRay = _camera.ScreenPointToRay( new Vector2( screenPos.x * Screen.width, screenPos.y * Screen.height ) );
            } else {
                castRay = _camera.ScreenPointToRay( screenPos );
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
