// Package/Editor/Diagnostics/LogCategoryTreeInspector.cs
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PF.Logging; // LogLevel

[CustomEditor(typeof(LogCategoryTree))]
public class LogCategoryTreeEditor : Editor
{
    private static readonly Dictionary<LogCategoryTree.Node, bool> _fold = new();
    private readonly List<Action> _postOps = new();
    private LogLevel _applyDefault = LogLevel.Error;

    public override void OnInspectorGUI()
    {
        var asset = (LogCategoryTree)target;

        if (asset.Root == null)
        {
            if (GUILayout.Button("Create Root"))
            {
                Undo.RecordObject(asset, "Create Root");
                asset.Root = new LogCategoryTree.Node { Name = "PF", Level = LogLevel.Default };
                EditorUtility.SetDirty(asset);
            }
            return;
        }

        EditorGUILayout.LabelField("Log Category Tree (IMGUI)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        _postOps.Clear();
        DrawNode(asset, parent: null, asset.Root, "");
        if (_postOps.Count > 0)
        {
            Undo.RecordObject(asset, "Edit Tree");
            foreach (var op in _postOps) op();
            EditorUtility.SetDirty(asset);
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // --- VALIDATE: tách riêng, đưa lên trên ---
        if (GUILayout.Button("Validate", GUILayout.Width(120)))
        {
            var issues = ValidateTree(asset);
            EditorUtility.DisplayDialog(
                "Validation",
                issues.Count == 0 ? "OK!" : string.Join("\n", issues),
                "Close"
            );
        }

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        // --- APPLY: có mô tả + chỉ mở khi PlayMode ---
        EditorGUILayout.LabelField("Apply to Runtime", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Apply sẽ đẩy các mức log từ cây này vào Runtime Logger theo luật 'longest-prefix wins'. " +
            "'Default on Apply' dùng làm fallback cho các node có Level = Default. " +
            "Thao tác chỉ khả dụng trong Play Mode và ảnh hưởng session đang chạy.",
            MessageType.Info
        );

        using (new EditorGUI.DisabledGroupScope(!EditorApplication.isPlaying))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _applyDefault = (LogLevel)EditorGUILayout.EnumPopup("Default on Apply", _applyDefault);

                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Apply (Longest-prefix)", GUILayout.Width(180)))
                {
                    LogCategoryApplier.Apply(asset, _applyDefault);
                    EditorUtility.DisplayDialog(
                        "Apply",
                        "Applied overrides to runtime logging (Longest-prefix).",
                        "Close"
                    );
                }
            }
        }
    }

    private void DrawNode(LogCategoryTree asset, LogCategoryTree.Node parent, LogCategoryTree.Node node, string parentPath)
    {
        if (!_fold.ContainsKey(node)) _fold[node] = true;

        using (new EditorGUILayout.VerticalScope(GUI.skin.box))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                _fold[node] = EditorGUILayout.Foldout(_fold[node], GUIContent.none, true, EditorStyles.foldout);

                // Name
                var newName = EditorGUILayout.TextField(node.Name);
                if (!string.Equals(newName, node.Name, StringComparison.Ordinal))
                {
                    Undo.RecordObject(asset, "Rename Category");
                    node.Name = newName.Trim();
                    EditorUtility.SetDirty(asset);
                }

                // Level
                var newLevel = (LogLevel)EditorGUILayout.EnumPopup(node.Level, GUILayout.Width(110));
                if (newLevel != node.Level)
                {
                    Undo.RecordObject(asset, "Change Log Level");
                    node.Level = newLevel;
                    EditorUtility.SetDirty(asset);
                }

                // Buttons: +Child, Delete (đã bỏ +Sibling)
                if (GUILayout.Button("+Child", GUILayout.Width(60)))
                {
                    _postOps.Add(() =>
                    {
                        node.Children ??= new List<LogCategoryTree.Node>();
                        node.Children.Add(new LogCategoryTree.Node
                        {
                            Name = "NewNode",
                            Level = LogLevel.Default,
                            Children = new List<LogCategoryTree.Node>()
                        });
                        _fold[node] = true;
                    });
                }

                if (parent != null)
                {
                    if (GUILayout.Button("Delete", GUILayout.Width(60)))
                    {
                        _postOps.Add(() => { parent.Children.Remove(node); });
                        return;
                    }
                }
            }

            // Bỏ hiển thị "Path: fullpath" cho gọn
            var path = string.IsNullOrEmpty(parentPath) ? node.Name : $"{parentPath}.{node.Name}";

            if (_fold[node] && node.Children != null)
            {
                EditorGUI.indentLevel++;
                for (int i = 0; i < node.Children.Count; i++)
                {
                    var child = node.Children[i];
                    DrawNode(asset, node, child, path);
                }
                EditorGUI.indentLevel--;
            }
        }
    }

    private static List<string> ValidateTree(LogCategoryTree tree)
    {
        var errs = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);

        void Walk(LogCategoryTree.Node n, string parentPath)
        {
            if (n == null) { errs.Add("Null node"); return; }
            if (string.IsNullOrWhiteSpace(n.Name)) { errs.Add("Empty node name"); return; }
            if (n.Name.Contains(" ")) errs.Add($"Space in name: {n.Name}");
            if (n.Name.Contains("/")) errs.Add($"Slash in name: {n.Name}");

            var path = string.IsNullOrEmpty(parentPath) ? n.Name : $"{parentPath}.{n.Name}";
            if (!seen.Add(path)) errs.Add($"Duplicate path: {path}");

            if (n.Children != null)
                foreach (var c in n.Children) Walk(c, path);
        }

        if (tree.Root == null) errs.Add("Root is null");
        else Walk(tree.Root, "");
        return errs;
    }
}
