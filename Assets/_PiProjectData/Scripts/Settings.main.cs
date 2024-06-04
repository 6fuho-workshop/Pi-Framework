using System;
using UnityEngine;
using PiFramework;
using PiFramework.Settings;
using System.Collections.Generic;
using UnityEngine.Audio;
using PiExtension.SimpleSound;
public partial class Settings : RuntimeSettings, IPersistentSetting
{
    [SerializeField]
    private bool _global1 = false;
    [SerializeField]
    private float _global2;
    [SerializeField]
    private float _global23;
    [SerializeField]
    [Tooltip("Should the Player be running when the application is in the background?")]
    private bool _runInBackground = false;
    [SerializeField]
    [Tooltip("The default value -1. In the default case, Unity uses the platform's default target frame rate. \nNote that platform and device capabilities affect the frame rate at runtime, so your game might not achieve the target frame rate.")]
    private int _targetFrameRate = -1;
    [SerializeField]
    private DevSettings _dev;
    [SerializeField]
    private SimpleSoundPlayerSettings _simpleSoundPlayer;
    [SerializeField]
    private OptionsSettings _options;

    public static bool global1 => _instance._global1;

    public static float global2
    {
        get { return _instance._global2; }
        set { if(_instance._global2 == value) return; _instance._global2 = value; _instance.OnChanged("global2"); _instance.dataStore.SetFloat(".global2", value); }
    }

    public static float global23
    {
        get { return _instance._global23; }
        set { if(_instance._global23 == value) return; _instance._global23 = value; _instance.OnChanged("global23"); _instance.dataStore.SetFloat(".global23", value); }
    }

    public static bool runInBackground => _instance._runInBackground;

    public static int targetFrameRate => _instance._targetFrameRate;

    public static DevSettings dev => _instance._dev;

    public static SimpleSoundPlayerSettings simpleSoundPlayer => _instance._simpleSoundPlayer;

    public static OptionsSettings options => _instance._options;

    public void OnLoadCallback()
    {
        global2 = dataStore.GetFloat(".global2", _global2);
        global23 = dataStore.GetFloat(".global23", _global23);
    }

    protected override void BuildNodeDict()
    {
        _nodeDict = new Dictionary<string, ISettingNode>() {
            {"dev", dev},
            {"simpleSoundPlayer", simpleSoundPlayer},
            {"options", options},
            {"options.sound", options.sound},
        };
    }

    [Serializable]
    public class DevSettings : SettingNode
    {
        [SerializeField]
        private bool _logPiMessages = false;

        public bool logPiMessages => _logPiMessages;

    }

    [Serializable]
    public class SimpleSoundPlayerSettings : SettingNode
    {
        [SerializeField]
        private AudioMixer _audioMixer;
        [SerializeField]
        private float _unmuteFadeLength = 1;
        [SerializeField]
        [Tooltip("Apply for global and channels")]
        private float _pauseFadeLength = 1;
        [SerializeField]
        [Tooltip("Apply for global and channels")]
        private float _stopFadeLength = 1;

        public AudioMixer audioMixer => _audioMixer;

        public float unmuteFadeLength => _unmuteFadeLength;

        public float pauseFadeLength => _pauseFadeLength;

        public float stopFadeLength => _stopFadeLength;

    }

    [Serializable]
    public class OptionsSettings : SettingNode
    {
        [SerializeField]
        private SoundSettings _sound;

        public SoundSettings sound => _sound;


        [Serializable]
        public class SoundSettings : SettingNode
        {
            [SerializeField]
            private VolumeSettings _volume;

            public VolumeSettings volume => _volume;

        }
    }
}
