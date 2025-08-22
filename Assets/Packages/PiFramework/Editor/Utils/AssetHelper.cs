using UnityEngine;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

namespace PF.PiEditor.Utils
{
    public class AssetHelper
    {

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

        public static string FindFirstAssetWithFullName(string fileName)
        {
            var files = AssetHelper.FindAssetsWithFullName(fileName);
            if (files.Length == 0)
                return string.Empty;
            else
                return files[0];
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
            {
                Debug.LogWarning("File already exists: " + desPath);
                return;
            }

            var dir = Path.GetDirectoryName(desPath);
            Directory.CreateDirectory(dir);

            var file = FindFirstAssetWithFullName(uniqueName);
            if (string.IsNullOrEmpty(file))
                Debug.LogError("Not found " + uniqueName);
            else
                FileUtil.CopyFileOrDirectory(file, desPath);
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