using PF.KeyValueStore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Settings
{
    public interface IPersistentSetting
    {
        void OnLoadCallback();
        ISavableKeyValueStore dataStore { get; set; }
    }
}