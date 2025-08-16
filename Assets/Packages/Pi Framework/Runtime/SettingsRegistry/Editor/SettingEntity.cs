using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PF.PiEditor.Settings
{
    [Serializable]
    public class SettingEntity
    {
        [HideInInspector]
        public string fullPath;
        [Tooltip("relative path của Setting, ví dụ options.video.fullscreen")]
        [FormerlySerializedAs("path")]
        public string RelativePath;
        //public string name;
        [FormerlySerializedAs("type")]
        public string ValueType;
        [Tooltip("Điền code, ví dụ: <b>new Vector3(1,1.5f,2)</b>\nnếu để trống thì sẽ dùng Default value của Type")]
        [FormerlySerializedAs("defaultValue")]
        public string DefaultExpression;
        [Tooltip("Readonly property sẽ ko có setter, tuy nhiên setting vẫn có thể override bằng deserialize process")]
        [FormerlySerializedAs("readOnly")]
        public bool IsReadOnly = true;
        [FormerlySerializedAs("tooltip")]
        public string Description;
        [Tooltip("Only support int and float.")]
        [FormerlySerializedAs("useRange")]
        public bool HasRange;
        [FormerlySerializedAs("min")]
        public float Min;
        [FormerlySerializedAs("max")]
        public float Max;
        [FormerlySerializedAs("persistent")]
        public bool Persist;
        [Tooltip("Settings using pattern \"node.entity\" as key by default.")]

        [FormerlySerializedAs("customKey")]
        public string StorageKeyOverride;
        public string LeafName { get; }
        public string ParentNodePath { get; }
        public string name
        {
            get
            {
                var idx = RelativePath.LastIndexOf(".");
                return RelativePath[(idx + 1)..];
            }
        }

        public string nodePath
        {
            get
            {
                var idx = RelativePath.LastIndexOf(".");
                return idx < 0 ? string.Empty : RelativePath[..idx];
            }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(RelativePath)) return false;
            if (string.IsNullOrEmpty(ValueType)) return false;
            return true;
        }

        public void Validate()
        {
            if (Min > Max)
                Min = Max;
            if (ValueType == "int")
            {
                Min = Mathf.Floor(Min);
                Max = Mathf.Floor(Max);
            }

            if (IsReadOnly)
                Persist = false;
            RelativePath = RelativePath.Replace(" ", "");
            ValueType = ValueType.Replace(" ", "");

            if (ValueType != "int" && ValueType != "float")
                HasRange = false;

            if (Persist)
                StorageKeyOverride = StorageKeyOverride.Trim();
        }
    }
}