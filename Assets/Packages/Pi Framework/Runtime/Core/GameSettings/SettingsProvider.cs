using PiFramework.KeyValueStore;
using PiFramework.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Settings
{
    public abstract class SettingsProvider
    {
        public event Action<string> changed;
        protected void OnChanged(string propertyName)
        {
            changed?.Invoke(propertyName);
        }

        protected IKeyValueStore dataStore => SettingsManager.dataStore;
        public abstract void LoadPersistent();
    }
}