using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using PiFramework;

namespace PiEditor
{
    [CustomEditor(typeof(SceneStartup))]
    public class SceneStartupEditor : Editor
    {

        SceneStartup targetScript;

        //SerializedProperty scenePath;
        SerializedProperty startupType;
        SerializedProperty redirectScene;
        SerializedProperty fragments;
        SerializedProperty destroyOnLoadObjs;

        private void OnEnable()
        {
            //scenePath = serializedObject.FindProperty("scenePath");
            startupType = serializedObject.FindProperty("startupType");
            redirectScene = serializedObject.FindProperty("redirectScene");
            fragments = serializedObject.FindProperty("fragments");
            //destroyOnLoadObjs = serializedObject.FindProperty("destroyOnLoadObjs");

            targetScript = (SceneStartup)target;

            var currentScene = EditorSceneManager.GetActiveScene();
        }


        public override void OnInspectorGUI()
        {
            // Make all the public and serialized fields visible in Inspector
            EditorGUILayout.PropertyField(startupType);

            string type = startupType.enumNames[startupType.enumValueIndex];

            if (type.Equals("Additive"))
            {
                EditorGUILayout.PropertyField(fragments);
            }
            else
            {
                EditorGUILayout.PropertyField(redirectScene);
                //EditorGUILayout.PropertyField(destroyOnLoadObjs);
            }
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("Pi/GameObject/Scene Startup")]
        public static void CreateSceneStartupObject()
        {
            var go = new GameObject("Scene Startup");
            go.AddComponent<SceneStartup>();
        }
    }

}