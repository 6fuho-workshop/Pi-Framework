using PiFramework.KeyValueStore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiFramework.Settings
{
    public sealed class SettingsManager: MonoBehaviour
    {
        public static GameSettings settings { get; internal set; }
     
        public static ISavableKeyValueStore dataStore { get; internal set; }
        internal void LoadSettings()
        {
            var loader = GetComponentInChildren<SettingsLoader>();
            GameSettings settingsObj = loader.settings;
#if UNITY_EDITOR
            //phải duplicate vì ở Editor Play Mode thì ScriptableObject sẽ thay đổi tạm thời
            //trong session nên dễ nhầm lẫn không biết việc modify có được save thật sự hay ko
            settingsObj = GameObject.Instantiate<GameSettings>(settingsObj);
#endif
            settings = settingsObj;
            settings.Initialize();

            foreach (Transform t in transform)
            {
                GameObject.Destroy(t.gameObject);
            }

            var GameObj = new GameObject("Runtime Settings");
            loader = GameObj.AddComponent<SettingsLoader>();
            loader.settings = settingsObj;
            GameObj.transform.parent = transform;
        }

        public static ISettingNode GetNode(string path) {
            return settings.GetSettingNode(path);
        }

        public static ISettingNode GetRootNode()
        {
            return settings.GetSettingNode(String.Empty);
        }

        internal static void Destroy()
        {
            settings = null;
            dataStore = null;
        }
    }
}