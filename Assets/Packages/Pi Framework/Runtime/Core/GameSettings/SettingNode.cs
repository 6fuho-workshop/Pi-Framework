using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace PiFramework.Settings
{
    public interface ISettingNode
    {
        event Action<string> changed;
        internal Dictionary<string, PropertyInfo> properties {  get; }
    }

    public static class ISettingNodeExtensions
    {
        public static T GetValue<T>(this ISettingNode node, string name)
        {
            return (T)node.GetProperty(name).GetValue(node);
        }

        public static void SetValue<T>(this ISettingNode node, string name, T value)
        {
            node.GetProperty(name).SetValue(node, value);
        }

        internal static void  GetPropertiesIfEmpty(this ISettingNode node)
        {
            if (node.properties.Count == 0)
            {
                var props = node.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (!prop.PropertyType.IsSubclassOf(typeof(SettingNode)))
                        node.properties.Add(prop.Name, prop);
                }
            }
        }

        internal static PropertyInfo GetProperty(this ISettingNode node, string name)
        {
            node.GetPropertiesIfEmpty();

            if (!node.properties.TryGetValue(name, out var result))
            {
                Debug.LogError($"setting name {name} does not exist");
            }
            return result;
        }

        internal static void LoadSettingsProviders(this ISettingNode node)
        {
            node.GetPropertiesIfEmpty();

            foreach (var prop in node.properties.Values)
            {
                if (prop.PropertyType.IsSubclassOf(typeof(SettingsProvider)))
                {
                    (prop.GetValue(node) as SettingsProvider).LoadPersistent();
                }
            }
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

        protected void OnChanged(string propertyName)
        {
            changed?.Invoke(propertyName);
        }
    }
}