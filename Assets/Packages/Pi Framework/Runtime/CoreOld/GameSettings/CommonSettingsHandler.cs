using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Settings
{
    public class CommonSettingsHandler : PiModule
    {
        ISettingNode root;
        void Awake()
        {
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