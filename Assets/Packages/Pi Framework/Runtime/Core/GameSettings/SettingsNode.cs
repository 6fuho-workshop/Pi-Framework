using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace PiFramework.Settings
{
    public class SettingsNode
    {
        /// <summary>
        /// The event occurs when a node field changed value
        /// </summary>
        public event Action changed;

        void OnValueChange()
        {
            if (changed != null)
                changed.Invoke();
        }
    }
}