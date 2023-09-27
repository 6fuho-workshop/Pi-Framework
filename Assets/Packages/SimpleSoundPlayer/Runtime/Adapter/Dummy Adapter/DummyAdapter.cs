using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiExtension.SimpleSound
{
    public class DummyAdapter : MonoBehaviour, ISoundAdapter
    {
        public void PauseAll(float fadeOut)
        {
            throw new System.NotImplementedException();
        }

        public void PauseChannel(SoundChannel channel, float fadeOut)
        {
            throw new System.NotImplementedException();
        }

        public void PlayMusicPlaylist(string playlist)
        {
            throw new System.NotImplementedException();
        }

        public void PlaySound(string audioID, SoundChannel channel, float volume = 1, float delay = 0, float startTime = 0, bool? overrideLoop = null)
        {
            throw new System.NotImplementedException();
        }

        public void PlaySound3DAtVector3(string audioID, SoundChannel channel, Vector3 pos, float volume = 1, float delay = 0, float startTime = 0)
        {
            throw new System.NotImplementedException();
        }

        public void PlaySound3DFollow(string audioID, SoundChannel channel, Transform target, float volume = 1, float delay = 0, float startTime = 0)
        {
            throw new System.NotImplementedException();
        }

        public void SetFinishCallback(string audioID, Action callback)
        {
            throw new NotImplementedException();
        }

        public void Stop(string audioID, float fadeOut)
        {
            throw new System.NotImplementedException();
        }

        public void StopAllSound(float fadeOut)
        {
            throw new System.NotImplementedException();
        }

        public void StopChannel(SoundChannel channel, float fadeOut)
        {
            throw new System.NotImplementedException();
        }

        public void StopPlaylist()
        {
            throw new NotImplementedException();
        }

        public void UnpauseAll(float fadeIn)
        {
            throw new System.NotImplementedException();
        }

        public void UnpauseChannel(SoundChannel channel, float fadeIn)
        {
            throw new System.NotImplementedException();
        }
    }
}