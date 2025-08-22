// Assets/PF/Editor/Settings/SettingDefinitionEditor.cs
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using PF.PiEditor.UI;

namespace PF.PiEditor.Settings
{
    [CustomEditor(typeof(SettingDefinition))]
    public class SettingDefinitionEditor : Editor
    {
        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            var so = serializedObject;
            var def = (SettingDefinition)target;

            // --- Header Toolbar ---
            var bar = new Toolbar();

            var btnValidate = new ToolbarButton(() => ValidateCurrent()) { text = "Validate" };

            var btnApply = new ToolbarButton(() =>
            {
                SettingsGenerator.Generate();
                AssetDatabase.SaveAssetIfDirty(target);
                EditorUtility.DisplayDialog("Settings", "Generated code successfully.", "OK");
            })
            { text = "Apply (Generate)" };

            // To JSON
            var btnToJson = new ToolbarButton(() =>
            {
                var path = AssetDatabase.GetAssetPath(def);
                if (string.IsNullOrEmpty(path))
                {
                    EditorUtility.DisplayDialog("Export JSON", "Hãy lưu asset này ra đĩa trước (Save/Save Project).", "OK");
                    return;
                }
                var jsonPath = Path.ChangeExtension(path, ".json");
                try
                {
                    var json = JsonUtility.ToJson(def, true);
                    File.WriteAllText(jsonPath, json);
                    AssetDatabase.Refresh();
                    EditorUtility.DisplayDialog("Export JSON", $"Đã ghi: {jsonPath}", "OK");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    EditorUtility.DisplayDialog("Export JSON", "Ghi file thất bại. Xem Console để biết chi tiết.", "OK");
                }
            })
            { text = "To JSON" };

            // From JSON
            var btnFromJson = new ToolbarButton(() =>
            {
                var path = AssetDatabase.GetAssetPath(def);
                if (string.IsNullOrEmpty(path))
                {
                    EditorUtility.DisplayDialog("Import JSON", "Hãy lưu asset này ra đĩa trước (Save/Save Project).", "OK");
                    return;
                }
                var jsonPath = Path.ChangeExtension(path, ".json");
                if (!File.Exists(jsonPath))
                {
                    // Cho chọn file thủ công nếu chưa có file mặc định
                    var pick = EditorUtility.OpenFilePanel("Chọn file JSON để import", Path.GetDirectoryName(path), "json");
                    if (string.IsNullOrEmpty(pick)) return;
                    jsonPath = pick;
                }
                try
                {
                    var json = File.ReadAllText(jsonPath);
                    Undo.RecordObject(def, "Import Settings JSON");
                    JsonUtility.FromJsonOverwrite(json, def);
                    EditorUtility.SetDirty(def);
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("Import JSON", $"Đã nhập từ: {jsonPath}", "OK");
                }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                    EditorUtility.DisplayDialog("Import JSON", "Import thất bại. Xem Console để biết chi tiết.", "OK");
                }
            })
            { text = "From JSON" };

            bar.Add(btnValidate);
            bar.Add(new ToolbarSpacer());
            bar.Add(btnApply);
            bar.Add(new ToolbarSpacer());
            bar.Add(btnToJson);
            bar.Add(btnFromJson);
            root.Add(bar);

            // --- Default inspector (tận dụng property drawer đã tách) ---
            var body = new VisualElement();
            InspectorElement.FillDefaultInspector(body, so, this);
            root.Add(body);
            
            var hintText =
                "• Validate: kiểm tra asset hiện tại.\n" +
                "• Apply: sinh mã (AOT-safe).\n" +
                "• To JSON: xuất file .json cùng tên/cùng thư mục với .asset.\n" +
                "• From JSON: nhập lại từ file .json đó.";
            var hint = new NoticeBox(hintText, NoticeBox.Type.Info, fontSize: 15, bold: false);
                //.UseIconFromAsset("Assets/Packages/Pi Framework/Editor/Icons/about.png", 32);
            root.Add(hint);
            
            return root;
        }

        void ValidateCurrent()
        {
            var def = (SettingDefinition)target;
            if (string.IsNullOrWhiteSpace(def.Owner))
                Debug.LogWarning($"[Settings] '{def.name}' missing Owner. (Ignore if intentional)");
            if (string.IsNullOrWhiteSpace(def.PathPrefix))
                Debug.LogWarning($"[Settings] '{def.name}' missing PathPrefix. (Ignore if intentional)");

            def.ValidateEntries();

            var seen = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            foreach (var e in def.Entries)
            {
                var rel = e.RelativePath?.Trim();
                var full = string.IsNullOrWhiteSpace(def.PathPrefix) ? rel : $"{def.PathPrefix}.{rel}";
                if (!seen.Add(full))
                    Debug.LogWarning($"[Settings] Duplicate FullPath: {full}");
            }

            EditorUtility.DisplayDialog("Validate", "Validation finished. Check Console for warnings.", "OK");
        }
    }
}