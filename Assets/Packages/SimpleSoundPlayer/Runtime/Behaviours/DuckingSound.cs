using PiFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiExtension.SimpleSound
{
    [AddComponentMenu("Simple Sound Player/Ducking Sound")]
    [RequireComponent(typeof(SingleSoundPlayer))]
    public class DuckingSound : MonoBehaviour
    {
        public enum DuckChannel { Music, Ambience, Both}

        public DuckChannel duckType;
        [Range(0,1)]
        public float duckVolume;
        public float fadeOutLength;
        public float fadeInLength;
        string _audioID;
        SimpleSoundPlayer _audio;
        
        private void Awake()
        {
            _audio = PiServiceLocator.instance.GetService<SimpleSoundPlayer>();
            var ssp = GetComponent<SingleSoundPlayer>();
            _audioID = ssp.audioID;
            ssp.onPlaySound += OnStartPlay;

        }

        void OnStartPlay()
        {
            _audio.SetFinishCallback(_audioID, OnFinishPlay);
            if(duckType == DuckChannel.Music || duckType == DuckChannel.Both)
                _audio.FadeMusicToVolume(duckVolume, fadeOutLength);
            if (duckType == DuckChannel.Ambience || duckType == DuckChannel.Both)
                _audio.FadeAmbienceToVolume(duckVolume, fadeOutLength);
        }

        void OnFinishPlay()
        {
            if (duckType == DuckChannel.Music || duckType == DuckChannel.Both)
                _audio.FadeMusicToVolume(1, fadeInLength);
            if (duckType == DuckChannel.Ambience || duckType == DuckChannel.Both)
                _audio.FadeAmbienceToVolume(1, fadeInLength);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}