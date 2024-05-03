using UnityEngine;
using UnityEditor;
using PiEditor.Utils;
using PiFramework;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor.VersionControl;
using PiFramework.Settings;


namespace PiEditor
{
    internal class PiEditorBootstrap
    {
        internal static void Bootstrap()
        {
            if (!File.Exists(FileHelper.piPrefabPath))
            {
                SetupPrefab();
            }

            ValidateFiles();
            ValidateModulePrefabs();
            UpdateExecutionOrder();
        }

        static void SetupPrefab()
        {
            var go = new GameObject("PiFramework");
            go.AddComponent<LatestExecOrder>();
            go.AddComponent<PiRoot>();

            var settingManager = new GameObject("Settings");
            settingManager.transform.parent = go.transform;
            settingManager.AddComponent<SettingManager>();

            var settingLoader = new GameObject("Default").AddComponent<SettingsLoader>();
            settingLoader.transform.parent = settingManager.transform;
            settingLoader.settings = GetDefaultSettings();

            var modules = new GameObject("Modules");
            modules.transform.parent = go.transform;

            PrefabUtility.SaveAsPrefabAsset(go, FileHelper.piPrefabPath);
            GameObject.DestroyImmediate(go);

            static GameSettings GetDefaultSettings()
            {
                var paths = FileHelper.FindScriptableObjects<GameSettings>();
                if (paths.Length > 0)
                {
                    return AssetDatabase.LoadAssetAtPath<GameSettings>(paths[0]);
                }
                else
                {

                }
                return null;
            }
        }

        static void ValidateModulePrefabs()
        {
            var allModules = GetAllModules();
            var prefab = PrefabUtility.LoadPrefabContents(FileHelper.piPrefabPath);
            var moduleContainer = prefab.transform.Find("Modules");
            //var settings = prefab.transform.Find("Settings");
            var addedModules = GetAddedModules(moduleContainer);
            bool dirty = false;

            foreach (var module in allModules)
            {
                if (!addedModules.Contains(module))
                {
                    var mInstance = GetModulePrefabInstance(module);
                    if (mInstance == null)
                        Debug.LogError("Something wrong: GetModulePrefabInstance return null");
                    else
                        mInstance.transform.parent = moduleContainer.transform;

                    dirty = true;
                }
            }

            if (dirty)
                PrefabUtility.SaveAsPrefabAsset(prefab, FileHelper.piPrefabPath);

            PrefabUtility.UnloadPrefabContents(prefab);

            #region helpers

            static List<Type> GetAddedModules(Transform container)
            {
                var components = container.transform.GetComponentsInChildren<PiModule>(true);
                var addedModules = new List<Type>();
                foreach (var module in components)
                    addedModules.Add(module.GetType());
                return addedModules;
            }

            static List<Type> GetAllModules()
            {
                var moduleClasses = new List<Type>();

                foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
                {
                    var type = monoScript.GetClass();
                    if (type != null && type.IsSubclassOf(typeof(PiModule)))
                    {
                        moduleClasses.Add(type);
                    }
                }
                return moduleClasses;
            }

            static GameObject GetModulePrefabInstance(Type moduleType)
            {
                var ds = Path.DirectorySeparatorChar;
                var assetPath = FileHelper.moduleDirectory + ds + moduleType.Name + ".prefab";
                if (!File.Exists(assetPath))
                {
                    var m = new GameObject(moduleType.Name);
                    m.AddComponent(moduleType);
                    PrefabUtility.SaveAsPrefabAssetAndConnect(m, assetPath, InteractionMode.AutomatedAction);
                    return m;
                }
                else
                {
                    var localAssetPath = "Assets" + ds + "PiModules" + ds + moduleType.Name + ".prefab";
                    var go = (GameObject)AssetDatabase.LoadMainAssetAtPath(localAssetPath);
                    return (GameObject)PrefabUtility.InstantiatePrefab(go);

                }
            }

            #endregion helpers
        }

        static void ValidateFiles()
        {
            var ds = Path.DirectorySeparatorChar;
            var path = FileHelper.dataDirectory + ds;

            Directory.CreateDirectory(FileHelper.piResourcesDirectory);
            Directory.CreateDirectory(FileHelper.settingDirectory);
            Directory.CreateDirectory(FileHelper.moduleDirectory);
            Directory.CreateDirectory(path + "PiClass");

            FileHelper.CopyAssetWithFullName("Pi.cs.txt", path + "PiClass" + ds + "Pi.cs");
            FileHelper.CopyAssetWithFullName("Settings.cs.txt", path + "Settings" + ds + "Settings.cs");
            
        }





        /// <summary>
        /// Todo: khi remove attribute ở code thì trong project settings vẫn còn
        /// </summary>
        static void UpdateExecutionOrder()
        {
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                Type type = monoScript.GetClass();
                if (type != null)
                {
                    Attribute[] attrs = (Attribute[])type.GetCustomAttributes(typeof(ExecutionOrderAttribute), false);

                    foreach (Attribute attr in attrs)
                    {
                        var order = (attr as ExecutionOrderAttribute).Order;
                        if (MonoImporter.GetExecutionOrder(monoScript) != order)
                            MonoImporter.SetExecutionOrder(monoScript, order);
                    }
                }
            }
        }
    }


}