using PF.Logging;
using UnityEngine;

namespace PF.Core.Settings
{
    public class CommonSettingsHandler : PiModule
    {
        ISettingNode root;
        void Awake()
        {
            Log.Core.Trace("CommonSettingsHandler Awake");
            root = RuntimeSettingsManager.GetRootNode();
            ApplyFrameRate();
            RunInBackground();
        }

        void ApplyFrameRate()
        {
            Application.targetFrameRate = root.GetValue<int>("targetFrameRate");
        }

        void RunInBackground()
        {
            Application.runInBackground = root.GetValue<bool>("runInBackground");
        }
    }
}