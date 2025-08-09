using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using PF;

namespace PiExtension.SimpleSound
{

    
    public enum PlaybackAction
    {
        None,
        Play,
        Stop
    }

    [AddComponentMenu("Simple Sound Player/Sound Player")]
    public class SingleSoundPlayer : MonoBehaviour
    {
        public enum LoopOption { UseItemSetting, DoNotLoop, LoopSubItem}
        public enum PlayPosition
        {
            Global,
            FollowObject,
            CurrentPosition,
        }
        SimpleSoundPlayer _audio;
        public string audioID;
        public SoundChannel channel = SoundChannel.SFX;
        public PlayPosition position = PlayPosition.Global; // has no meaning for Music

        [Range(0f,1f)]
        public float volume = 1f;
        public float delay = 0;
        public LoopOption loop;
        public event Action onPlaySound;
        /*
        [Tooltip("Behaviour to trigger when the user configuration to allow play audio on specify channel")]
        [SerializeField]
        PlaybackAction onChannelEnabled = PlaybackAction.None;
        */

        [Tooltip("Behaviour to trigger when the object this is attached to is created")]
        [SerializeField]
        PlaybackAction onStart = PlaybackAction.Play;

        [Tooltip("Behaviour to trigger when the object this is attached to is enabled or when the object is created")]
        [SerializeField]
        PlaybackAction onEnable = PlaybackAction.None;

        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed or set to in-active")]
        [SerializeField]
        PlaybackAction onDisable = PlaybackAction.None;

        [SerializeField]
        PlaybackAction onCollisionEnter = PlaybackAction.None;

        [SerializeField]
        PlaybackAction onCollisionExit = PlaybackAction.None;


        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed")]
        [SerializeField]
        PlaybackAction onDestroy = PlaybackAction.None;

        /// <summary>
        /// Boolean prevents the sound from being played multiple times when the Start and OnEnable callbacks intersect
        /// </summary>
        bool activated;

        public void PlaySound()
        {
            PlaybackParams playback = new PlaybackParams();
            playback.SetDelayTime(delay).SetVolume(volume);
            if (loop == LoopOption.UseItemSetting)
                playback.SetLoop(null);
            else
                playback.SetLoop(loop == LoopOption.LoopSubItem);

            if (channel == SoundChannel.SFX)
            {
                _audio.PlaySFX(audioID, playback);
            }
            else if (channel == SoundChannel.Music)
            {
                _audio.PlayMusic(audioID, playback);
            }
            else if(channel == SoundChannel.Ambience)
            {
                //
            }

            if (onPlaySound != null)
                onPlaySound.Invoke();
        }

        private void Awake()
        {
            _audio = PiBase.Services.Resolve<SimpleSoundPlayer>();
        }
        // Start is called before the first frame update
        protected void Start()
        {
            switch (onStart)
            {
                case PlaybackAction.Play:
                    if (!activated)
                    {
                        activated = true;
                        StartCoroutine(PlayOnEnable());
                    }
                    break;
                case PlaybackAction.Stop:
                    Stop();
                    break;
            }
        }

        void CheckAction(PlaybackAction action)
        {
            switch(action)
            {
                case PlaybackAction.None:
                    break;
                case PlaybackAction.Play:
                    PlaySound();
                    break;
                case PlaybackAction.Stop:
                    Stop();
                    break;
            }
        }

        /// <summary>
        /// Stops the sound instantly
        /// </summary>
        public void Stop()
        {
            _audio.Stop(audioID);
        }

        private void OnEnable()
        {
            switch (onEnable)
            {
                case PlaybackAction.Play:
                    if (!activated)
                    {
                        activated = true;
                        StartCoroutine(PlayOnEnable());
                    }
                    break;
                case PlaybackAction.Stop:
                    Stop();
                    break;
            }
        }

        IEnumerator PlayOnEnable()
        {
            yield return null;
            PlaySound();
        }

        void OnCollisionEnter(Collision other)
        {
            CheckAction(onCollisionEnter);
        }

        void OnCollisionExit(Collision other)
        {
            CheckAction(onCollisionExit);
        }

        private void OnDisable()
        {
            CheckAction(onDisable);
        }

        private void OnDestroy()
        {
            CheckAction(onDestroy);
        }
    }
}