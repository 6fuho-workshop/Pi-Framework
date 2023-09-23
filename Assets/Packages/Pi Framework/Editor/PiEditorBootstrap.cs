using UnityEngine;
using UnityEditor;

using PiEditor.Utils;
using PiFramework;

using System.Collections;
using System.Collections.Generic;
using System;
using System.CodeDom;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using UnityEditor.Compilation;
using UnityEditor.Callbacks;
using System.Linq;

namespace PiEditor
{

    /// InitializeOnLoad: Allows you to initialize an Editor class when Unity loads, and when your scripts are recompiled.
    /// Static constructors with this attribute are called when scripts in the project are recompiled 
    /// (also known as a Domain Reload). This happens when Unity first loads your project, but also when Unity detects 
    /// modifications to scripts (depending on your Auto Refresh preferences), 
    /// and when you enter Play Mode (depending on your Play Mode configuration).
    [InitializeOnLoad]
    public class PiEditorBootstrap
    {
        static bool _systemInitialized;
        static PiEditorBootstrap()
        {
            ValidateFiles();
            ValidateTags();
            UpdateExecutionOrder();
        }

        static void ValidateFiles()
        {
            //create folder Resources
            var path = FileHelper.GetPiDataPath() + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(path + "Resources");

            //create folder Code
            path += "Code" + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(path);

            //Check Pi.cs file
            var piclass = path + "Pi.cs";
            if (!File.Exists(piclass))
                CopyPiClassFile(piclass);
        }

        static void ValidateTags()
        {

        }

        /// <summary>
        /// Copy file mẫu Pi.cs.txt vào thư mục _PiProjectData/Code
        /// </summary>
        /// <param name="desPath"></param>
        static void CopyPiClassFile(string desPath)
        {
            var files = FileHelper.FindAssetsWithFullName("Pi.cs.txt");
            if (files.Length == 0)
            {
                Debug.LogError("Not found Pi.cs.txt");
            }
            else
            {
                var path = files[0];
                FileUtil.CopyFileOrDirectory(path, desPath);
            }
        }

        /// <summary>
        /// Dùng CompilationPipeline.assemblyCompilationFinished và
        /// CompilationPipeline.compilationFinished đều không ổn
        /// </summary>
        [DidReloadScripts()]
        static void OnScriptReloaded()
        {
            PinServicesGenerator.Generate();
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