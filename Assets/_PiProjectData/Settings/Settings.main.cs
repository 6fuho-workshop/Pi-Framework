using System;
using UnityEngine;
public partial class Settings : ScriptableObject
{
    [SerializeField]
    private bool _runInBackground;
    [SerializeField]
    private devSettings _dev;
    [SerializeField]
    private optionsSettings _options;

    public static bool runInBackground
    {
        get { return _instance._runInBackground; }
        set { if(_instance._runInBackground == value) return; _instance._runInBackground = value; changed?.Invoke(); }
    }

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
        private loggerSettings _logger;

        public loggerSettings logger
        {
            get { return _logger; }
        }


        [Serializable]
        public class loggerSettings
        {
            public event Action changed;
            [SerializeField]
            private bool _framework;

            public bool framework
            {
                get { return _framework; }
                set { if(_framework == value) return; _framework = value; changed?.Invoke(); }
            }

        }
    }

    [Serializable]
    public class optionsSettings
    {
        public event Action changed;
        [SerializeField]
        private bool _cheatingEnable;
        [SerializeField]
        private audioSettings _audio;

        public bool cheatingEnable
        {
            get { return _cheatingEnable; }
            set { if(_cheatingEnable == value) return; _cheatingEnable = value; changed?.Invoke(); }
        }

        public audioSettings audio
        {
            get { return _audio; }
        }


        [Serializable]
        public class audioSettings
        {
            [SerializeField]
            private float _sfxVomue = 0.5f;

            public float sfxVomue
            {
                get { return _sfxVomue; }
            }

        }
    }
}
