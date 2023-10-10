using PiEditor;
using PiFramework.KeyValueStore;
using PiFramework.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public partial class Settings : GameSettings
{
    static Settings _instance;

    public static event Action<string> settingChanged
    {
        add { _instance.changed += value; }
        remove { _instance.changed -= value; }
    }

    public override event Action<string> changed;

    public static void FromJsonOverwrite(string json)
    {
        JsonUtility.FromJsonOverwrite(json, _instance);
    }

    public override ISettingNode GetSettingNode(string path)
    {
        if (String.IsNullOrEmpty(path))
            return this;
        _nodeDict.TryGetValue(path, out var node);
        return node;
    }

    public override void Initialize()
    {
        Pi.systemEvents.FinalApplicationQuit.AddListener(() => _instance = null);
        _instance = SettingsManager.settings as Settings;
        BuildNodeDict();
        _nodeDict.Add(string.Empty, this); //add root node
    }

    protected void OnChanged(string propertyName)
    {
        changed?.Invoke(propertyName);
    }
}