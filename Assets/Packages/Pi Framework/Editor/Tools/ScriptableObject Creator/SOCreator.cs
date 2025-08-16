using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// @Hant
/// </summary>

namespace PF.PiEditor.Tools.SOCreator
{
    public class ScriptableObjectCreator : EditorWindow
    {
        private const string _title = "Create ScriptableObject";
        private static readonly Vector2 _winSize = new Vector2(370f, 150f);

        int _choiceIndex;
        string[] _choices;
        Type[] _types;

        [MenuItem("Pi/Create/ScriptableObject of any Type")]
        private static void ShowWindow()
        {
            ScriptableObjectCreator.Open();
        }

        public static void Open()
        {
            var wnd = EditorWindow.GetWindow<ScriptableObjectCreator>(true, _title, true);
            wnd.position = new Rect(Screen.width / 2, Screen.height / 2, 0, 0);
            wnd.maxSize = _winSize;
            wnd.minSize = _winSize;
            wnd.ShowUtility();

            wnd.Init();
        }

        void Init()
        {
            _types = GetAllTypes();
            _choices = _types.Select(x => x.Name).ToArray();
        }

        Type[] GetAllTypes()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.GetName().Name.StartsWith("UnityEngine"))
                .Where(x => !x.GetName().Name.StartsWith("Unity."))
                .Where(x => !x.GetName().Name.StartsWith("System."))
                .Where(x => !x.GetName().Name.StartsWith("mscorlib"))
                .Where(x => !x.GetName().Name.StartsWith("UnityEditor"))
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass && x.IsSubclassOf(typeof(ScriptableObject)))
                .Where(x => !x.IsSubclassOf(typeof(Editor)))
                .Where(x => !x.IsSubclassOf(typeof(EditorWindow)));

            return types.ToArray();
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Select ScriptableObject Class from list", EditorStyles.wordWrappedLabel);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Type: ");
            _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(40);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Create!", GUILayout.Width(100)))
            {
                var type = _types[_choiceIndex];
                var asset = ScriptableObject.CreateInstance(type);

                AssetDatabase.CreateAsset(asset, "Assets/NewScripableObject.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }
}