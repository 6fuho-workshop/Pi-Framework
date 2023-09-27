using PiEditor.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiEditor
{
    [Serializable]
    public class ModuleInfo
    {
        public string name;
        public string displayName;
        public string version;
        public string description;
        public string unity;
        public string author;
        public string documentationUrl;
        public string[] dependencies;
        
        public PinServiceInfo[] pinServices;
        public string[] initComponents;//fullType,unique, must derives from PiModule => auto addservice
        public SettingsManifest[] settingsManifests;

        [Serializable]
        public class PinServiceInfo
        {
            /// <summary>
            /// pin name để tạo property Pi.propertyName
            /// </summary>
            public string name;

            /// <summary>
            /// Type fuleName (include namespace)
            /// </summary>
            public string fullType;
        }

        public class ComponentInfo
        {
            public string fullType;
            public bool autoAddService;
            public bool disabled;
        }
    }
}