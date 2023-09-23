using UnityEngine;
using UnityEditor;
using PiFramework;
using PiEditor;
using PiEditor.Utils;
using System.IO;
using System.Collections;

namespace PiEditor
{
    public class MenuTool : MonoBehaviour
    {

        /// <summary>
        /// tạo framework game object nếu chưa có trong scene hiện tại
        /// </summary>
        [MenuItem("Pi/Attach Framework")]
        static void AttachFramework()
        {
            if (GameObject.FindObjectOfType<PiRoot>() == null)
            {
                var go = new GameObject("PiFramework");
                go.AddComponent<PiRoot>().hideFlags = HideFlags.NotEditable;
                //go.hideFlags = HideFlags.NotEditable;
            }

        }
        /* đang thay đổi
        [MenuItem("Pi/Create/Create Config File")]
        public static void CreateConfigFile()
        {
            PiSetting asset = ScriptableObject.CreateInstance<PiSetting>();

            //var path = FileHelper.GetMetaDataPath() + "/Resources/DefaultConfig.asset";
            var suffix = "";
            var count = 0;
            while (File.Exists(FileHelper.GetPiAppDataPath() + "/Resources/DefaultConfig" + suffix + ".asset"))
            {
                count++;
                suffix = " " + count.ToString();
            }

            var path = "Assets/" + PiEditorParams.PiProjectDataFolder + "/Resources/DefaultConfig" + suffix + ".asset";


            //AssetDatabase.CreateAsset(asset, FileHelper.GetUniqueAssetPathNameOrFallback(path));
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        */

        [MenuItem("Pi/Toggle PiProfiler %#o")]
        static void TogglePiProfiler()
        {
            PiProfiler profiler = PiServiceLocator.instance.GetService<PiProfiler>();
            profiler.Enabled = !profiler.Enabled;
        }

        [MenuItem("Pi/Toggle PiProfiler %#o", true)]
        static bool ValidateTogglePiProfiler()
        {
            return Application.isPlaying;
        }
    }
}