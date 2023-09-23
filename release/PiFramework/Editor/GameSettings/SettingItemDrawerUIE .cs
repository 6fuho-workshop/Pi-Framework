using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using PopupWindow = UnityEngine.UIElements.PopupWindow;

namespace PiEditor.Settings
{
    [CustomPropertyDrawer(typeof(SettingItem))]
    public class SettingItemDrawerUIE  : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // Create property container element.
            var container = new VisualElement();

            //var popup = new PopupWindow();
            //container.Add(popup);
            //popup.text = property.displayName + " - Using C#";
            //popup.Add(new PropertyField(property.FindPropertyRelative("path")));
            //popup.Add(new PropertyField(property.FindPropertyRelative("type")));
            //popup.Add(new PropertyField(property.FindPropertyRelative("defaultValue"), "CustomLabel: Name"));

            // Create property fields.
            var amountField = new PropertyField(property.FindPropertyRelative("path"));
            var unitField = new PropertyField(property.FindPropertyRelative("type"));
            var nameField = new PropertyField(property.FindPropertyRelative("defaultValue"), "Fancy Name");

            // Add fields to the container.
            container.Add(amountField);
            container.Add(unitField);
            container.Add(nameField);

            return container;
        }
    }
}