using ModestTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Games.ScaleTest
{
    public class ScaleSlider : MonoBehaviour
    {
        private Slider _slider;
        private SignalBus _signalBus;
        private Settings _settings;

        [Inject]
        void Construct(SignalBus signalBus, Settings settings)
        {
            _slider = GetComponent<Slider>();
            Assert.IsNotNull( _slider );
            _signalBus = signalBus;
            _settings = settings;

        }

        private void OnValueChanged(float value)
        {
            _signalBus.Fire( new ScaleChangedSignal() { Value = value } );
        }

        private void Start()
        {
            _slider.minValue = _settings.Min;
            _slider.maxValue = _settings.Max;
            _slider.value = _settings.Default;
            _slider.onValueChanged.AddListener( OnValueChanged );
        }

        private void OnDestroy()
        {
            _slider.onValueChanged.RemoveListener( OnValueChanged );
        }

        [Serializable]
        public class Settings
        {
            public float Default = 1f;
            public float Min = .1f;
            public float Max = 2f;
        }
    }
}