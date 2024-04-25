using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ClockStone;

namespace PiExtension.SimpleSound
{
    public class AudioToolkitAdapter : MonoBehaviour, ISoundAdapter
    {
        public void PauseAll(float fadeOut)
        {
            AudioController.PauseAll(fadeOut);
        }

        public void PauseChannel(SoundChannel channel, float fadeOut)
        {
            switch (channel)
            {
                case SoundChannel.Music:
                    AudioController.PauseMusic(fadeOut);
                    break;
                case SoundChannel.Ambience:
                    AudioController.PauseAmbienceSound(fadeOut);
                    break;
                case SoundChannel.Voice:
                    Debug.LogError("PauseChannel Voice not implemented");
                    break;
            }
        }

        public void PlayMusicPlaylist(string playlist)
        {
            AudioController.PlayMusicPlaylist(playlist);
        }

        public void SetItemVolume(string audioID, float volume)
        {
            var item = AudioController.GetAudioItem(audioID);
            if (item != null)
            {
                item.Volume = volume;
            }
        }

        public AudioSource PlaySound(string audioID, SoundChannel channel, float volume = 1, float delay = 0, float startTime = 0, bool? overrideLoop = null)
        {
            AudioObject obj = null;
            switch (channel)
            {
                case SoundChannel.SFX:
                    obj = AudioController.Play(audioID, volume, delay, startTime);
                    break;
                case SoundChannel.Music:
                    obj = AudioController.PlayMusic(audioID, volume, delay, startTime);
                    break;
                case SoundChannel.Ambience:
                    obj = AudioController.PlayAmbienceSound(audioID, volume, delay, startTime);
                    break;
                case SoundChannel.Voice:
                    Debug.LogError("Play Voice not implemented");
                    break;
            }
            if (obj != null)
            {
                if (overrideLoop.HasValue)
                    obj.audioItem.Loop = overrideLoop.Value ? AudioItem.LoopMode.LoopSubitem : AudioItem.LoopMode.DoNotLoop;
                return obj.primaryAudioSource;
            }
            return null;
        }

        public AudioSource PlaySound3DAtVector3(string audioID, SoundChannel channel, Vector3 pos, float volume = 1, float delay = 0, float startTime = 0)
        {
            throw new System.NotImplementedException();
        }

        public AudioSource PlaySound3DFollow(string audioID, SoundChannel channel, Transform target, float volume = 1, float delay = 0, float startTime = 0)
        {
            AudioObject obj = null;
            switch (channel)
            {
                case SoundChannel.SFX:
                    obj = AudioController.Play(audioID, target, volume, delay, startTime);
                    break;
                case SoundChannel.Music:
                    obj = AudioController.PlayMusic(audioID, target, volume, delay, startTime);
                    break;
                case SoundChannel.Ambience:
                    obj = AudioController.PlayAmbienceSound(audioID, target, volume, delay, startTime);
                    break;
                case SoundChannel.Voice:
                    Debug.LogError("Play Voice not implemented");
                    break;
            }

            if (obj != null)
                return obj.primaryAudioSource;

            return null;
        }

        public void SetFinishCallback(string audioID, Action callback)
        {
            var objs = AudioController.GetPlayingAudioObjects(audioID, true);
            foreach (var obj in objs)
            {
                obj.completelyPlayedDelegate += (audioObject) => { callback.Invoke(); };
            }
        }

        public void Stop(string audioID, float fadeOut)
        {
            AudioController.Stop(audioID, fadeOut);
        }

        public void StopAllSound(float fadeOut)
        {
            AudioController.StopAll(fadeOut);
        }

        public void StopChannel(SoundChannel channel, float fadeOut)
        {
            switch (channel)
            {
                case SoundChannel.Music:
                    AudioController.StopMusic(fadeOut);
                    break;
                case SoundChannel.Ambience:
                    AudioController.StopAmbienceSound(fadeOut);
                    break;
                case SoundChannel.Voice:
                    Debug.LogError("StopChannel Voice not implemented");
                    break;
            }
        }

        public void UnpauseAll(float fadeIn)
        {
            AudioController.UnpauseAll(fadeIn);
        }

        public void UnpauseChannel(SoundChannel channel, float fadeIn)
        {
            switch (channel)
            {
                case SoundChannel.Music:
                    AudioController.UnpauseMusic(fadeIn);
                    break;
                case SoundChannel.Ambience:
                    AudioController.UnpauseAmbienceSound(fadeIn);
                    break;
                case SoundChannel.Voice:
                    Debug.LogError("UnpauseChannel Voice not implemented");
                    break;
            }
        }

        public void StopPlaylist()
        {
            if (AudioController.IsPlaylistPlaying())
                AudioController.StopMusic(AudioController.Instance.musicCrossFadeTime_Out);
        }
    }
}
