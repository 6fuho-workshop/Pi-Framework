using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiEditor.Settings
{
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
}