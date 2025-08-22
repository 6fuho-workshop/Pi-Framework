// Assets/PF/Editor/Settings/SettingDefinitionEditor.cs
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

// (giữ nguyên namespace)
namespace PF.PiEditor.Settings
{
    // Drawer SettingEntry giữ nguyên như hiện tại của bạn
    [CustomPropertyDrawer(typeof(SettingEntry))]
    public class SettingEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var def = property.serializedObject.targetObject as SettingDefinition;

            var foldout = new Foldout { value = false };
            var container = new VisualElement();
            foldout.Add(container);

            var relativePathProp = property.FindPropertyRelative("RelativePath");
            var relativePathField = new PropertyField(relativePathProp, "Relative Path");
            foldout.text = string.IsNullOrWhiteSpace(relativePathProp.stringValue)
                ? property.displayName
                : relativePathProp.stringValue;
            container.Add(relativePathField);

            var valueTypeProp = property.FindPropertyRelative("ValueType");
            var valueTypeField = new PropertyField(valueTypeProp, "Value Type");
            container.Add(valueTypeField);

            var defaultExpr = new PropertyField(property.FindPropertyRelative("DefaultExpression"), "Default Expression");
            container.Add(defaultExpr);

            var description = new PropertyField(property.FindPropertyRelative("Description"), "Description");
            container.Add(description);

            var isReadOnlyProp = property.FindPropertyRelative("IsReadOnly");
            var isReadOnly = new PropertyField(isReadOnlyProp, "Is Read Only");
            container.Add(isReadOnly);

            var persistProp = property.FindPropertyRelative("Persist");
            var persist = new PropertyField(persistProp, "Persistent");
            container.Add(persist);

            var customKeyField = new PropertyField(property.FindPropertyRelative("StorageKeyOverride"), "Storage Key Override");
            customKeyField.style.paddingLeft = 22;
            container.Add(customKeyField);

            var hasRangeProp = property.FindPropertyRelative("HasRange");
            var hasRange = new PropertyField(hasRangeProp, "Has Range");
            container.Add(hasRange);

            var minField = new PropertyField(property.FindPropertyRelative("Min"), "Min");
            minField.style.paddingLeft = 22;
            container.Add(minField);

            var maxField = new PropertyField(property.FindPropertyRelative("Max"), "Max");
            maxField.style.paddingLeft = 22;
            container.Add(maxField);

            var previewBox = new HelpBox("", HelpBoxMessageType.None);
            container.Add(previewBox);

            void RefreshPreview()
            {
                var rel = relativePathProp.stringValue?.Trim() ?? "";
                var prefix = def != null ? (def.PathPrefix ?? "").Trim() : "";
                var full = string.IsNullOrEmpty(prefix) ? rel : $"{prefix}.{rel}";
                var typeName = valueTypeProp.stringValue;

                bool isNumeric = typeName == "int" || typeName == "float";
                hasRange.SetEnabled(isNumeric);
                bool showRange = isNumeric && hasRangeProp.boolValue;
                minField.style.display = showRange ? DisplayStyle.Flex : DisplayStyle.None;
                maxField.style.display = showRange ? DisplayStyle.Flex : DisplayStyle.None;

                bool ro = isReadOnlyProp.boolValue;
                persist.SetEnabled(!ro);
                customKeyField.style.display = (persistProp.boolValue && !ro) ? DisplayStyle.Flex : DisplayStyle.None;

                string key = "";
                if (persistProp.boolValue)
                {
                    var keyOverride = property.FindPropertyRelative("StorageKeyOverride").stringValue;
                    key = string.IsNullOrWhiteSpace(keyOverride) ? $"pf.settings.v1.{full}" : keyOverride.Trim();
                }

                foldout.text = string.IsNullOrWhiteSpace(rel) ? property.displayName : rel;

                string line1 = $"FullPath: {(string.IsNullOrWhiteSpace(full) ? "(invalid)" : full)}";
                string line2 = persistProp.boolValue ? $"StorageKey: {key}" : "StorageKey: (not persisted)";
                previewBox.text = $"{line1}\n{line2}";
            }

            relativePathField.RegisterValueChangeCallback(_ => RefreshPreview());
            valueTypeField.RegisterValueChangeCallback(_ => RefreshPreview());
            isReadOnly.RegisterValueChangeCallback(_ => RefreshPreview());
            persist.RegisterValueChangeCallback(_ => RefreshPreview());
            hasRange.RegisterValueChangeCallback(_ => RefreshPreview());

            RefreshPreview();
            return foldout;
        }
    }
}
