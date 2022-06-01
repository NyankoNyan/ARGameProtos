using ModestTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UISetup : ITickable
{
    private bool _initiaized;
    private Preset _preset;
    private Settings _settings;

    public UISetup(Preset preset, Settings settings)
    {
        _preset = preset;
        _settings = settings;

        Assert.IsNotNull( preset.FireStick );
    }

    public void Tick()
    {
        if (!_initiaized) {
            _initiaized = true;

            if (_settings.HideSticksOnPC) {
                switch (Application.platform) {
                    case RuntimePlatform.WindowsPlayer:
                    case RuntimePlatform.WindowsEditor:
                    case RuntimePlatform.OSXEditor:
                    case RuntimePlatform.OSXPlayer:
                    case RuntimePlatform.LinuxPlayer:
                    case RuntimePlatform.LinuxEditor:
                        _preset.FireStick.SetActive( false );
                        break;
                }
            }
        }
    }

    [Serializable]
    public class Preset
    {
        public GameObject FireStick;
    }

    [Serializable]
    public class Settings
    {
        public bool HideSticksOnPC;
    }
}
