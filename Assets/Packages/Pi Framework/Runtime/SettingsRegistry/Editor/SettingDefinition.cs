using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PF.PiEditor.Settings
{
    public class SettingDefinition : ScriptableObject
    {
        //[Tooltip("path prefix áp dụng cho toàn bộ khai báo setting, e.g. options.graphics")]
        [FormerlySerializedAs("basePath")]
        public string PathPrefix = "options";


        //[Tooltip("some types need imports namespaces for code generating")]
        [FormerlySerializedAs("usingDirectives")]
        public List<string> UsingNamespaces = new() { "System;" };


        [FormerlySerializedAs("settingEntities")]
        public List<SettingEntity> Entries;

        private void OnValidate()
        {
            foreach (var entity in Entries)
            {
                entity.Validate();
            }
            PathPrefix = PathPrefix.Replace(" ", "");
        }
    }
}