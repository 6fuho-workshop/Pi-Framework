using PiEditor.Callbacks;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiEditor
{
    /// InitializeOnLoad: Allows you to initialize an Editor class when Unity loads, and when your scripts are recompiled.
    /// Static constructors with this attribute are called when scripts in the project are recompiled 
    /// (also known as a Domain Reload). This happens when Unity first loads your project, but also when Unity detects 
    /// modifications to scripts (depending on your Auto Refresh preferences), 
    /// and when you enter Play Mode (depending on your Play Mode configuration).

    [InitializeOnLoad]
    public class PE
    {
        static bool _recompile;
        static PE()
        {
            var t = EditorApplication.timeSinceStartup;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            PiEditorBootstrap.Bootstrap();
            InvokeOnLoadCallbacks();

            t = EditorApplication.timeSinceStartup - t;
            //Debug.Log("Pi Editor ctor: " + t + "s");
        }

        static void InvokeOnLoadCallbacks()
        {
            var callbacks = TypeCache.GetMethodsWithAttribute<OnLoadPiEditorAttribute>();
            foreach (var callback in callbacks)
            {
                callback.Invoke(null, null);
            }
        }

        public static void Recompile()
        {
            _recompile = true;
        }

        static void RecompileImmediate()
        {
            _recompile = false;
            //Dùng EditorUtility.RequestScriptReload() không thích hợp vì nó không compile changed scripts
            CompilationPipeline.RequestScriptCompilation();

            //Gọi Refresh sau khi Compile nếu không Unity sẽ tự gọi nó khi switch sang app khác rồi quay lại Unity
            AssetDatabase.Refresh();
        }

        static void Update()
        {
            if (_recompile)
                RecompileImmediate();
        }
    }
}