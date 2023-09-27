using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiExtension.SimpleSound
{
    public class PlaybackParams
    {
        public float volume = 1f;
        public float delay = 0f;
        public float startTime = 0f;
        public bool? overrideLoop = null;
        /// <summary>
        /// variation name of soundGroup or songName of playlist
        /// </summary>
        public string variation;

        public PlaybackParams SetVolume(float volume)
        {
            this.volume = volume;
            return this;
        }

        public PlaybackParams SetDelayTime(float delayTime)
        {
            this.delay = delayTime;
            return this;
        }

        public PlaybackParams SetStartTime(float startTime)
        {
            this.startTime = startTime;
            return this;
        }

        public PlaybackParams SetLoop(bool? overrideLoop)
        {
            this.overrideLoop = overrideLoop;
            return this;
        }
    }
}