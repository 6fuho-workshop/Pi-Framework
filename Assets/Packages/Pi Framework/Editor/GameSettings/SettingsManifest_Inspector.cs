using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;

namespace PiEditor.Settings
{
    [CustomEditor(typeof(SettingsManifest))]
    public class SettingsManifest_Inspector : Editor
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

    [CustomPropertyDrawer(typeof(SettingEntity))]
    public class SettingEntity_PropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {

            // Create a new VisualElement to be the root the property UI
            var container = new VisualElement();
            var foldout = new Foldout();
            foldout.Add(container);
            foldout.value = false;
            //foldout.viewDataKey = property.displayName; //đoạn này chưa hiểu lắm, tạm set foldout.value về false


            var pathProperty = property.FindPropertyRelative("path");
            var path = new PropertyField(pathProperty, "Path");
            foldout.text = string.IsNullOrWhiteSpace(pathProperty.stringValue) ? property.displayName : pathProperty.stringValue;
            container.Add(path);

            var type = new PropertyField(property.FindPropertyRelative("type"), "Type");
            container.Add(type);

            var defaultValue = new PropertyField(property.FindPropertyRelative("defaultValue"), "Default Value");
            container.Add(defaultValue);

            var tooltip = new PropertyField(property.FindPropertyRelative("tooltip"), "Tooltip");
            container.Add(tooltip);

            var readOnly = new PropertyField(property.FindPropertyRelative("readOnly"), "Is Read Only");
            container.Add(readOnly);

            var persistent = new PropertyField(property.FindPropertyRelative("persistent"), "Persistent");
            container.Add(persistent);

            var customKey = new PropertyField(property.FindPropertyRelative("customKey"), "Custom Key");
            customKey.style.paddingLeft = 25;
            container.Add(customKey);

            var useRange = new PropertyField(property.FindPropertyRelative("useRange"), "Use Range");
            container.Add(useRange);

            var min = new PropertyField(property.FindPropertyRelative("min"), "Min");
            min.style.paddingLeft = 25;
            container.Add(min);

            var max = new PropertyField(property.FindPropertyRelative("max"), "Max");
            max.style.paddingLeft = 25;
            container.Add(max);

            type.RegisterValueChangeCallback(evt =>
            {
                var typeName = evt.changedProperty.stringValue;
                useRange.SetEnabled(typeName == "int" || typeName == "float");
            });

            readOnly.RegisterValueChangeCallback(evt =>
            {
                var readOnly = evt.changedProperty.boolValue;
                persistent.SetEnabled(!readOnly);
            });

            useRange.RegisterValueChangeCallback(evt =>
            {
                min.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
                max.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            persistent.RegisterValueChangeCallback(evt =>
            {
                customKey.style.display = evt.changedProperty.boolValue ? DisplayStyle.Flex : DisplayStyle.None;
            });

            return foldout;
        }
    }
}