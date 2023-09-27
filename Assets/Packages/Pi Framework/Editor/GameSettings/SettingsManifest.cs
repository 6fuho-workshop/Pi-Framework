using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiEditor.Settings
{
    public class SettingsManifest : ScriptableObject
    {
        [Tooltip("path prefix")]
        public string basePath = "sound";
        public List<string> usingDirectives = new() { "System;"};
        //public List<Branch> branches;
        public List<SettingItem> settingItems;

        /*
        [Serializable]
        public class SettingItem
        {
            public string path;
            public string name;
            public string type;
            public string defaultValue;
            public bool readOnly;
            public string tooltip;
            public float rangeFrom;
            public float rangeTo;
        }
        */
        
        /*
        [Serializable]
        public class Branch
        {
            public string name;
            public List<Branch> branches = null;
            public List<SettingItem> settingItems;
        }
        */
    }
}