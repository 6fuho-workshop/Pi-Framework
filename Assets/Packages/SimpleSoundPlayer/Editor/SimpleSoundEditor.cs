using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using PF.PiEditor.Utils;

namespace PiExtension.SimpleSound
{
    [CustomEditor(typeof(SimpleSoundPlayer))]
    public class SimpleSoundEditor : Editor
    {
        public static List<Type> controllerClasses;
        static string[] choices;
        int choiceIndex = 0;
        SimpleSoundPlayer script;

        [InitializeOnLoadMethod]
        public static void OnLoadHandler() //tạo list controller để chọn
        {
            controllerClasses = new List<Type>();

            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                var type = monoScript.GetClass();
                if (type != null && typeof(ISoundAdapter).IsAssignableFrom(type) && !type.IsAbstract)
                {
                    controllerClasses.Add(type);
                }
            }

            choices = new string[controllerClasses.Count];
            for (var i = 0; i < controllerClasses.Count; i++)
            {
                choices[i] = controllerClasses[i].Name;
            }


        }

        void OnEnable()
        {
            script = (SimpleSoundPlayer)target;
            ISoundAdapter controller = script.GetComponent<ISoundAdapter>();
            Type controllerType = controller is null ? null : controller.GetType();
            if (controller is null)
            {
                if (controllerClasses.Count == 1)
                {
                    controller = script.gameObject.AddComponent<DummyAdapter>();
                }
                else
                {
                    foreach (var type in controllerClasses)
                    {
                        if (type != typeof(DummyAdapter))
                        {
                            controller = (ISoundAdapter)script.gameObject.AddComponent(type);
                            break;
                        }
                    }
                }
            }

            for (var i = 0; i < controllerClasses.Count; i++)
            {
                if (controllerType == controllerClasses[i])
                {
                    choiceIndex = i;
                    break;
                }
            }
        }
        public override void OnInspectorGUI()
        {
            #region adapter

            var oldChoiceIndex = choiceIndex;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Adapter");
            choiceIndex = EditorGUILayout.Popup(choiceIndex, choices);
            EditorGUILayout.EndHorizontal();
            if (oldChoiceIndex != choiceIndex)
            {
                DestroyImmediate((UnityEngine.Object)script.GetComponent<ISoundAdapter>());
                script.gameObject.AddComponent(controllerClasses[choiceIndex]);
            }

            #endregion adapter

            DrawDefaultInspector();
            Draw_AudioMixerButton();
            EditorGUILayout.HelpBox("Sử dụng AudioMixer tạo bởi SimpleSoundPlayer để đảm bảo các chức năng hoạt động đúng!", MessageType.Info, true);
        }

        void Draw_AudioMixerButton()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Create AudioMixer", GUILayout.Width(200), GUILayout.Height(25)))
            {
                CreateAudioMixerAsset();
                //gọi ExitGUI vì khi popup hiện ra thì các lệnh GUI phía dưới có thể báo lỗi do không đồng bộ frame
                EditorGUIUtility.ExitGUI();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        void CreateAudioMixerAsset()
        {
            var paths = AssetHelper.FindAssetsWithFullName("AudioMixer.template");
            if (paths.Length > 0)
            {
                var source = paths[0];
                var audioPath = Application.dataPath + Path.DirectorySeparatorChar + "Audio";
                if (!Directory.Exists(audioPath))
                    audioPath = Application.dataPath;

                var savePath = EditorUtility.SaveFilePanel(
                    "Save AudioMixer pattern",
                    audioPath,
                    "AudioMixer.mixer",
                    "mixer");

                if (savePath.Length != 0)
                {
                    if (File.Exists(savePath))
                        FileUtil.ReplaceFile(source, savePath); 
                    else
                        FileUtil.CopyFileOrDirectory(source, savePath);

                    AssetDatabase.Refresh();
                }
            }
            else
            {
                Debug.LogError("Not found file name AudioMixer.template");
            }
        }
    }
}