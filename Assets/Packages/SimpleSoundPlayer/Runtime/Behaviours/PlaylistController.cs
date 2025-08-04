using PiFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiExtension.SimpleSound
{
    [AddComponentMenu("Simple Sound Player/Playlist Controller")]
    public class PlaylistController : MonoBehaviour
    {
        
        public string playListName = "Default";
        [Tooltip("Behaviour to trigger when the object this is attached to is created")]
        [SerializeField]
        PlaybackAction onStart = PlaybackAction.Play;

        [Tooltip("Behaviour to trigger when the object this is attached to is enabled or when the object is created")]
        [SerializeField]
        PlaybackAction onEnable = PlaybackAction.None;

        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed or set to in-active")]
        [SerializeField]
        PlaybackAction onDisable = PlaybackAction.None;

        [Tooltip("Behaviour to trigger when the object this is attached to is destroyed")]
        [SerializeField]
        PlaybackAction onDestroy = PlaybackAction.None;
        public void Play()
        {
            PiBase.Services.GetService<SimpleSoundPlayer>().PlayMusicPlaylist(playListName);
        }

        public void Stop()
        {
            PiBase.Services.GetService<SimpleSoundPlayer>().StopPlaylist();
        }

        private void Start()
        {
            CheckAction(onStart);
        }
        private void OnEnable()
        {
            CheckAction(onEnable);
        }

        private void OnDisable()
        {
            CheckAction(onDisable);
        }

        private void OnDestroy()
        {
            CheckAction(onDestroy);
        }

        void CheckAction(PlaybackAction action)
        {
            switch (action)
            {
                case PlaybackAction.None:
                    break;
                case PlaybackAction.Play:
                    Play();
                    break;
                case PlaybackAction.Stop:
                    Stop();
                    break;
            }
        }
    }
}