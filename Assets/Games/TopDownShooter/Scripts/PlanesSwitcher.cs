using UnityEngine.XR.ARFoundation;

namespace Games.TDS
{
    public class PlanesSwitcher
    {
        private readonly ARPlaneManager _planeManager;

        public PlanesSwitcher(ARPlaneManager planeManager)
        {
            _planeManager = planeManager;
        }

        public void SetPlanesActive(bool active)
        {
            _planeManager.enabled = active;
            foreach (var plane in _planeManager.trackables) {
                plane.gameObject.SetActive( active );
            }
        }
    }
}
