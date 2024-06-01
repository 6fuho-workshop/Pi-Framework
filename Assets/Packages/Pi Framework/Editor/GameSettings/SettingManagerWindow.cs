using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace PiEditor.Settings
{
    public class SettingManagerWindow : EditorWindow
    {
        [MenuItem("Pi/Runtime Settings")]
        public static void ShowSettingsWindow()
        {
            SettingManagerWindow wnd = GetWindow<SettingManagerWindow>();
            wnd.titleContent = new GUIContent("Runtime Setting Manager");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // VisualElements objects can contain other VisualElement following a tree hierarchy.
            VisualElement label = new Label("Hello World! From C#");
            root.Add(label);

            // Create a two-pane view with the left pane being fixed.
            var splitView = new TwoPaneSplitView(0, 250, TwoPaneSplitViewOrientation.Horizontal);

            // Add the view to the visual tree by adding it as a child to the root element.
            root.Add(splitView);

            // A TwoPaneSplitView needs exactly two child elements.
            var leftPane = new VisualElement();
            splitView.Add(leftPane);
            var rightPane = new VisualElement();
            splitView.Add(rightPane);
        }
    }
}