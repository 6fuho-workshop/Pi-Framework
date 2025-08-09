using PF;
using PF.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiExtension.SimpleSound
{
    [Serializable]
    public class VolumeSettings : SettingsProvider
    {
        [SerializeField]
        bool _enableSound = true;
        [SerializeField]
        [Range(0, 1)]
        float _masterVolume = 1;
        [SerializeField]
        [Range(0, 1)]
        float _musicVolume = 1;
        [SerializeField]
        [Range(0, 1)]
        float _sfxVolume = 1;
        [SerializeField]
        [Range(0, 1)]
        float _ambienceVolume = 1;
        [SerializeField]
        [Range(0, 1)]
        float _voiceVolume = 1;
        [SerializeField]
        bool _mute;
        [SerializeField]
        bool _musicMute;
        [SerializeField]
        bool _sfxMute;
        [SerializeField]
        bool _voiceMute;
        [SerializeField]
        bool _ambienceMute;

        public override void OnLoadCallback()
        {
            _enableSound = dataStore.GetBool("vol.enableSound", _enableSound);
            _mute = dataStore.GetBool("vol.mute", _mute);
            _musicMute = dataStore.GetBool("vol.musicMute", _musicMute);
            _sfxMute = dataStore.GetBool("vol.sfxMute", _sfxMute);
            _voiceMute = dataStore.GetBool("vol.voiceMute", _voiceMute);
            _ambienceMute = dataStore.GetBool("vol.ambienceMute", _ambienceMute);
            _masterVolume = dataStore.GetFloat("vol.masterVolume", _masterVolume);
            _musicVolume = dataStore.GetFloat("vol.musicVolume", _musicVolume);
            _sfxVolume = dataStore.GetFloat("vol.sfxVolume", _sfxVolume);
            _ambienceVolume = dataStore.GetFloat("vol.ambienceVolume", _ambienceVolume);
            _voiceVolume = dataStore.GetFloat("vol.voiceVolume", _voiceVolume);
        }

        public bool enableSound
        {
            get => _enableSound;
            set
            {
                if (_enableSound == value) return;
                _enableSound = value;
                OnChanged("enableSound");
                dataStore.SetBool("vol.enableSound", value);
            }
        }

        public float masterVolume
        {
            get { return _masterVolume; }
            set
            {
                if (_masterVolume == value) return;
                _masterVolume = value;
                OnChanged("masterVolume");
                dataStore.SetFloat("vol.masterVolume", value);
            }
        }

        public float musicVolume
        {
            get => _musicVolume;
            set
            {
                if (_musicVolume == value) return;
                _musicVolume = value;
                OnChanged("musicVolume");
                dataStore.SetFloat("vol.musicVolume", value);
            }
        }

        public float sfxVolume
        {
            get => _sfxVolume;
            set
            {
                if (_sfxVolume == value) return;
                _sfxVolume = value;
                OnChanged("sfxVolume");
                dataStore.SetFloat("vol.sfxVolume", value);
            }
        }

        public float ambienceVolume
        {
            get => _ambienceVolume;
            set
            {
                if (_ambienceVolume == value) return;
                _ambienceVolume = value;
                OnChanged("ambienceVolume");
                dataStore.SetFloat("vol.ambienceVolume", value);
            }
        }

        public float voiceVolume
        {
            get => _voiceVolume;
            set
            {
                if (_voiceVolume == value) return;
                _voiceVolume = value;
                OnChanged("voiceVolume");
                dataStore.SetFloat("vol.voiceVolume", value);
            }
        }

        public bool mute
        {
            get => _mute;
            set
            {
                if (_mute == value) return;
                _mute = value;
                OnChanged("mute");
                dataStore.SetBool("vol.mute", value);
            }
        }

        public bool musicMute
        {
            get => _musicMute;
            set
            {
                if (_musicMute == value) return;
                _musicMute = value;
                OnChanged("musicMute");
                dataStore.SetBool("vol.musicMute", value);
            }
        }

        public bool sfxMute
        {
            get => _sfxMute;
            set
            {
                if (_sfxMute == value) return;
                _sfxMute = value;
                OnChanged("sfxMute");
                dataStore.SetBool("vol.sfxMute", value);
            }
        }

        public bool ambienceMute
        {
            get => _ambienceMute;
            set
            {
                if (_ambienceMute == value) return;
                _ambienceMute = value;
                OnChanged("ambienceMute");
                dataStore.SetBool("vol.ambienceMute", value);
            }
        }

        public bool voiceMute
        {
            get => _voiceMute;
            set
            {
                if (_voiceMute == value) return;
                _voiceMute = value;
                OnChanged("voiceMute");
                dataStore.SetBool("vol.voiceMute", value);
            }
        }
    }
}