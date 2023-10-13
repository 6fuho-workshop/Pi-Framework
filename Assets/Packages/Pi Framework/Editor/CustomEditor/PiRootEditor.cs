using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PiFramework;
using System;
using System.Reflection;

namespace PiEditor
{
    [CustomEditor(typeof(PiGameBase))]
    public class PiRootEditor : Editor
    {
        static List<Type> moduleClasses;

        /// <summary>
        /// Get and Cache all class derived from PiModule
        /// </summary>
        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor()
        {
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
        
            moduleClasses = new List<Type>();
            
            foreach (MonoScript monoScript in MonoImporter.GetAllRuntimeMonoScripts())
            {
                var type = monoScript.GetClass();
                if (type != null && type.IsSubclassOf(typeof(PiModule)))
                {
                    moduleClasses.Add(type);
                }
            }
            
            ValidateChildren();
        }
        static void OnHierarchyChanged()
        {
            ValidateChildren();
        }

        static GameObject GetOrCreateChild(string childName)
        {
            var app = GameObject.FindObjectOfType<PiGameBase>();
            if (app == null)
                return null;
            GameObject child = null;
            foreach (Transform c in app.transform.GetComponentsInChildren<Transform>())
            {
                if (c.gameObject.name.Equals(childName))
                {
                    child = c.gameObject;
                    break;
                }
            }

            if(child == null)
            {
                child = new GameObject(childName);
                child.transform.SetParent(app.transform);
            }
            child.hideFlags = HideFlags.NotEditable;
            return child;
        }
        public static void ValidateChildren()
        {
            GameObject moduleContainer = GetOrCreateChild("Modules");
            if(moduleContainer)
                ValidateModules(moduleContainer);
        }
        /// <summary>
        /// Kiểm tra việc add các modules script trong Scene
        /// </summary>
        /// <param name="moduleContainer">GameObject có tên là Modules, là con của object PiFramework</param>
        static void ValidateModules(GameObject moduleContainer)
        {
            List<Transform> modules = new List<Transform>();
            foreach (Type t in moduleClasses)
            {
                var comp = moduleContainer.GetComponentInChildren(t);

                //không chấp nhận module ở child object cấp cháu
                if (comp && !comp.transform.IsChildOf(moduleContainer.transform))
                {
                    DestroyImmediate(comp.gameObject);
                    comp = moduleContainer.GetComponentInChildren(t);
                }
                
                if (!comp)
                {
                    comp = new GameObject(t.Name).AddComponent(t);
                    comp.transform.SetParent(moduleContainer.transform);
                }
                
                modules.Add(comp.transform);
                var types = comp.GetType().GetCustomAttributes(typeof(ModuleObjectNameAttribute),false);
                if(types.Length > 0)
                {
                    var name = (types[0] as ModuleObjectNameAttribute).Name;
                    if (!name.Equals(comp.gameObject.name))
                        comp.gameObject.name = name;
                }
            }

            //remove những object không chứa module
            var i = moduleContainer.transform.childCount;
            while(i > 0)
            {
                i--;
                var child = moduleContainer.transform.GetChild(i);
                if (!modules.Contains(child))
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            
        }

        /*
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
        */
    }
}