using System;
using UnityEngine;
public partial class Settings : ScriptableObject
{
    private static Settings _instance;
    public static event Action changed;

    public static void Load(Settings origin) {
        _instance = ScriptableObject.Instantiate<Settings>(origin);
        changed = null;
    }

    public static void FromJsonOverwrite(string json)
    {
        JsonUtility.FromJsonOverwrite(json, _instance);
    }
}