using PF.PiEditor.Callbacks;
using PF.PiEditor.Settings;
using PF.PiEditor.Utils;
using PF.Core.Settings;
using PF.Internal;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.Compilation;
using System.Collections.Generic;
using PF.Unity.Internal;

namespace PF.PiEditor
{
    /// <summary>
    /// InitializeOnLoad: Allows you to initialize an Editor class when Unity loads, and when your scripts are recompiled.
    /// Static constructors with this attribute are called when scripts in the project are recompiled 
    /// (also known as a Domain Reload). This happens when Unity first loads your project, but also when Unity detects 
    /// modifications to scripts (depending on your Auto Refresh preferences), 
    /// and when you enter Play Mode (depending on your Play Mode configuration).  
    /// FLOW: 
    /// Domain Reload 
    /// => PE static ctor 
    /// => OnPostprocessAllAssets 
    /// => DelayCall 
    /// => OnPostprocessAllAssets
    /// </summary>

    [InitializeOnLoad]
    public class PE
    {
        static bool _recompile;
        public static bool initialized { get; private set; } = false;
        public static bool activated => File.Exists(PathHelper.PiPrefabPath);

        static PE()
        {
            initialized = false;
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            EditorApplication.delayCall += DelayCall;
            Debug.Log("PE static ctor");
        }

        internal static void Initialize()
        {
            if (initialized)
                return;

            if (!IsPiImported())
            {
                Import();
                InvokeOnLoadCallbacks(typeof(OnPiImportAttribute));
            }
            else
            {
                if (activated)
                {
                    PiEditorBootstrap.Bootstrap();
                    initialized = true;
                    InvokeOnLoadCallbacks(typeof(OnLoadPiEditorAttribute));
                }
            }
        }

        /// <summary>
        /// DelayCall make sure all assets are loaded and safe to use AssetDatabase methods
        /// </summary>
        static void DelayCall()
        {
            Debug.Log("DelayCall");
            Initialize();
        }

        /// <summary>
        /// Check files existed thay vi check class exist vi class co the bi loi compile
        /// </summary>
        /// <returns></returns>
        static bool IsPiImported()
        {
            return Directory.Exists(PathHelper.PiDataDir);
        }


        /// <summary>
        /// Importing process is for internal framework only
        /// Import đảm bảo các asset sẵn sàng cho bước setup, script are generated
        /// </summary>
        static void Import()
        {
            var scriptDirectory = PathHelper.PiDataScriptsDir + "/";

            AssetHelper.FineAndCopyAsset("Pi.cs.txt", scriptDirectory + "Pi.cs");
            AssetHelper.FineAndCopyAsset("Settings.cs.txt", scriptDirectory + "Settings.cs");

            RebuildSettings();
            PinServicesGenerator.Generate();
        }

        [MenuItem("▶ Pi ◀/Force Generate Code/Settings")]
        internal static void RebuildSettings()
        {
            SettingsGenerator.Generate();
        }

        /// <summary>
        /// Phuong an Auto setup pha san vi settings manifest khong duoc load luc import
        /// </summary>
        [MenuItem("▶ Pi ◀/Setup Framework")]
        static void SetupFramework()
        {
            PathHelper.CreateDirectories();
            SetupPiPrefab();
            AssetDatabase.SaveAssets();
        }

        static void SetupPiPrefab()
        {
            var go = new GameObject("PiFramework");
            go.AddComponent<PostPhaseEventRaiser>();
            go.AddComponent<PiRoot>();

            var settingManager = new GameObject("Settings");
            settingManager.transform.parent = go.transform;
            settingManager.AddComponent<RuntimeSettingsManager>();

            var settingLoader = new GameObject("Default").AddComponent<SettingsLoader>();
            settingLoader.transform.parent = settingManager.transform;
            settingLoader.settings = GetOrCreateSettings();

            var modules = new GameObject("Modules");
            modules.transform.parent = go.transform;

            PrefabUtility.SaveAsPrefabAsset(go, PathHelper.PiPrefabPath);
            GameObject.DestroyImmediate(go);

            static RuntimeSettings GetOrCreateSettings()
            {
                var paths = AssetHelper.FindScriptableObjects<RuntimeSettings>();
                if (paths.Length > 0)
                {
                    return AssetDatabase.LoadAssetAtPath<RuntimeSettings>(paths[0]);
                }
                else
                {
                    var settings = ScriptableObject.CreateInstance("Settings");
                    AssetDatabase.CreateAsset(settings, "Assets/Settings/Default.asset");
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

        public static void RecompileImmediate()
        {
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