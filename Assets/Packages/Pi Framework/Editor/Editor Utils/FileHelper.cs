﻿using UnityEngine;
using PiEditor;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using System;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

namespace PiEditor.Utils
{
    public class FileHelper
    {
        static readonly char ds = Path.DirectorySeparatorChar;
        static readonly string dataPath = Application.dataPath + Path.DirectorySeparatorChar;

        /// <summary>
        /// full path of _PiProjectData folder
        /// </summary>
        public static string dataDirectory => dataPath + PiEditorParams.PiProjectDataFolder;
        public static string piResourcesDirectory => dataDirectory + ds + "Resources";
        public static string piPrefabPath => piResourcesDirectory + ds + "PiFramework.prefab";
        public static string moduleDirectory => dataPath + "PiModules";
        public static string settingDirectory => dataPath + "Settings";
        public static string scriptDirectory => dataDirectory + ds + "Scripts";

        /// <summary>
        /// Get full path of _PiProjectData folder
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public static string GetPiDataPath()
        {
            return Application.dataPath + "/" + PiEditorParams.PiProjectDataFolder;
        }

        /// <summary>
        /// Function exposing the private implementation of a handy filenaming function in UnityEditor.
        /// AssetDatabase with fallback to a public function. 
        /// Maybe I'll add some more handy private functions along the way.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetUniqueAssetPathNameOrFallback(string filename)
        {
            string path;
            try
            {
                // Private implementation of a filenaming function which puts the file at the selected path.
                System.Type assetdatabase = typeof(UnityEditor.AssetDatabase);
                path = (string)assetdatabase.GetMethod("GetUniquePathNameAtSelectedPath", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).Invoke(assetdatabase, new object[] { filename });
            }
            catch
            {
                // Protection against implementation changes.
                path = UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets" + "/" + filename);
            }
            return path;
        }

        /// <summary>
        /// Find Assets With FullName (include extension)
        /// (Bản mới của Unity sẽ support find with extension)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>return array of found path</returns>
        public static string[] FindAssetsWithFullName(string fileName)
        {
            var results = new List<string>();

            int lastIndex = fileName.LastIndexOf('.');
            var name = fileName.Substring(0, lastIndex);
            var ext = fileName.Substring(lastIndex);

            string[] guids = AssetDatabase.FindAssets(name);
            if (guids.Length > 0)
            {
                foreach (var guid in guids)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (Path.GetFileName(path).Equals(fileName))
                    {
                        results.Add(path);
                    }
                }
            }
            return results.ToArray();
        }

        /// <summary>
        /// Ignore if dest already existed
        /// Auto create directory
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <param name="desPath"></param>
        public static void FineAndCopyAsset(string uniqueName, string desPath)
        {
            if (File.Exists(desPath))
                return;

            var dir = Path.GetDirectoryName(desPath);
            Directory.CreateDirectory(dir);

            var files = FileHelper.FindAssetsWithFullName(uniqueName);
            if (files.Length == 0)
            {
                Debug.LogError("Not found " + uniqueName);
            }
            else
            {
                var path = files[0];
                FileUtil.CopyFileOrDirectory(path, desPath);
            }
        }

        /// <summary>
        /// find all path of a ScriptableObject in the Project of a Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Array of paths</returns>
        public static string[] FindScriptableObjects<T>() where T : ScriptableObject
        {
            string[] results = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = AssetDatabase.GUIDToAssetPath(results[i]);
            }

            return results;
        }
    }

}