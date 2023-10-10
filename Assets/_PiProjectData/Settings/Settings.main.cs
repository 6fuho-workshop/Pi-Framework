using System;
using UnityEngine;
using PiFramework;
using PiFramework.Settings;
using System.Collections.Generic;
using UnityEngine.Audio;
using PiExtension.SimpleSound;
public partial class Settings : GameSettings, ILoadable
{
    [SerializeField]
    private bool _global1 = false;
    [SerializeField]
    private float _global2;
    [SerializeField]
    private devSettings _dev;
    [SerializeField]
    private simpleSoundPlayerSettings _simpleSoundPlayer;
    [SerializeField]
    private optionsSettings _options;

    public static bool global1 => _instance._global1;

    public static float global2
    {
        get { return _instance._global2; }
        set { if(_instance._global2 == value) return; _instance._global2 = value; _instance.OnChanged("global2"); storage.SetFloat(".global2", value); }
    }

    public static devSettings dev => _instance._dev;

    public static simpleSoundPlayerSettings simpleSoundPlayer => _instance._simpleSoundPlayer;

    public static optionsSettings options => _instance._options;

    public void LoadPersistent()
    {
        global2 = storage.GetFloat(".global2", _global2);
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
    public class devSettings : SettingNode
    {
        [SerializeField]
        private bool _logPiMessages = false;

        public bool logPiMessages => _logPiMessages;

    }

    [Serializable]
    public class simpleSoundPlayerSettings : SettingNode
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
    public class optionsSettings : SettingNode
    {
        [SerializeField]
        private soundSettings _sound;

        public soundSettings sound => _sound;


        [Serializable]
        public class soundSettings : SettingNode
        {
            [SerializeField]
            private VolumeSettings _volume;

            public VolumeSettings volume => _volume;

        }
    }
}
