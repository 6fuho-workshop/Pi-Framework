using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PF.PiEditor.Settings
{
    [Serializable]
    public class TopLevelCategory
    {
        [Tooltip("First-level node under PathPrefix. Example: 'audio', 'graphics'")]
        public string Node;
        [Tooltip("Display label for UI. If empty, Node is used.")]
        public string DisplayName;
        [Tooltip("Optional tooltip/description for UI.")]
        public string Tooltip;
        [Tooltip("Ordering in UI; lower comes first.")]
        public int Order = 0;

        public void Normalize()
        {
            Node = (Node ?? "").Trim().Trim('.');
            DisplayName = (DisplayName ?? "").Trim();
            Tooltip = (Tooltip ?? "").Trim();
        }
    }

    [CreateAssetMenu(menuName = "Pi/Setting Definition", fileName = "custom_settings.def.asset")]
    public class SettingDefinition : ScriptableObject
    {
        [Tooltip("Prefix applied to all entries in this asset. Example: options.graphics")]
        public string PathPrefix = "options";

        [Tooltip("Owner/module for traceability (e.g., AudioModule, Core, MyGame.Graphics).")]
        public string Owner = "";

        [Tooltip("Namespaces to include in generated code. Example: \"System\", \"MyGame.Graphics\"")]
        public List<string> UsingNamespaces = new() { "System" };

        [Tooltip("List of leaf entries to be merged into the generated Settings API.")]
        public List<SettingEntry> Entries = new();

        [Tooltip("Optional category mapping for first-level nodes under PathPrefix. Only affects UI (not codegen).")]
        public List<TopLevelCategory> TopLevelCategories = new();

        private void OnValidate()
        {
            // Normalize PathPrefix
            PathPrefix = (PathPrefix ?? string.Empty).Replace(" ", string.Empty).Trim('.');

            // Normalize Owner
            Owner = (Owner ?? "").Trim();

            // Normalize namespaces (trim, remove trailing ';', distinct, drop empties)
            UsingNamespaces ??= new List<string>();
            for (int i = 0; i < UsingNamespaces.Count; i++)
            {
                if (UsingNamespaces[i] == null) { UsingNamespaces[i] = string.Empty; continue; }
                UsingNamespaces[i] = UsingNamespaces[i].Replace(";", string.Empty).Trim();
            }
            UsingNamespaces.RemoveAll(s => string.IsNullOrEmpty(s));
            UsingNamespaces = new List<string>(new HashSet<string>(UsingNamespaces));

            // Normalize categories
            TopLevelCategories ??= new List<TopLevelCategory>();
            foreach (var cat in TopLevelCategories) cat?.Normalize();

            // Validate entries
            Entries ??= new List<SettingEntry>();
            foreach (var e in Entries)
            {
                if (e == null) continue;
                e.Validate(PathPrefix);
            }
        }
    }
}
