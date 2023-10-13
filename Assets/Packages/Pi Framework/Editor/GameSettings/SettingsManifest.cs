using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

namespace PiEditor.Settings
{
    public class SettingsManifest : ScriptableObject
    {
        [Tooltip("path prefix áp dụng cho toàn bộ khai báo setting, e.g. options.graphics")]
        public string basePath = "options";
        [Tooltip("some types need imports namespaces for code generating")]
        public List<string> usingDirectives = new() { "System;"};
        //public List<Branch> branches;
        public List<SettingEntity> settingEntities;

        private void OnValidate()
        {
            foreach (var entity in settingEntities)
            {
                entity.Validate();
            }
            basePath = basePath.Replace(" ", "");
        }
    }
}