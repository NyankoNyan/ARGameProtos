using UnityEngine;
using Zenject;

namespace Games.ScaleTest
{
    public class ScaleChanger
    {
        private readonly GameObject _scalableObject;
        private readonly SignalBus _signalBus;

        public ScaleChanger(GameObject scalableObject, SignalBus signalBus)
        {
            _scalableObject = scalableObject;
            _signalBus = signalBus;
            _signalBus.Subscribe<ScaleChangedSignal>( ChangeScale );
        }
        public void ChangeScale(ScaleChangedSignal signal)
        {
            _scalableObject.transform.localScale = Vector3.one * signal.Value;
        }
    }
}