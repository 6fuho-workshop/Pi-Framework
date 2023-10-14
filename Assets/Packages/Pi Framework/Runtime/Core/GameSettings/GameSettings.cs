using PiFramework.KeyValueStore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace PiFramework.Settings
{
    /// <summary>
    /// Cần thiết phải có abstract class GameSettings ở PiFramework Assembly 
    /// thay vì dùng Interface để có thể gán Settings Asset vào SettingsLoader.
    /// </summary>
    public abstract class GameSettings : ScriptableObject, ISettingNode
    {

        #region ISettingNode

        protected Dictionary<string, PropertyInfo> _properties = new();
        Dictionary<string, PropertyInfo> ISettingNode.properties => _properties;
        public abstract event Action<string> changed;

        #endregion ISettingNode

        protected Dictionary<string, ISettingNode> _nodeDict;
        //public abstract ISettingNode GetSettingNode(string path);
        public abstract void Initialize();

        
        public ISavableKeyValueStore dataStore { get; set; }
        public void Save() => SettingsManager.SaveSettings();

        internal void LoadAllPersistents()
        {
            foreach (var node in _nodeDict.Values)
            {
                if (node is IPersistentSetting loader)
                    loader.OnLoadCallback();
                node.LoadSettingsProviders();
            }
        }

        public ISettingNode GetSettingNode(string path)
        {
            if (String.IsNullOrEmpty(path))
                return this;
            _nodeDict.TryGetValue(path, out var node);
            if (node == null)
                Debug.LogError($"Node {name} does not exist");
            return node;
        }

        internal Dictionary<string, ISettingNode> GetNodeDict() => _nodeDict; 
        /// <summary>
        /// Tạo method virtual để khi xóa file generated vẫn không báo lỗi Undefined Method BuildNodeDict
        /// </summary>
        virtual protected void BuildNodeDict() { }
    }
}