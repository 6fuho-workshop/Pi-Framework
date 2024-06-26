﻿using UnityEngine;
using UnityEditor;
using PiEditor.Utils;
using PiFramework;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using PiFramework.Settings;
using PiEditor.Settings;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;


namespace PiEditor
{
    internal class PiEditorBootstrap
    {
        internal static void Bootstrap()
        {
            ValidateModulePrefabs();
            UpdateExecutionOrder();
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