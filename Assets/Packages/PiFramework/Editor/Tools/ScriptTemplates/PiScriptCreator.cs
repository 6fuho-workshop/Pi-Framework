using UnityEditor;
using UnityEngine;
using PF.PiEditor.Utils;

public static class PiScriptCreator
{
    
    [MenuItem("▶ Pi ◀/Create Script/Assembly-CSharp Script", priority = -10)]
    public static void CreateAssemblyCSharp() => Create("Pi_CSharpMonoScript.cs.txt", "NewMono.cs");

    [MenuItem("▶ Pi ◀/Create Script/Default MonoBehaviour", priority = -9)]
    public static void CreateDefaultMono() => Create("Pi_DefaultMonoScript.cs.txt", "NewMono.cs");

    [MenuItem("▶ Pi ◀/Create Script/Default ScriptableObject", priority = -8)]
    public static void CreateDefaultSO() => Create("Pi_DefaultSOScript.cs.txt", "NewDataType.cs");


    /// Tạo script .cs từ template tại folder hiện hành (hoặc /Assets nếu không có).
    private static void Create(string templateFileName, string suggestedName)
    {
        var folder = PathHelper.GetCurrentProjectFolder();

        // Đặt selection về đúng folder đích để Unity tạo file tại đó
        var folderAsset = AssetDatabase.LoadAssetAtPath<Object>(folder);
        Selection.activeObject = folderAsset;

        var templatePath = AssetHelper.FindFirstAssetWithFullName(templateFileName);
        if (string.IsNullOrEmpty(templatePath))
        {
            Debug.LogError($"[PF] Không tìm thấy template: {templateFileName}. Vui lòng kiểm tra lại đường dẫn.");
            return;
        }

        // Gợi ý tên file; Unity sẽ bật ô rename ngay sau khi tạo
        ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, suggestedName);

        if (folder == "Assets")
            Debug.Log("[PF] Không có folder đang chọn. Script sẽ được tạo tại Assets/.");
    }
}
