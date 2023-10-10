using PiFramework.KeyValueStore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

namespace PiFramework.Settings
{
    public interface ILoadable
    {
        void LoadPersistent();
    }

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
        public abstract ISettingNode GetSettingNode(string path);
        public abstract void Initialize();
        protected static ISavableKeyValueStore storage => SettingsManager.dataStore;
        public void Save() => storage.Save();

        internal void LoadAllPersistents()
        {
            foreach (var node in _nodeDict.Values)
            {
                if(node is ILoadable loader)
                    loader.LoadPersistent();
                node.LoadSettingsProviders();
            }
        }

        /// <summary>
        /// Tạo method virtual để khi xóa file generated vẫn không báo lỗi Undefined Method BuildNodeDict
        /// </summary>
        virtual protected void BuildNodeDict() { }
    }
}