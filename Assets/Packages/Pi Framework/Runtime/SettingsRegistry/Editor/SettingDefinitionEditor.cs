using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PF.PiEditor.Settings
{
    [CustomEditor(typeof(SettingDefinition))]
    public class SettingDefinitionEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement myInspector = new();
            InspectorElement.FillDefaultInspector(myInspector, serializedObject, this);

            var applyBtn = new Button(ApplyBtnClickedHandler);
            applyBtn.text = "Apply";
            applyBtn.style.width = Length.Percent(30);
            applyBtn.style.alignSelf = Align.Center;

            myInspector.Add(applyBtn);

            return myInspector;
        }

        void ApplyBtnClickedHandler()
        {
            SettingsGenerator.Generate();
            AssetDatabase.SaveAssetIfDirty(target);
        }
    }

    [CustomPropertyDrawer(typeof(SettingEntry))]
    public class SettingEntryDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            // Create a new VisualElement to be the root the property UI
            var container = new VisualElement();
            var foldout = new Foldout();
            foldout.Add(container);
            foldout.value = false;
            //foldout.viewDataKey = property.displayName; //đoạn này chưa hiểu lắm, tạm set foldout.value về false


            var relativePathProperty = property.FindPropertyRelative("RelativePath");
            var relativePath = new PropertyField(relativePathProperty, "RelativePath");
            foldout.text = string.IsNullOrWhiteSpace(relativePathProperty.stringValue) ? property.displayName : relativePathProperty.stringValue;
            container.Add(relativePath);

            var valueType = new PropertyField(property.FindPropertyRelative("ValueType"), "Value Type");
            container.Add(valueType);

            var defaultExpression = new PropertyField(property.FindPropertyRelative("DefaultExpression"), "Default Expression");
            container.Add(defaultExpression);

            var description = new PropertyField(property.FindPropertyRelative("Description"), "Description");
            container.Add(description);

            var isReadOnly = new PropertyField(property.FindPropertyRelative("IsReadOnly"), "Is Read Only");
            container.Add(isReadOnly);

            var persist = new PropertyField(property.FindPropertyRelative("Persist"), "Persistent");
            container.Add(persist);

            var customKey = new PropertyField(property.FindPropertyRelative("StorageKeyOverride"), "Storage Key Override");
            customKey.style.paddingLeft = 25;
            container.Add(customKey);

            var hasRange = new PropertyField(property.FindPropertyRelative("HasRange"), "Has Range");
            container.Add(hasRange);

            var min = new PropertyField(property.FindPropertyRelative("Min"), "Min");
            min.style.paddingLeft = 25;
            container.Add(min);

            var max = new PropertyField(property.FindPropertyRelative("Max"), "Max");
            max.style.paddingLeft = 25;
            container.Add(max);

            valueType.RegisterValueChangeCallback(evt =>
            {
                var typeName = evt.changedProperty.stringValue;
                hasRange.SetEnabled(typeName == "int" || typeName == "float");
            });

            isReadOnly.RegisterValueChangeCallback(evt =>
            {
                var readOnly = evt.changedProperty.boolValue;
                persist.SetEnabled(!readOnly);
            });

            hasRange.RegisterValueChangeCallback(evt =>
            {
                min.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                max.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            persist.RegisterValueChangeCallback(evt =>
            {
                customKey.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            return foldout;
        }
    }
}