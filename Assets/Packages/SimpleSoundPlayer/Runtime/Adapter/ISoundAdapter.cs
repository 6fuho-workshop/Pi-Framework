﻿using UnityEngine;
using System.Collections;
using System;

namespace PiExtension.SimpleSound
{
    public interface ISoundAdapter
    {
        void SetItemVolume(string audioID, float volume);
        void PlayMusicPlaylist(string playlist);
        AudioSource PlaySound(string audioID, SoundChannel channel, float volume = 1, float delay = 0, float startTime = 0, bool? overrideLoop = null);
        AudioSource PlaySound3DAtVector3(string audioID, SoundChannel channel, Vector3 pos, float volume = 1, float delay = 0, float startTime = 0);
        AudioSource PlaySound3DFollow(string audioID, SoundChannel channel, Transform target, float volume = 1, float delay = 0, float startTime = 0);
        void SetFinishCallback(string audioID, Action callback);
        void PauseAll(float fadeOut);
        void UnpauseAll(float fadeIn);
        void PauseChannel(SoundChannel channel, float fadeOut);
        void UnpauseChannel(SoundChannel channel, float fadeIn);
        void StopChannel(SoundChannel channel, float fadeOut);
        void Stop(string audioID, float fadeOut);
        void StopAllSound(float fadeOut);
        void StopPlaylist();
        
    }

}