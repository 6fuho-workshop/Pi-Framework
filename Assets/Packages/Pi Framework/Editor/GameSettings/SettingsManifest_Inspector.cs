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
            Settings_Generator.Generate();
        }
    }
}