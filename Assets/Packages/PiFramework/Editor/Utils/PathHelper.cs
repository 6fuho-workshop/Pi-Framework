using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PF.PiEditor.Utils
{
    // lưu ý ko dùng const
    public class PathHelper
    {
        /// <summary>
        /// Application.dataPath => D:/MyProject/Assets
        /// </summary>
        public static readonly string ProjectDataAbs = Application.dataPath;
        /// <summary>
        ///  project root => D:/MyProject (bỏ /Assets)
        /// </summary>
        public static readonly string ProjectRootAbs = ProjectDataAbs[..(Application.dataPath.LastIndexOf('/'))];

        /// <summary>
        /// _PiProjectData
        /// </summary>
        public static readonly string PiDataDir = "Assets/_PiProjectData";
        public static readonly string PiDataResourcesDir = PiDataDir + "/Resources";
        public static readonly string PiDataScriptsDir = PiDataDir + "/Scripts";
        public static readonly string PiDataEditorDir = PiDataDir + "/Editor";
        public static readonly string PiDataCacheDir = PiDataDir + "/Cache";

        public static readonly string SettingsDir = "Assets/Settings";
        public static readonly string ModulesDir = "Assets/PiModules";

        public static readonly string PiPrefabPath = PiDataResourcesDir + "/PiFramework.prefab";

        /// <summary>
        /// relativePath tính từ ProjectRoot: "Assets/SomeFolder/SomeFile.txt"
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static string AbsolutePath(string relativePath)
        {
            return $"{ProjectRootAbs}/{relativePath}";
        }

        internal static void CreateDirectories()
        {
            Directory.CreateDirectory(PiDataResourcesDir);
            Directory.CreateDirectory(PiDataCacheDir);
            Directory.CreateDirectory(PiDataScriptsDir);
            Directory.CreateDirectory(PiDataEditorDir);
            Directory.CreateDirectory(SettingsDir);
            Directory.CreateDirectory(ModulesDir);

        }

        /// <summary>
        /// Lấy folder hiện hành trong Project window.
        /// - Nếu đang chọn folder -> trả về folder đó.
        /// - Nếu đang chọn asset -> trả về thư mục chứa asset.
        /// - Nếu không chọn gì -> "Assets".
        /// </summary>
        public static string GetCurrentProjectFolder()
        {
            var obj = Selection.activeObject;
            if (obj == null) return "Assets";

            var path = AssetDatabase.GetAssetPath(obj);
            if (string.IsNullOrEmpty(path)) return "Assets";

            if (AssetDatabase.IsValidFolder(path))
                return path; // đang chọn 1 folder

            // đang chọn 1 asset -> lấy folder chứa nó
            var dir = Path.GetDirectoryName(path)?.Replace("\\", "/");
            return string.IsNullOrEmpty(dir) ? "Assets" : dir!;
        }
    }
}