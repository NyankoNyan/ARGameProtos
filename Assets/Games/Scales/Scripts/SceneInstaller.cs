using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityEngine.XR.ARFoundation;

namespace Games.ScaleTest
{
    public class SceneInstaller : MonoInstaller
    {
        [SerializeField] Settings _settings;
        public override void InstallBindings()
        {
            SignalBusInstaller.Install( Container );

            Container.DeclareSignal<ScaleChangedSignal>();

            Container.Bind<ScaleChanger>().AsSingle().NonLazy();
            Container.BindInstance( _settings.ScalableObject ).WhenInjectedInto<ScaleChanger>();

            Container.Bind<ScaleSlider>().FromNewComponentOn( _settings.Slider.gameObject ).AsSingle().NonLazy();

            if (_settings.IsAR) {
                Container.BindInterfacesAndSelfTo<AnchorCreator>().AsSingle().NonLazy();
                Container.BindInstance( _settings.Origin ).AsSingle();
                Container.BindInstance( _settings.RaycastManager ).AsSingle();
                Container.BindInstance( _settings.MainScene ).WhenInjectedInto<AnchorCreator>();
            }
        }

        [Serializable]
        public class Settings
        {
            public GameObject ScalableObject;
            public Slider Slider;
            public bool IsAR;
            public ARSessionOrigin Origin;
            public ARRaycastManager RaycastManager;
            public GameObject MainScene;
        }
    }
}