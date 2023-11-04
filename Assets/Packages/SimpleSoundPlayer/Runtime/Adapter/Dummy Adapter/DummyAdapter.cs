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
            
        }

        public void PauseChannel(SoundChannel channel, float fadeOut)
        {
            
        }

        public void PlayMusicPlaylist(string playlist)
        {
            
        }

        public void PlaySound(string audioID, SoundChannel channel, float volume = 1, float delay = 0, float startTime = 0, bool? overrideLoop = null)
        {
            
        }

        public void PlaySound3DAtVector3(string audioID, SoundChannel channel, Vector3 pos, float volume = 1, float delay = 0, float startTime = 0)
        {
            
        }

        public void PlaySound3DFollow(string audioID, SoundChannel channel, Transform target, float volume = 1, float delay = 0, float startTime = 0)
        {
            
        }

        public void SetFinishCallback(string audioID, Action callback)
        {
            
        }

        public void Stop(string audioID, float fadeOut)
        {
            
        }

        public void StopAllSound(float fadeOut)
        {
            
        }

        public void StopChannel(SoundChannel channel, float fadeOut)
        {
            
        }

        public void StopPlaylist()
        {
            
        }

        public void UnpauseAll(float fadeIn)
        {
            
        }

        public void UnpauseChannel(SoundChannel channel, float fadeIn)
        {
            
        }
    }
}