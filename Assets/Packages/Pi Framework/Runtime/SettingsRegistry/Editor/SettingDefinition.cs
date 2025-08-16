using PF.PiEditor.Settings;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PF.PiEditor.Settings
{
    public class SettingDefinition : ScriptableObject
    {
        [Tooltip("Prefix áp dụng cho tất cả entries trong asset này, ví dụ: options.graphics")]
        [FormerlySerializedAs("basePath")]
        public string PathPrefix = "options";

        [Tooltip("Tên namespace cần import khi codegen (enum/type tuỳ biến). Ví dụ: System; MyGame.Graphics;")]
        [FormerlySerializedAs("usingDirectives")]
        public List<string> UsingNamespaces = new() { "System;"};

        [SerializeReference, Tooltip("Danh sách entries (leaf) sẽ được codegen gộp vào class Settings.")]
        [FormerlySerializedAs("settingEntities")]
        public List<SettingEntry> Entries;

        private void OnValidate()
        {
            PathPrefix = (PathPrefix ?? "").Replace(" ", "").Trim('.');
            foreach (var e in Entries) e?.Validate(PathPrefix);
        }
    }
}