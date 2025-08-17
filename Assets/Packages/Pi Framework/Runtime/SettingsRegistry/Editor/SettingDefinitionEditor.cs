// Assets/PF/Editor/Settings/SettingDefinitionEditor.cs
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.Linq;

namespace PF.PiEditor.Settings
{
    [CustomEditor(typeof(SettingDefinition))]
    public class SettingDefinitionEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var so = serializedObject;

            // --- Header Toolbar ---
            var bar = new Toolbar();
            var btnValidate = new ToolbarButton(() => ValidateCurrent()) { text = "Validate" };
            var btnApply = new ToolbarButton(() =>
            {
                SettingsGenerator.Generate(); // file #2
                AssetDatabase.SaveAssetIfDirty(target);
                EditorUtility.DisplayDialog("Settings", "Generated manifest & code successfully.", "OK");
            })
            { text = "Apply (Generate)" };

            var btnReindex = new ToolbarButton(() =>
            {
#if UNITY_EDITOR
                //PF.PiEditor.SettingsRegistry.SettingsRegistry.RebuildIndex(); // file #3
                EditorUtility.DisplayDialog("Settings", "Registry index rebuilt.", "OK");
#endif
            })
            { text = "Rebuild Index" };

            var btnCP = new ToolbarButton(() =>
            {
                PF.PiEditor.ControlPanel.ControlPanelWindow.Open(); // mở Control Panel
            })
            { text = "Open Control Panel" };

            bar.Add(btnValidate);
            bar.Add(new ToolbarSpacer());
            bar.Add(btnApply);
            bar.Add(new ToolbarSpacer());
            bar.Add(btnReindex);
            bar.Add(new ToolbarSpacer());
            bar.Add(btnCP);
            root.Add(bar);

            // --- Default inspector (giữ để tận dụng PropertyDrawer của SettingEntry) ---
            var body = new VisualElement();
            InspectorElement.FillDefaultInspector(body, so, this);
            root.Add(body);

            // Hint nhỏ
            var hint = new HelpBox("• Validate để kiểm tra FullPath trùng/không hợp lệ.\n• Apply để sinh manifest/code AOT-safe cho runtime.\n• Open Control Panel › Presets để quản lý nhiều asset.", HelpBoxMessageType.Info);
            root.Add(hint);

            return root;
        }

        void ValidateCurrent()
        {
            var def = (SettingDefinition)target;
            // Kiểm tra Owner + PathPrefix
            if (string.IsNullOrWhiteSpace(def.Owner))
                Debug.LogWarning($"[Settings] '{def.name}' missing Owner. (Ignore if intentional)");

            if (string.IsNullOrWhiteSpace(def.PathPrefix))
                Debug.LogWarning($"[Settings] '{def.name}' missing PathPrefix. (Ignore if intentional)");

            def.ValidateEntries();

            // addition check: FullPath duplicate
            var seen = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            foreach (var e in def.Entries)
            {
                var rel = e.RelativePath?.Trim();
                var full = string.IsNullOrWhiteSpace(def.PathPrefix) ? rel : $"{def.PathPrefix}.{rel}";
                if (!seen.Add(full))
                    Debug.LogWarning($"[Settings] Duplicate FullPath: {full}");
            }

            EditorUtility.DisplayDialog("Validate", "Validation finished. Check Console for warnings.", "OK");
        }
    }

    // ---- Drawer cho SettingEntry: foldout + logic động + preview FullPath/Key ----
    [CustomPropertyDrawer(typeof(SettingEntry))]
    public class SettingEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var def = property.serializedObject.targetObject as SettingDefinition; // để lấy PathPrefix/Owner

            var foldout = new Foldout { value = false };

            // Container bên trong
            var container = new VisualElement();
            foldout.Add(container);

            // RelativePath + đặt tiêu đề foldout
            var relativePathProp = property.FindPropertyRelative("RelativePath");
            var relativePathField = new PropertyField(relativePathProp, "Relative Path");
            foldout.text = string.IsNullOrWhiteSpace(relativePathProp.stringValue)
                ? property.displayName
                : relativePathProp.stringValue;
            container.Add(relativePathField);

            // ValueType
            var valueTypeProp = property.FindPropertyRelative("ValueType");
            var valueTypeField = new PropertyField(valueTypeProp, "Value Type");
            container.Add(valueTypeField);

            // DefaultExpression
            var defaultExpr = new PropertyField(property.FindPropertyRelative("DefaultExpression"), "Default Expression");
            container.Add(defaultExpr);

            // Description
            var description = new PropertyField(property.FindPropertyRelative("Description"), "Description");
            container.Add(description);

            // ReadOnly
            var isReadOnlyProp = property.FindPropertyRelative("IsReadOnly");
            var isReadOnly = new PropertyField(isReadOnlyProp, "Is Read Only");
            container.Add(isReadOnly);

            // Persist + StorageKeyOverride
            var persistProp = property.FindPropertyRelative("Persist");
            var persist = new PropertyField(persistProp, "Persistent");
            container.Add(persist);

            var customKeyField = new PropertyField(property.FindPropertyRelative("StorageKeyOverride"), "Storage Key Override");
            customKeyField.style.paddingLeft = 22;
            container.Add(customKeyField);

            // Range group
            var hasRangeProp = property.FindPropertyRelative("HasRange");
            var hasRange = new PropertyField(hasRangeProp, "Has Range");
            container.Add(hasRange);

            var minField = new PropertyField(property.FindPropertyRelative("Min"), "Min");
            minField.style.paddingLeft = 22;
            container.Add(minField);

            var maxField = new PropertyField(property.FindPropertyRelative("Max"), "Max");
            maxField.style.paddingLeft = 22;
            container.Add(maxField);

            // --- Preview FullPath & StorageKey (readonly label) ---
            var previewBox = new HelpBox("", HelpBoxMessageType.None);
            container.Add(previewBox);

            void RefreshPreview()
            {
                var rel = relativePathProp.stringValue?.Trim() ?? "";
                var prefix = def != null ? (def.PathPrefix ?? "").Trim() : "";
                var full = string.IsNullOrEmpty(prefix) ? rel : $"{prefix}.{rel}";
                var typeName = valueTypeProp.stringValue;

                // Toggle range
                bool isNumeric = typeName == "int" || typeName == "float";
                hasRange.SetEnabled(isNumeric);
                bool showRange = isNumeric && hasRangeProp.boolValue;
                minField.style.display = showRange ? DisplayStyle.Flex : DisplayStyle.None;
                maxField.style.display = showRange ? DisplayStyle.Flex : DisplayStyle.None;

                // Toggle persist/key
                bool ro = isReadOnlyProp.boolValue;
                persist.SetEnabled(!ro);
                customKeyField.style.display = (persistProp.boolValue && !ro) ? DisplayStyle.Flex : DisplayStyle.None;

                // Tính storage key
                string key = "";
                if (persistProp.boolValue)
                {
                    var keyOverride = property.FindPropertyRelative("StorageKeyOverride").stringValue;
                    key = string.IsNullOrWhiteSpace(keyOverride) ? $"pf.settings.v1.{full}" : keyOverride.Trim();
                }

                // Cập nhật foldout title
                foldout.text = string.IsNullOrWhiteSpace(rel) ? property.displayName : rel;

                // Preview text
                string line1 = $"FullPath: {(string.IsNullOrWhiteSpace(full) ? "(invalid)" : full)}";
                string line2 = persistProp.boolValue ? $"StorageKey: {key}" : "StorageKey: (not persisted)";
                previewBox.text = $"{line1}\n{line2}";
            }

            // Đăng ký callback cập nhật động
            relativePathField.RegisterValueChangeCallback(_ => RefreshPreview());
            valueTypeField.RegisterValueChangeCallback(_ => RefreshPreview());
            isReadOnly.RegisterValueChangeCallback(_ => RefreshPreview());
            persist.RegisterValueChangeCallback(_ => RefreshPreview());
            hasRange.RegisterValueChangeCallback(_ => RefreshPreview());

            // First refresh
            RefreshPreview();

            return foldout;
        }
    }
}
