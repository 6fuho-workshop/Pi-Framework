using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
using PF.PiEditor.Utils;

namespace PF.PiEditor
{
    /// InitializeOnLoad: Allows you to initialize an Editor class when Unity loads, and when your scripts are recompiled.
    /// Static constructors with this attribute are called when scripts in the project are recompiled 
    /// (also known as a Domain Reload). This happens when Unity first loads your project, but also when Unity detects 
    /// modifications to scripts (depending on your Auto Refresh preferences), 
    /// and when you enter Play Mode (depending on your Play Mode configuration).
    [InitializeOnLoad]
    public class ModuleSetup
    {
        static bool _initialized;
        static List<ModuleInfo> _modules;

        static ModuleSetup()
        {
            Initialize();
        }

        static void Initialize()
        {
            if (_initialized)
                return;
            ReadModules();
        }

        public static ModuleInfo[] GetAllModules()
        {
            Initialize();
            return _modules.ToArray();
        }

        static void ReadModules()
        {
            _modules = new List<ModuleInfo>();
            var paths = AssetHelper.FindAssetsWithFullName("ModuleInfo.json");

            foreach (var path in paths)
            {
                var content = File.ReadAllText(path);
                ModuleInfo m = new ModuleInfo();
                EditorJsonUtility.FromJsonOverwrite(content, m);
                _modules.Add(m);
            }

        }
    }
}