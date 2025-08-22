// Assets/PF/Editor/Diagnostics/LogCategoryTreeWindow.cs
using PF.Contracts;
using PF.Logging;
using PF.Logging.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LogCategoryTreeWindow : EditorWindow
{
    [MenuItem("▶ Pi ◀/Logging")]
    public static void Open() => GetWindow<LogCategoryTreeWindow>("Pi Logging");

    private Vector2 _leftScroll;
    private Vector2 _rightScroll;

    private readonly List<string> _guids = new();
    private readonly List<UnityEngine.Object> _assets = new();
    private int _activeIndex = -1;
    private Editor _rightEditor;

    private string _search = "";
    private string _renameBuffer = null;  // null => không show UI rename

    private void OnEnable() => RefreshList();

    private void OnDisable()
    {
        if (_rightEditor != null) DestroyImmediate(_rightEditor);
    }

    private void RefreshList()
    {
        _guids.Clear();
        _assets.Clear();
        _activeIndex = Mathf.Clamp(_activeIndex, -1, int.MaxValue);

        foreach (var guid in AssetDatabase.FindAssets("t:LogCategoryTree"))
        {
            _guids.Add(guid);
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
            if (obj != null) _assets.Add(obj);
        }

        Repaint();
    }

    private void SelectIndex(int idx)
    {
        if (_activeIndex == idx) return;
        _activeIndex = idx;
        _renameBuffer = null; // huỷ mode rename khi chuyển asset

        if (_rightEditor != null)
        {
            DestroyImmediate(_rightEditor);
            _rightEditor = null;
        }
        if (_activeIndex >= 0 && _activeIndex < _assets.Count)
        {
            _rightEditor = Editor.CreateEditor(_assets[_activeIndex]); // dùng CustomEditor sẵn có
        }
    }

    private void OnGUI()
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            DrawLeftPane(260);
            DrawRightPane();
        }
    }

    private void DrawLeftPane(float width)
    {
        using (new EditorGUILayout.VerticalScope(GUILayout.Width(width)))
        {
            // Toolbar (Refresh / Create / Search)
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
                    RefreshList();

                GUILayout.Space(4);
                if (GUILayout.Button("Create", EditorStyles.toolbarButton, GUILayout.Width(60)))
                    CreateNewAsset();

                GUILayout.FlexibleSpace();
                _search = GUILayout.TextField(
                    _search,
                    GUI.skin.FindStyle("ToolbarSeachTextField") ?? EditorStyles.toolbarTextField,
                    GUILayout.Width(140)
                );
                if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(20)))
                    _search = "";
            }

            EditorGUILayout.Space(4);

            // List assets (không có file path, không có nút ▶)
            _leftScroll = EditorGUILayout.BeginScrollView(_leftScroll);
            for (int i = 0; i < _assets.Count; i++)
            {
                var obj = _assets[i];
                var path = AssetDatabase.GUIDToAssetPath(_guids[i]);
                var name = obj != null ? obj.name : "(Missing)";

                if (!string.IsNullOrEmpty(_search) &&
                    !name.Contains(_search, StringComparison.OrdinalIgnoreCase) &&
                    !path.Contains(_search, StringComparison.OrdinalIgnoreCase))
                    continue;

                using (new EditorGUILayout.VerticalScope("box"))
                using (new EditorGUILayout.HorizontalScope())
                {
                    bool isActive = (i == _activeIndex);
                    var style = isActive ? EditorStyles.boldLabel : EditorStyles.label;
                    if (GUILayout.Button(name, style, GUILayout.ExpandWidth(true)))
                        SelectIndex(i);
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }

    private void DrawRightPane()
    {
        using (new EditorGUILayout.VerticalScope())
        {
            if (_activeIndex < 0 || _activeIndex >= _assets.Count || _assets[_activeIndex] == null)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox(
                    "Chọn một LogCategoryTree ở cột trái để cấu hình.\n" +
                    "Bạn cũng có thể bấm Create để tạo mới.",
                    MessageType.Info);
                GUILayout.FlexibleSpace();
                return;
            }

            _rightScroll = EditorGUILayout.BeginScrollView(_rightScroll);

            // Header + inline rename
            var activePath = AssetDatabase.GUIDToAssetPath(_guids[_activeIndex]);
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (_renameBuffer == null)
                    {
                        // Header thường: Name + Rename + Reveal (giữ lại Reveal ở panel phải)
                        EditorGUILayout.LabelField(_assets[_activeIndex].name, EditorStyles.boldLabel);
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Rename", GUILayout.Width(70)))
                        {
                            _renameBuffer = Path.GetFileNameWithoutExtension(activePath);
                        }

                        if (GUILayout.Button("Reveal", GUILayout.Width(60)))
                        {
                            EditorGUIUtility.PingObject(_assets[_activeIndex]);
                            Selection.activeObject = _assets[_activeIndex];
                        }
                    }
                    else
                    {
                        // UI rename inline
                        _renameBuffer = EditorGUILayout.TextField("New Name", _renameBuffer, GUILayout.MinWidth(220));
                        GUILayout.FlexibleSpace();

                        if (GUILayout.Button("Apply", GUILayout.Width(70)))
                        {
                            if (!string.IsNullOrWhiteSpace(_renameBuffer))
                            {
                                AssetDatabase.RenameAsset(activePath, _renameBuffer.Trim());
                                AssetDatabase.SaveAssets();
                                RefreshList();
                                int idx = _guids.FindIndex(g => AssetDatabase.GUIDToAssetPath(g) == activePath);
                                if (idx >= 0) SelectIndex(idx);
                            }
                            _renameBuffer = null;
                        }

                        if (GUILayout.Button("Cancel", GUILayout.Width(70)))
                        {
                            _renameBuffer = null;
                        }
                    }
                }

                // File path hiển thị NGAY DƯỚI tên file
                EditorGUILayout.LabelField(activePath, EditorStyles.miniLabel);
            }

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            // Embedded inspector (LogCategoryTreeInspector với Validate/Apply)
            if (_rightEditor != null)
                _rightEditor.OnInspectorGUI();
            // (Apply sẽ gọi LogCategoryApplier.Apply(...).) 

            EditorGUILayout.EndScrollView();
        }
    }

    private void CreateNewAsset()
    {
        // Gợi ý thư mục mặc định
        string defaultDir = "Assets/PF/Configs/Logging";
        if (!AssetDatabase.IsValidFolder("Assets/PF")) AssetDatabase.CreateFolder("Assets", "PF");
        if (!AssetDatabase.IsValidFolder("Assets/PF/Configs")) AssetDatabase.CreateFolder("Assets/PF", "Configs");
        if (!AssetDatabase.IsValidFolder(defaultDir)) AssetDatabase.CreateFolder("Assets/PF/Configs", "Logging");

        string path = EditorUtility.SaveFilePanelInProject(
            "Create LogCategoryTree",
            "LogCategoryTree",
            "asset",
            "Chọn nơi lưu ScriptableObject LogCategoryTree mới.",
            defaultDir
        );
        if (string.IsNullOrEmpty(path)) return;

        var so = ScriptableObject.CreateInstance<LogCategoryTree>();
        so.Root = new LogCategoryTree.Node
        {
            Name = "PF",
            Level = LogLevel.Inherit,
            Children = new List<LogCategoryTree.Node>()
        };

        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        RefreshList();

        int idx = _guids.FindIndex(g => AssetDatabase.GUIDToAssetPath(g) == path);
        if (idx >= 0) SelectIndex(idx);
    }
}