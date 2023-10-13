using PiFramework.KeyValueStore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace PiFramework.Settings
{
    public sealed class SettingsManager : MonoBehaviour
    {
        public static GameSettings settings { get; internal set; }

        public static ISavableKeyValueStore defaultDataStore { get; set; }
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
            CreateSettingsContainer();
            settings.Initialize();

            //todo: patch settings go here

            //todo: Config Default and Custom KeyValueStore for Nodes and SettingsProviders
            SettingsManager.defaultDataStore = new KeyValueStore.PiPlayerPref() as ISavableKeyValueStore;
            var nodes = settings.GetNodeDict();
            foreach(var node in nodes)
            {
                if(node.Value is IPersistentSetting persistent)
                {
                    persistent.dataStore = defaultDataStore;
                }
                //todo: set dataStore for SettingsProviders
            }
            
            settings.LoadAllPersistents();
        }

        public static void SaveSettings()
        {
            defaultDataStore.Save();
        }

        void CreateSettingsContainer()
        {
            foreach (Transform t in transform)
                GameObject.Destroy(t.gameObject);
            var GameObj = new GameObject("Runtime Settings");
            var loader = GameObj.AddComponent<SettingsLoader>();
            loader.settings = settings;
            GameObj.transform.parent = transform;
        }

        public static ISettingNode GetNode(string path)
        {
            return settings.GetSettingNode(path);
        }

        public static ISettingNode GetRootNode()
        {
            return settings.GetSettingNode(String.Empty);
        }

        public static T GetValue<T>(string path)
        {
            var idx = path.LastIndexOf('.');
            string nodePath = idx < 0 ? string.Empty : path[..idx];
            string setting = idx < 0 ? path : path[(idx + 1)..];
            return GetNode(nodePath).GetValue<T>(setting);
        }

        public static void SetValue<T>(string path, T value)
        {
            var idx = path.LastIndexOf('.');
            string nodePath = idx < 0 ? string.Empty : path[..idx];
            string setting = idx < 0 ? path : path[(idx + 1)..];
            GetNode(nodePath).SetValue(setting, value);
        }

        internal static void Destroy()
        {
            SaveSettings();
            settings = null;
            defaultDataStore = null;
        }
    }
}