using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiEditor.Settings
{
    [Serializable]
    public class SettingItem
    {
        [Tooltip("relative path của Setting, ví dụ options.video.fullscreen")]
        public string path;
        //public string name;
        public string type;
        [Tooltip("ví dụ: 'new Vector3(1,1.5f,2)', nếu để trống thì sẽ dùng Default value của Type")]
        public string defaultValue;
        public bool readOnly;
        public string tooltip;
        public float rangeFrom;
        public float rangeTo;
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
            if (rangeFrom > rangeTo)
                rangeFrom = rangeTo;
            if (string.IsNullOrEmpty(path)) return false;
            if (string.IsNullOrEmpty(type)) return false;
            return true;
        }
    }
}