using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using PiFramework.KeyValueStore;

namespace PiFramework.Settings
{
    public interface ISettingNode
    {
        event Action<string> changed;
        internal Dictionary<string, PropertyInfo> properties {  get; }

        internal void GetPropertiesIfEmpty()
        {
            if (properties.Count == 0)
            {
                var props = GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (!prop.PropertyType.IsSubclassOf(typeof(SettingNode)))
                        properties.Add(prop.Name, prop);
                }
            }
        }

        internal void LoadSettingsProviders()
        {
            this.GetPropertiesIfEmpty();

            foreach (var prop in properties.Values)
            {
                if (prop.PropertyType.IsSubclassOf(typeof(SettingsProvider)))
                {
                    (prop.GetValue(this) as SettingsProvider).OnLoadCallback();
                }
            }
        }

        internal PropertyInfo GetProperty(string name)
        {
            GetPropertiesIfEmpty();

            if (!properties.TryGetValue(name, out var result))
            {
                Debug.LogError($"setting name {name} does not exist");
            }
            return result;
        }

        public T GetValue<T>(string name)
        {
            return (T)GetProperty(name).GetValue(this);
        }

        public  void SetValue<T>(string name, T value)
        {
            GetProperty(name).SetValue(this, value);
        }
    }

    public class SettingNode : ISettingNode
    {
        /// <summary>
        /// The event occurs when a node field changed value.
        /// Event arg is property name of setting changed
        /// </summary>
        public event Action<string> changed;
        Dictionary<string, PropertyInfo> _properties = new();
        Dictionary<string, PropertyInfo> ISettingNode.properties => _properties;
        public ISavableKeyValueStore dataStore { get; set; }
        protected void OnChanged(string propertyName)
        {
            changed?.Invoke(propertyName);
        }
    }
}