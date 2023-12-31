﻿using UnityEngine;
using PiEditor;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace PiEditor.Utils
{
    public class FileHelper
    {
        /// <summary>
        /// Get full path of _PiProjectData folder
        /// </summary>
        /// <returns></returns>
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