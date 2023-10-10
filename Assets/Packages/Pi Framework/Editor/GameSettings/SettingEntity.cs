using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiEditor.Settings
{
    [Serializable]
    public class SettingEntity
    {
        [HideInInspector]
        public string fullPath;
        [Tooltip("relative path của Setting, ví dụ options.video.fullscreen")]
        public string path;
        //public string name;
        public string type;
        [Tooltip("Điền code, ví dụ: <b>new Vector3(1,1.5f,2)</b>\nnếu để trống thì sẽ dùng Default value của Type")]
        public string defaultValue;
        [Tooltip("Readonly property sẽ ko có setter, tuy nhiên setting vẫn có thể override bằng deserialize process")]
        public bool readOnly = true;
        public string tooltip;
        [Tooltip("Only support int and float.")]
        public bool addRange;
        public float min;
        public float max;
        public bool persistent;
        [Tooltip("Settings using pattern \"node.entity\" as key by default.")]
        public string customKey;
        public string name
        {
            get
            {
                var idx = path.LastIndexOf(".");
                return path[(idx + 1)..];
            }
        }

        public string nodePath
        {
            get
            {
                var idx = path.LastIndexOf(".");
                return idx < 0 ? string.Empty : path[..idx];
            }
        }

        public bool Validate()
        {
            if (min > max)
                min = max;
            if (string.IsNullOrEmpty(path)) return false;
            if (string.IsNullOrEmpty(type)) return false;
            return true;
        }
    }
}