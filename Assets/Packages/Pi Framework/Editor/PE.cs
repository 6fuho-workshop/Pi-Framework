using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.Compilation;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PiEditor
{
    
    public class PE
    {
        static PE()
        {
            //Debug.Log("PE static ctor");
        }

        public static void Recompile()
        {
            //Dùng EditorUtility.RequestScriptReload() không thích hợp vì nó không compile changed scripts
            CompilationPipeline.RequestScriptCompilation();

            //Gọi Refresh sau khi Compile nếu không Unity sẽ tự gọi nó khi switch sang app khác rồi quay lại Unity
            AssetDatabase.Refresh();
        }
    }
}