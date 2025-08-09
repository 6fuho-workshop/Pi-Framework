using PF.KeyValueStore;
using PF.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Settings
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
                _dataStore ??= RuntimeSettingsManager.defaultDataStore;
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