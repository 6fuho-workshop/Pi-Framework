using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace PF.PiEditor.Settings
{
    [Serializable]
    public class SettingEntry
    {
        [HideInInspector]
        public string FullPath;

        [Tooltip("Relative path from the asset's PathPrefix. Example: audio.volume.sfxVolume")]
        public string RelativePath;

        [Tooltip("Optional display label for UI. If empty, LeafName/RelativePath will be used.")]
        public string DisplayName;

        [Tooltip("Data type: int, float, bool, string, enum, or a fully-qualified type name.")]
        public string ValueType;

        [Tooltip("C# expression for default value. Examples: 0.75f, true, \"en\", new Vector2(1,1.5f). Leave empty to use default(T).")]
        public string DefaultExpression;

        [Tooltip("If enabled, the property has no setter in generated code. Can still be overridden via deserialization.")]
        public bool IsReadOnly = true;

        [Tooltip("Short description for Inspector/Control Panel.")]
        public string Description;

        [Tooltip("Only valid for int/float. Enables Min/Max clamping and range UI.")]
        public bool HasRange;

        [Tooltip("Minimum value for range clamping (only valid for int/float).")]
        public float Min;

        [Tooltip("Maximum value for range clamping (only valid for int/float).")]
        public float Max;

        [Tooltip("If true, value will be persisted using a storage key (either default or overridden)." +
            "\n The value may still be modified as part of deserialization.")]
        public bool Persist;

        [Tooltip("Optional override for the storage key. Default is pf.settings.v1.<FullPath>")]
        public string StorageKeyOverride;

        [Tooltip("Mark this entry as obsolete (kept for backward compatibility).")]
        public bool IsObsolete;

        [Tooltip("Optional message showing why/what to use instead.")]
        public string ObsoleteMessage;

        [Tooltip("Legacy persisted keys to auto-migrate from (older paths or prefixes).")]
        public string[] LegacyKeys;

        // --- Helpers (not serialized) ---

        /// <summary>Last segment of RelativePath. Example: sfxVolume</summary>
        public string LeafName
        {
            get
            {
                var p = RelativePath ?? string.Empty;
                var idx = p.LastIndexOf('.');
                return idx >= 0 ? p[(idx + 1)..] : p;
            }
        }

        /// <summary>Parent node from RelativePath. Example: audio.volume</summary>
        public string ParentNodePath
        {
            get
            {
                var p = RelativePath ?? string.Empty;
                var idx = p.LastIndexOf('.');
                return idx >= 0 ? p[..idx] : string.Empty;
            }
        }

        /// <summary>
        /// Validate & normalize. Also computes FullPath using the provided prefix.
        /// </summary>
        public void Validate(string pathPrefix)
        {
            // Normalize strings
            RelativePath = (RelativePath ?? string.Empty).Replace(" ", string.Empty).Trim('.');
            ValueType = (ValueType ?? string.Empty).Replace(" ", string.Empty);
            DefaultExpression = (DefaultExpression ?? string.Empty);
            Description = (Description ?? string.Empty);
            DisplayName = (DisplayName ?? string.Empty).Trim();
            StorageKeyOverride = (StorageKeyOverride ?? string.Empty).Trim();
            ObsoleteMessage = (ObsoleteMessage ?? string.Empty).Trim();

            // Compute FullPath
            FullPath = string.IsNullOrEmpty(pathPrefix)
                ? RelativePath
                : string.IsNullOrEmpty(RelativePath) ? pathPrefix : $"{pathPrefix}.{RelativePath}";

            // Range rules
            if (Min > Max) (Min, Max) = (Max, Min);

            // Specialize int clamping meta
            if (string.Equals(ValueType, "int", StringComparison.OrdinalIgnoreCase))
            {
                Min = Mathf.Floor(Min);
                Max = Mathf.Floor(Max);
            }

            // Range only for int/float
            if (!string.Equals(ValueType, "int", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(ValueType, "float", StringComparison.OrdinalIgnoreCase))
            {
                HasRange = false;
            }

            // ReadOnly ⇒ never persist
            if (IsReadOnly) Persist = false;

            // Key override valid only if persisting
            if (!Persist) StorageKeyOverride = string.Empty;

            // Legacy keys normalize
            if (LegacyKeys == null) LegacyKeys = Array.Empty<string>();
            for (int i = 0; i < LegacyKeys.Length; i++)
                LegacyKeys[i] = (LegacyKeys[i] ?? string.Empty).Trim();
        }
    }
}
