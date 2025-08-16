using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PF.PiEditor.Utils
{
    public class PiPath
    {
        public static readonly string projectPath;
        public static readonly string piDataPath;
        public static readonly string resourcePath;
        public static readonly string scriptPath;
        public static readonly string editorPath;
        public static readonly string cachePath;

        public static readonly string settingPath;
        public static readonly string modulePath;

        public static readonly string piPrefab;

        static PiPath()
        {
            projectPath = Application.dataPath[..(Application.dataPath.LastIndexOf('/'))];
            piDataPath = "Assets/_PiProjectData";
            resourcePath = piDataPath + "/Resources";
            editorPath = piDataPath + "/Editor";
            scriptPath = piDataPath + "/Scripts";
            cachePath = piDataPath + "/Cache";

            settingPath = "Assets/Settings";
            modulePath = "Assets/PiModules";

            piPrefab = resourcePath + "/PiFramework.prefab";
        }

        public static string AbsolutePath(string path)
        {
            return projectPath + "/" + path;
        }

        internal static void CreateDirectories()
        {
            Directory.CreateDirectory(resourcePath);
            Directory.CreateDirectory(cachePath);
            Directory.CreateDirectory(scriptPath);
            Directory.CreateDirectory(editorPath);
            Directory.CreateDirectory(settingPath);
            Directory.CreateDirectory(modulePath);

        }
    }
}