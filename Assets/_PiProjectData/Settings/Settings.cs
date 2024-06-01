using PiFramework.Settings;
using System;

public partial class Settings : RuntimeSettings
{
    static Settings _instance;

    public override event Action<string> changed;

    public static event Action<string> settingChanged
    {
        add { _instance.changed += value; }
        remove { _instance.changed -= value; }
    }

    public override void Initialize()
    {
        Pi.systemEvents.AppQuitPhase1.Register(() => _instance = null);
        _instance = SettingLoaderer.settings as Settings;
        BuildNodeDict();
        _nodeDict.Add(string.Empty, this); //add root node
    }

    protected void OnChanged(string propertyName)
    {
        changed?.Invoke(propertyName);
    }
}