using PiFramework.KeyValueStore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Settings
{
    public interface IPersistentSetting
    {
        void OnLoadCallback();
        ISavableKeyValueStore dataStore { get; set; }
    }
}