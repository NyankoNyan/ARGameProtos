using System;
using System.Collections;
using UnityEngine;
using Zenject;
using MyBox;

namespace Games
{
    public class MainMenuInstaller : MonoInstaller
    {
        [SerializeField, MustBeAssigned] ButtonList ButtonList;
        public override void InstallBindings()
        {
            Container.Bind<MainMenu>().AsSingle().NonLazy();
            Container.Bind<ButtonList>().FromInstance( ButtonList ).AsSingle();
            Container
                .BindFactory<string, string, Action<string>, Button, Button.Factory>()
                .FromComponentInNewPrefab( ButtonList.ButtonPrefab )
                .UnderTransform( ButtonList.ButtonRoot );
        }
    }
}