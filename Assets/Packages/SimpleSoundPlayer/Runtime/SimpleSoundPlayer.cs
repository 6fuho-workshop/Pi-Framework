using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using PiFramework;
using PiFramework.Math;
using System;
using PiFramework.Settings;

namespace PiExtension.SimpleSound
{
    public enum SoundChannel { SFX = 0, Music = 1, Ambience = 2, Voice = 3 }

    public class SimpleSoundPlayer : PiModule
    {
        ISoundAdapter _adapter;

        AudioMixer _audioMixer;
        float _unmuteFadeLength = 1;
        float _pauseFadeLength = 1;
        float _stopFadeLength = 1;

        MixerGroup _masterMixer;
        MixerGroup _sfxMixer;
        MixerGroup _musicMixer;
        MixerGroup _ambienceMixer;
        MixerGroup _voiceMixer;
        MixerGroup[] _mixers;
        VolumeSettings _volSettings;

        #region settings

        protected override void Initialize()
        {
            var sspSettings = SettingsManager.GetNode("simpleSoundPlayer");
            _audioMixer = sspSettings.GetValue<AudioMixer>("audioMixer");
            _unmuteFadeLength = sspSettings.GetValue<float>("unmuteFadeLength");
            _pauseFadeLength = sspSettings.GetValue<float>("pauseFadeLength");
            _stopFadeLength = sspSettings.GetValue<float>("stopFadeLength");

            _volSettings = SettingsManager.GetValue<VolumeSettings>("options.sound.volume");
            _volSettings.changed += SettingsChangedHandler;
            _enableSound = _volSettings.enableSound;
        }

        private void Awake()
        {
            _adapter = GetComponent<ISoundAdapter>();
            _masterMixer = new MixerGroup(this, "Master");
            _sfxMixer = new MixerGroup(this, "SFX");
            _musicMixer = new MixerGroup(this, "Music");
            _ambienceMixer = new MixerGroup(this, "Ambience");
            _voiceMixer = new MixerGroup(this, "Voice");
            _mixers = new MixerGroup[] { _masterMixer, _sfxMixer, _musicMixer, _ambienceMixer, _voiceMixer };
        }

        void SettingsChangedHandler(string property)
        {
            switch (property)
            {
                case "enableSound":
                    enableSound = _volSettings.enableSound;
                    break;
                case "mute":
                    mute = _volSettings.mute;
                    break;
                case "sfxMute":
                    sfxMute = _volSettings.sfxMute;
                    break;
                case "musicMute":
                    musicMute = _volSettings.musicMute;
                    break;
                case "ambienceMute":
                    ambienceMute = _volSettings.ambienceMute;
                    break;
                case "voiceMute":
                    voiceMute = _volSettings.voiceMute;
                    break;
                case "masterVolume":
                    masterVolume = _volSettings.masterVolume;
                    break;
                case "sfxVolume":
                    sfxVolume = _volSettings.sfxVolume;
                    break;
                case "musicVolume":
                    musicVolume = _volSettings.musicVolume;
                    break;
                case "ambienceVolume":
                    ambienceVolume = _volSettings.ambienceVolume;
                    break;
                case "voiceVolume":
                    voiceVolume = _volSettings.voiceVolume;
                    break;
            }
        }

        void ReadSettings()
        {
            enableSound = _volSettings.enableSound;
            mute = _volSettings.mute;
            sfxMute = _volSettings.sfxMute;
            musicMute = _volSettings.musicMute;
            ambienceMute = _volSettings.ambienceMute;
            voiceMute = _volSettings.voiceMute;
            masterVolume = _volSettings.masterVolume;
            sfxVolume = _volSettings.sfxVolume;
            musicVolume = _volSettings.musicVolume;
            ambienceVolume = _volSettings.ambienceVolume;
            voiceVolume = _volSettings.voiceVolume;
        }

        private void Start()
        {
            foreach (var mixerGroup in _mixers) mixerGroup.ApplyVolume();
        }

        #endregion settings

        #region playlist

        public void PlayMusicPlaylist(string playlist)
        {
            _adapter.PlayMusicPlaylist(playlist);
        }

        public void StopPlaylist()
        {
            _adapter.StopPlaylist();
        }

        #endregion playlist

        #region playing control

        public void PlayMusic(string audioID, bool? overrideLoop = null)
        {
            _adapter.PlaySound(audioID, SoundChannel.Music, 1, 0, 0, overrideLoop);
        }
        public void PlayMusic(string audioID, PlaybackParams playback)
        {
            _adapter.PlaySound(audioID, SoundChannel.Music, playback.volume, playback.delay, playback.startTime);
        }

        public void PlaySFX(string group, float volume = 1)
        {
            _adapter.PlaySound(group, SoundChannel.SFX, volume);
        }
        public void PlaySFX(string group, PlaybackParams playback)
        {
            _adapter.PlaySound(group, SoundChannel.SFX, playback.volume, playback.delay, playback.startTime, playback.overrideLoop);
        }
        public void PlayAtVector3(string audioID, SoundChannel channel, Vector3 pos, float volume = 1, float delay = 0, float startTime = 0)
        {
            throw new System.NotImplementedException();
        }
        public void PlayFollow(string audioID, SoundChannel channel, Transform target, float volume = 1, float delay = 0, float startTime = 0)
        {
            throw new System.NotImplementedException();
        }
        public void PlaySFXScheduled(string audioID, float delay, float volume = 1)
        {
            _adapter.PlaySound(audioID, SoundChannel.SFX, volume, delay);
        }

        public void SetFinishCallback(string audioID, Action callback)
        {
            _adapter.SetFinishCallback(audioID, callback);
        }

        #endregion playing control

        #region stop

        public void Stop(string audioID, float fadeOut = -1)
        {
            _adapter.Stop(audioID, fadeOut);
        }

        /// <summary>
        /// Nếu fadeOut bé hơn 0 thì sẽ dùng config StopFadeLength
        /// </summary>
        /// <param name="fadeOut">fade out length</param>
        public void StopMusic(float fadeOut = -1)
        {
            _adapter.StopChannel(SoundChannel.Music, fadeOut < 0 ? _stopFadeLength : fadeOut);
        }

        /// <summary>
        /// Nếu fadeOut bé hơn 0 thì sẽ dùng config StopFadeLength
        /// </summary>
        /// <param name="fadeOut">fade out length</param>
        public void StopAmbience(float fadeOut = -1)
        {
            _adapter.StopChannel(SoundChannel.Ambience, fadeOut < 0 ? _stopFadeLength : fadeOut);
        }

        /// <summary>
        /// Nếu fadeOut bé hơn 0 thì sẽ dùng config StopFadeLength
        /// </summary>
        /// <param name="fadeOut">fade out length</param>
        public void StopVoice(float fadeOut = -1)
        {
            _adapter.StopChannel(SoundChannel.Voice, fadeOut < 0 ? _stopFadeLength : fadeOut);
        }

        /// <summary>
        /// Stops all sound
        /// Nếu fadeOut bé hơn 0 thì sẽ dùng config StopFadeLength
        /// </summary>
        /// <param name="fadeOut">fade out length</param>
        public void StopAll(float fadeOut = -1)
        {
            _adapter.StopAllSound(fadeOut < 0 ? _stopFadeLength : fadeOut);
        }

        #endregion stop

        #region pause

        /// <summary>
        /// Nếu fadeOut bé hơn 0 thì sẽ dùng config StopFadeLength
        /// </summary>
        /// <param name="fadeOut">fade out length</param>
        public void PauseAll(float fadeOut = -1)
        {
            _adapter.PauseAll(fadeOut < 0 ? _pauseFadeLength : fadeOut);
        }
        /// <summary>
        /// Nếu fadeIn bé hơn 0 thì sẽ dùng config PauseFadeLength
        /// </summary>
        /// <param name="fadeIn">fade in length</param>
        public void UnpauseAll(float fadeIn = -1)
        {
            _adapter.UnpauseAll(fadeIn < 0 ? _pauseFadeLength : fadeIn);
        }
        /// <summary>
        /// Nếu fadeOut bé hơn 0 thì sẽ dùng config PauseFadeLength
        /// </summary>
        /// <param name="fadeOut">fade out length</param>
        public void PauseMusic(float fadeOut = -1)
        {
            _adapter.PauseChannel(SoundChannel.Music, fadeOut < 0 ? _pauseFadeLength : fadeOut);
        }
        /// <summary>
        /// Nếu fadeIn bé hơn 0 thì sẽ dùng config PauseFadeLength
        /// </summary>
        /// <param name="fadeIn">fade in length</param>
        public void UnpauseMusic(float fadeIn = -1)
        {
            _adapter.UnpauseChannel(SoundChannel.Music, fadeIn < 0 ? _pauseFadeLength : fadeIn);
        }

        #endregion pause

        #region mute & volume

        bool _enableSound;
        /// <summary>
        /// Dùng cho một số tình huống có disbale Sound trong UI
        /// </summary>
        public bool enableSound
        {
            get => _enableSound;
            set
            {
                if (_enableSound == value)
                    return;
                _enableSound = value;
                _masterMixer.fadingVolume = _enableSound ? 1 : 0;
                _masterMixer.ApplyVolume();
                //_volSettings.enableSound = value;
            }
        }

        public void ToggleSFX() { sfxMute = !sfxMute; }

        public void ToggleMusic() { musicMute = !musicMute; }

        public void ToggleMute() { mute = !mute; }

        public void ToggleAmbience() { ambienceMute = !ambienceMute; }

        public void ToggleVoice() { voiceMute = !voiceMute; }

        public bool mute
        {
            get => _masterMixer.mute;
            set
            {
                _masterMixer.mute = value;
                //_volSettings.mute = value;
            }
        }
        public bool sfxMute
        {
            get => _sfxMixer.mute;
            set
            {
                _sfxMixer.mute = value;
                //_volSettings.sfxMute = value;
            }
        }
        public bool musicMute
        {
            get => _musicMixer.mute;
            set
            {
                _musicMixer.mute = value;
                //_volSettings.musicMute = value;
            }
        }
        public bool ambienceMute
        {
            get => _ambienceMixer.mute;
            set
            {
                _ambienceMixer.mute = value;
                //_volSettings.ambienceMute = value;
            }
        }

        public bool voiceMute
        {
            get => _voiceMixer.mute;
            set
            {
                _voiceMixer.mute = value;
                //_volSettings.voiceMute = value;
            }
        }

        public float masterVolume
        {
            get => _masterMixer.volume;
            set
            {
                _masterMixer.volume = value;
                //_volSettings.masterVolume = value;
            }
        }
        public float sfxVolume
        {
            get => _sfxMixer.volume;
            set
            {
                _sfxMixer.volume = value;
                //_volSettings.sfxVolume = value;
            }
        }
        public float musicVolume
        {
            get => _musicMixer.volume;
            set
            {
                _musicMixer.volume = value;
                //_volSettings.musicVolume = value;
            }
        }
        public float ambienceVolume
        {
            get => _ambienceMixer.volume;
            set
            {
                _ambienceMixer.volume = value;
                //_volSettings.ambienceVolume = value;
            }

        }
        public float voiceVolume
        {
            get => _voiceMixer.volume;
            set
            {
                _voiceMixer.volume = value;
                //_volSettings.voiceVolume = value;
            }
        }

        #endregion mute & volume

        #region fading

        public void FadeMusicToVolume(float volume, float fadeLength)
        {
            _musicMixer.Fade(volume, fadeLength);
        }

        public void FadeAmbienceToVolume(float volume, float fadeLength)
        {
            _ambienceMixer.Fade(volume, fadeLength);
        }

        #endregion fading

        class MixerGroup
        {
            SimpleSoundPlayer _player;
            string _groupName;
            public float unmuteFadingVol = 1;
            public float fadingVolume = 1;

            Coroutine _muteRoutine;
            Coroutine _fadeRoutine;

            bool _mute;
            public bool mute
            {
                get => _mute;
                set
                {
                    if (_mute == value) return;
                    _mute = value;

                    if (_mute)
                    {
                        if (_muteRoutine != null)
                        {
                            _player.StopCoroutine(_muteRoutine);
                            _muteRoutine = null;
                        }
                        unmuteFadingVol = 0;
                    }
                    else
                    {
                        _muteRoutine = _player.StartCoroutine(Unmute(_player._unmuteFadeLength));
                    }
                    ApplyVolume();
                }
            }
            float _volume = 1;
            public float volume
            {
                get => _volume;
                set
                {
                    var vol = Mathf.Clamp01(value);
                    if (_volume == vol) return;
                    _volume = vol;
                    ApplyVolume();
                }
            }
            public MixerGroup(SimpleSoundPlayer player, string groupName)
            {
                _player = player;
                _groupName = groupName;
            }

            public void ApplyVolume()
            {
                float vol = _mute ? 0 : _volume * unmuteFadingVol * fadingVolume;
                _player._audioMixer.SetFloat(_groupName, Mathf.Log10(Mathf.Clamp(vol, 0.0001f, 1)) * 20);
            }

            public void Fade(float finalVol, float fadeLen)
            {
                finalVol = Mathf.Clamp01(finalVol);
                if (_fadeRoutine != null)
                {
                    _player.StopCoroutine(_fadeRoutine);
                }
                _fadeRoutine = _player.StartCoroutine(FadingVolume(finalVol, fadeLen));
            }
            IEnumerator FadingVolume(float finalVol, float fadeLen)
            {
                float t = 0;
                var b = fadingVolume;//starting value
                var c = finalVol - b;//change
                while (t < fadeLen)
                {
                    t += Time.deltaTime;
                    fadingVolume = Easing.Linear(t, b, c, fadeLen);
                    ApplyVolume();
                    yield return null;
                }
                fadingVolume = finalVol;
                ApplyVolume();
                _fadeRoutine = null;
            }

            IEnumerator Unmute(float fadeIn)
            {
                float t = 0;
                while (t < fadeIn)
                {
                    t += Time.deltaTime;
                    unmuteFadingVol = Easing.Linear(t, 0, 1, fadeIn);
                    ApplyVolume();
                    yield return null;
                }
                unmuteFadingVol = 1;
                ApplyVolume();
                _muteRoutine = null;
            }
        }

    }
}