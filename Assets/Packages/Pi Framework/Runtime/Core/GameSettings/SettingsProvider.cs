using PiFramework.KeyValueStore;
using PiFramework.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Settings
{
    public abstract class SettingsProvider : IPersistentSetting
    {
        public event Action<string> changed;
        protected void OnChanged(string propertyName)
        {
            changed?.Invoke(propertyName);
        }

        ISavableKeyValueStore _dataStore;

        public ISavableKeyValueStore dataStore
        {
            get
            {
                _dataStore ??= SettingsManager.defaultDataStore;
                return _dataStore;
            }
            set
            {
                _dataStore = value;
            }
        }

        public virtual void OnLoadCallback() { }
    }
}