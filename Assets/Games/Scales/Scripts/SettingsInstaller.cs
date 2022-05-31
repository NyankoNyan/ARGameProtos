using UnityEngine;
using Zenject;

namespace Games.ScaleTest
{
    [CreateAssetMenu( fileName = "SettingsInstaller", menuName = "Installers/ScaleTest/SettingsInstaller" )]
    public class SettingsInstaller : ScriptableObjectInstaller<SettingsInstaller>
    {
        [SerializeField] ScaleSlider.Settings ScaleSliderSettings;
        public override void InstallBindings()
        {
            Container.BindInstance( ScaleSliderSettings ).AsSingle();
        }
    }
}