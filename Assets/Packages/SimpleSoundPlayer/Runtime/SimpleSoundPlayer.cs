using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using PiFramework;
using PiFramework.Math;
using System;

namespace PiExtension.SimpleSound
{
    public enum SoundChannel { SFX = 0, Music = 1, Ambience = 2, Voice = 3 }

    [ModuleObjectName("Simple Sound Player")]
    public class SimpleSoundPlayer : PiModule
    {
        const string PrefPrefix = "ssplayer.";
        ISoundAdapter _adapter;
        public event Action settingsChanged;

        [SerializeField]
        AudioMixer _audioMixer;

        [SerializeField]
        float _unmuteFadeLength = 1;

        [SerializeField]
        [Tooltip("Apply for global and channels")]
        float _pauseFadeLength = 1;

        [SerializeField]
        [Tooltip("Apply for global and channels")]
        float _stopFadeLength = 1;

        MixerGroup _masterMixer;
        MixerGroup _sfxMixer;
        MixerGroup _musicMixer;
        MixerGroup _ambienceMixer;
        MixerGroup _voiceMixer;
        MixerGroup[] _mixers;

        [Tooltip("save settings to playerPref when mute/unmute")]
        [SerializeField]
        bool autoSaveOnToggles = true;

        #region settings

        protected override void Initialize()
        {

        }

        private void Awake()
        {
            _adapter = GetComponent<ISoundAdapter>();
            //Pi.serviceLocator.AddService(this, typeof(SimpleSoundPlayer));
            _masterMixer = new MixerGroup(this, "Master", "vol", "mute");
            _sfxMixer = new MixerGroup(this, "SFX", "sVol", "sMute");
            _musicMixer = new MixerGroup(this, "Music", "mVol", "mMute");
            _ambienceMixer = new MixerGroup(this, "Ambience", "aVol", "aMute");
            _voiceMixer = new MixerGroup(this, "Voice", "vVol", "vMute");
            _mixers = new MixerGroup[] { _masterMixer, _sfxMixer, _musicMixer, _ambienceMixer, _voiceMixer };
            LoadSettings();
        }
        void LoadSettings()
        {
            var pref = Pi.playerPref.CreateInstance().SetPrefix(PrefPrefix);
            _audioDisabled = pref.GetBool("audioDisabled", false);
            foreach (var mixer in _mixers) mixer.ReadPref(pref);
            OnSettingsChanged();
        }
        /// <summary>
        /// Write settings vào playerPref rồi save
        /// </summary>
        public void SaveSettings()
        {
            var pref = Pi.playerPref.CreateInstance().SetPrefix(PrefPrefix);
            pref.SetBool("audioDisabled", _audioDisabled);
            foreach (var mixer in _mixers) mixer.WritePref(pref);
            pref.Save();
        }

        void OnSettingsChanged()
        {
            if (settingsChanged != null)
                settingsChanged.Invoke();
        }
        void OnApplicationQuit()
        {
            SaveSettings();
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
            _adapter.PlaySound(audioID, SoundChannel.Music, 1,0,0, overrideLoop);
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

        bool _audioDisabled;
        /// <summary>
        /// Dùng cho một số tình huống có disbale Sound trong UI
        /// </summary>
        public bool audioDisabled
        {
            get => _audioDisabled;
            set
            {
                if (_audioDisabled == value)
                    return;
                _audioDisabled = value;
                if (autoSaveOnToggles)
                    SaveSettings();
                _masterMixer.fadingVolume = _audioDisabled ? 0 : 1;
                _masterMixer.ApplyVolume();
                OnSettingsChanged();
            }
        }

        public void ToggleSFX() { sfxMuted = !sfxMuted; }

        public void ToggleMusic() { musicMuted = !musicMuted; }

        public void ToggleMute() { mute = !mute; }

        public void ToggleAmbience() { ambienceMuted = !ambienceMuted; }

        public void ToggleVoice() { voiceMuted = !voiceMuted; }

        public bool mute
        {
            get => _masterMixer.mute;
            set => _masterMixer.mute = value;
        }
        public bool sfxMuted
        {
            get => _sfxMixer.mute;
            set => _sfxMixer.mute = value;
        }
        public bool musicMuted
        {
            get => _musicMixer.mute;
            set => _musicMixer.mute = value;
        }
        public bool ambienceMuted
        {
            get => _ambienceMixer.mute;
            set => _ambienceMixer.mute = value;
        }

        public bool voiceMuted
        {
            get => _voiceMixer.mute;
            set => _voiceMixer.mute = value;
        }

        public float masterVolume
        {
            get => _masterMixer.volume;
            set => _masterMixer.volume = value;
        }
        public float sfxVolume
        {
            get => _sfxMixer.volume;
            set => _sfxMixer.volume = value;
        }
        public float musicVolume
        {
            get => _musicMixer.volume;
            set => _musicMixer.volume = value;
        }
        public float ambienceVolume
        {
            get => _ambienceMixer.volume;
            set => _ambienceMixer.volume = value;

        }
        public float voiceVolume
        {
            get => _voiceMixer.volume;
            set => _voiceMixer.volume = value;
        }

        #endregion mute & volume

        #region fading

        public void FadeMusicToVolume(float volume,float fadeLength)
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
            string _volKey;
            string _muteKey;

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
                    if (_player.autoSaveOnToggles)
                        _player.SaveSettings();
                    
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
                    _player.OnSettingsChanged();
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
                    _player.OnSettingsChanged();
                }
            }
            public MixerGroup(SimpleSoundPlayer player, string groupName, string prefVolumeKey, string prefMuteKey)
            {
                _player = player;
                this._groupName = groupName;
                this._muteKey = prefMuteKey;
                this._volKey = prefVolumeKey;
            }

            public void ReadPref(PiPlayerPref pref)
            {
                _volume = pref.GetFloat(_volKey, 1);
                _mute = pref.GetBool(_muteKey, false);
            }

            public void WritePref(PiPlayerPref pref)
            {
                pref.SetFloat(_volKey, _volume);
                pref.SetBool(_muteKey, _mute);
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