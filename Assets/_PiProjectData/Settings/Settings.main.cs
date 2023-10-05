using System;
using UnityEngine;
public partial class Settings : ScriptableObject
{
    [SerializeField]
    private devSettings _dev;
    [SerializeField]
    private optionsSettings _options;

    public static devSettings dev
    {
        get { return _instance._dev; }
    }

    public static optionsSettings options
    {
        get { return _instance._options; }
    }


    [Serializable]
    public class devSettings
    {
        [SerializeField]
        private bool _logPiMessages = false;

        public bool logPiMessages
        {
            get { return _logPiMessages; }
        }

    }

    [Serializable]
    public class optionsSettings
    {
        [SerializeField]
        private audioSettings _audio;

        public audioSettings audio
        {
            get { return _audio; }
        }


        [Serializable]
        public class audioSettings
        {
            public event Action changed;
            [SerializeField]
            [Range(0, 1)]
            private float _sfxVolume;

            public float sfxVolume
            {
                get { return _sfxVolume; }
                set { if(_sfxVolume == value) return; _sfxVolume = value; changed?.Invoke(); }
            }

        }
    }
}
