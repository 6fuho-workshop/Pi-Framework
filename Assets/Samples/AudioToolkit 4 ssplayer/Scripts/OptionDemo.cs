using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Demo.AudioToolkit
{
    public class OptionDemo : MonoBehaviour
    {
        // Start is called before the first frame update
        public void ToggleMute()
        {
            //Settings.options.sound.volume.;
            Pi.Audio.ToggleMute();
        }
    }
}