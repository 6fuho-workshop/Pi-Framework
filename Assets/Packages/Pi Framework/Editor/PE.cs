﻿using PiEditor.Callbacks;
using PiEditor.Settings;
using PiEditor.Utils;
using PiFramework.Settings;
using PiFramework;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;

namespace PiEditor
{
    /// InitializeOnLoad: Allows you to initialize an Editor class when Unity loads, and when your scripts are recompiled.
    /// Static constructors with this attribute are called when scripts in the project are recompiled 
    /// (also known as a Domain Reload). This happens when Unity first loads your project, but also when Unity detects 
    /// modifications to scripts (depending on your Auto Refresh preferences), 
    /// and when you enter Play Mode (depending on your Play Mode configuration).

    [InitializeOnLoad]
    public class PE
    {
        static bool _recompile;
        static PE()
        {
            Debug.Log("PE static ctor");
            var t = EditorApplication.timeSinceStartup;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            if (!IsPiImported())
            {
                Import();
                InvokeOnLoadCallbacks(typeof(OnPiImportAttribute));
            }
            else
            {
                if (IsPiActivated())
                {
                    PiEditorBootstrap.Bootstrap();
                    InvokeOnLoadCallbacks(typeof(OnLoadPiEditorAttribute));
                    //SetupFramework();
                    //InvokeOnLoadCallbacks(typeof(OnPiSetupAttribute));
                }

                //PiEditorBootstrap.Bootstrap();
                //InvokeOnLoadCallbacks(typeof(OnLoadPiEditorAttribute));
            }

            t = EditorApplication.timeSinceStartup - t;
            //Debug.Log("Pi Editor ctor: " + t + "s");
        }


        /// <summary>
        /// Check files existed thay vi check class exist vi class co the bi loi compile
        /// </summary>
        /// <returns></returns>
        public static bool IsPiImported()
        {
            return Directory.Exists(FileHelper.dataDirectory);
        }

        public static bool IsPiActivated()
        {
            return File.Exists(FileHelper.piPrefabPath);
        }


        /// <summary>
        /// Importing process is for internal framework only
        /// </summary>
        public static void Import()
        {
            Debug.Log("Call Import");
            var ds = Path.DirectorySeparatorChar;
            var scriptDirectory = FileHelper.scriptDirectory + ds;

            FileHelper.FineAndCopyAsset("Pi.cs.txt", scriptDirectory + "Pi.cs");
            FileHelper.FineAndCopyAsset("Settings.cs.txt", scriptDirectory + "Settings.cs");

            Debug.Log("begin call SettingsGenerator");
            SettingsGenerator.Generate();
            Debug.Log("begin call PinServicesGenerator");
            PinServicesGenerator.Generate();
            //RecompileImmediate();
        }

        [MenuItem("Pi/Setup Framework")]
        public static void SetupFramework()
        {
            Debug.Log("Call SetupFramework");
            Directory.CreateDirectory(FileHelper.piResourcesDirectory);
            Directory.CreateDirectory(FileHelper.settingDirectory);
            Directory.CreateDirectory(FileHelper.moduleDirectory);
            SetupPiPrefab();
            SettingsGenerator.Generate();
        }

        static void SetupPiPrefab()
        {
            var go = new GameObject("PiFramework");
            go.AddComponent<LatestExecOrder>();
            go.AddComponent<PiRoot>();

            var settingManager = new GameObject("Settings");
            settingManager.transform.parent = go.transform;
            settingManager.AddComponent<RuntimeSettingsManager>();

            var settingLoader = new GameObject("Default").AddComponent<SettingsLoader>();
            settingLoader.transform.parent = settingManager.transform;
            settingLoader.settings = GetDefaultSettings();

            var modules = new GameObject("Modules");
            modules.transform.parent = go.transform;

            Debug.Log("begin call SaveAsPrefabAsset");
            PrefabUtility.SaveAsPrefabAsset(go, FileHelper.piPrefabPath);
            Debug.Log("After call SaveAsPrefabAsset");
            GameObject.DestroyImmediate(go);

            static RuntimeSettings GetDefaultSettings()
            {
                var paths = FileHelper.FindScriptableObjects<RuntimeSettings>();
                if (paths.Length > 0)
                {
                    return AssetDatabase.LoadAssetAtPath<RuntimeSettings>(paths[0]);
                }
                else
                {
                    var settings = ScriptableObject.CreateInstance("Settings");
                    AssetDatabase.CreateAsset(settings, "Assets/Settings/Default.asset");
                    Debug.Log("begin call SaveAssets");
                    AssetDatabase.SaveAssets();
                    Debug.Log("After call SaveAssets");
                    return settings as RuntimeSettings;
                }
            }
        }

        static void InvokeOnLoadCallbacks(Type type)
        {
            var callbacks = TypeCache.GetMethodsWithAttribute(type);
            foreach (var callback in callbacks)
            {
                callback.Invoke(null, null);
            }
        }

        public static void Recompile()
        {
            _recompile = true;
        }

        static void RecompileImmediate()
        {
            Debug.Log("PiEditor Recompile");
            _recompile = false;
            //Dùng EditorUtility.RequestScriptReload() không thích hợp vì nó không compile changed scripts
            CompilationPipeline.RequestScriptCompilation();

            //Gọi Refresh sau khi Compile nếu không Unity sẽ tự gọi nó khi switch sang app khác rồi quay lại Unity
            AssetDatabase.Refresh();
        }

        static void Update()
        {
            if (_recompile)
                RecompileImmediate();
        }
    }
}