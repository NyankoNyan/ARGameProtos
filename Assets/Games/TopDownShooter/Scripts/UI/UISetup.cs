using ModestTree;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UISetup : ITickable
{
    private bool _initiaized;
    private Context _context;
    private Settings _settings;

    public UISetup(Context context, Settings settings)
    {
        _context = context;
        _settings = settings;

        Assert.IsNotNull( context.FireStick );
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
                        _context.FireStick.SetActive( false );
                        break;
                }
            }
        }
    }

    [Serializable]
    public class Context
    {
        public GameObject FireStick;
    }

    [Serializable]
    public class Settings
    {
        public bool HideSticksOnPC;
    }
}
