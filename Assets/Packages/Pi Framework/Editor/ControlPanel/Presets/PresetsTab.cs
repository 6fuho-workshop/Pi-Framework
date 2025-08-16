// Package/Editor/ControlPanel/Presets/PresetsTab.cs
using PF.PiEditor.ControlPanel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace PF.PiEditor.ControlPanel.Presets
{
    /// <summary>API để module đăng ký loại ScriptableObject (preset type) vào tab Presets.</summary>
    public static class PresetTypeRegistry
    {
        public sealed class Entry
        {
            public Type SoType;
            public string DisplayName;
            public Func<ScriptableObject> CreateDefault; // cách tạo mặc định
            public int Order;
        }

        private static readonly List<Entry> _entries = new();

        public static void Register(Type soType, string displayName, Func<ScriptableObject> createDefault, int order = 0)
        {
            if (soType == null || !typeof(ScriptableObject).IsAssignableFrom(soType)) return;
            int idx = _entries.FindIndex(e => e.SoType == soType);
            var e = new Entry { SoType = soType, DisplayName = displayName ?? soType.Name, CreateDefault = createDefault, Order = order };
            if (idx >= 0) _entries[idx] = e; else _entries.Add(e);
            _entries.Sort((a, b) =>
            {
                int c = a.Order.CompareTo(b.Order);
                return c != 0 ? c : string.Compare(a.DisplayName, b.DisplayName, StringComparison.Ordinal);
            });
        }

        internal static IReadOnlyList<Entry> Entries => _entries;
    }

    /// <summary>Provider để đăng ký tab Presets vào Control Panel.</summary>
    public sealed class PresetsTabProvider : ITabProvider
    {
        public string Id => "pf.presets";
        public string Title => "Presets";
        public int Order => 0;
        public ITab Create() => new PresetsTab();
    }

    /// <summary>Tab Presets – quản lý mọi ScriptableObject config theo loại đã đăng ký</summary>
    public sealed class PresetsTab : ITab
    {
        public string Id => "pf.presets";
        public string Title => "Presets";

        private Vector2 _left;
        private Vector2 _right;
        private string _search = "";

        // per-type foldout
        private readonly Dictionary<Type, bool> _fold = new();
        // current selection
        private UnityEngine.Object _activeObj;
        private Editor _activeEditor;
        private string _renameBuffer; // null => không rename

        public void OnEnable() { }
        public void OnDisable()
        {
            if (_activeEditor != null) UnityEngine.Object.DestroyImmediate(_activeEditor);
            _activeEditor = null;
        }
        public void Dispose() => OnDisable();

        public void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                DrawLeft(280);
                DrawRight();
            }
        }

        private void DrawLeft(float width)
        {
            using (new EditorGUILayout.VerticalScope(GUILayout.Width(width)))
            {
                // Toolbar
                using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
                {
                    _search = GUILayout.TextField(
                        _search,
                        GUI.skin.FindStyle("ToolbarSeachTextField") ?? EditorStyles.toolbarTextField,
                        GUILayout.Width(160)
                    );
                    if (GUILayout.Button("×", EditorStyles.toolbarButton, GUILayout.Width(22))) _search = "";
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Refresh", EditorStyles.toolbarButton, GUILayout.Width(70)))
                        RepaintInspectors();
                }

                EditorGUILayout.Space(4);
                _left = EditorGUILayout.BeginScrollView(_left);

                foreach (var e in PresetTypeRegistry.Entries)
                {
                    if (!_fold.ContainsKey(e.SoType)) _fold[e.SoType] = true;

                    using (new EditorGUILayout.VerticalScope("box"))
                    {
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            _fold[e.SoType] = EditorGUILayout.Foldout(_fold[e.SoType], e.DisplayName, true);
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Create", GUILayout.Width(70)))
                            {
                                CreatePreset(e);
                            }
                        }

                        if (_fold[e.SoType])
                        {
                            // find assets of this type
                            var guids = AssetDatabase.FindAssets($"t:{e.SoType.Name}");
                            foreach (var g in guids)
                            {
                                var path = AssetDatabase.GUIDToAssetPath(g);
                                var obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(path);
                                if (obj == null) continue;

                                // filter
                                if (!string.IsNullOrEmpty(_search))
                                {
                                    if (!obj.name.Contains(_search, StringComparison.OrdinalIgnoreCase) &&
                                        !path.Contains(_search, StringComparison.OrdinalIgnoreCase))
                                        continue;
                                }

                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    bool isActive = _activeObj == obj;
                                    var style = isActive ? EditorStyles.boldLabel : EditorStyles.label;
                                    if (GUILayout.Button(obj.name, style, GUILayout.ExpandWidth(true)))
                                        Select(obj);

                                    if (GUILayout.Button("Delete", GUILayout.Width(56)))
                                    {
                                        if (EditorUtility.DisplayDialog("Delete preset?",
                                            $"Delete asset:\n{path} ?", "Delete", "Cancel"))
                                        {
                                            AssetDatabase.DeleteAsset(path);
                                            AssetDatabase.SaveAssets();
                                            if (_activeObj == obj) Select(null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.EndScrollView();
            }
        }

        private void DrawRight()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                if (_activeObj == null)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.HelpBox(
                        "Chọn một preset ở panel trái để xem và chỉnh. Bạn có thể dùng nút Create trong từng nhóm loại.",
                        MessageType.Info);
                    GUILayout.FlexibleSpace();
                    return;
                }

                var path = AssetDatabase.GetAssetPath(_activeObj);

                _right = EditorGUILayout.BeginScrollView(_right);

                // Header: Name + Rename + Reveal
                using (new EditorGUILayout.VerticalScope())
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (_renameBuffer == null)
                        {
                            EditorGUILayout.LabelField(_activeObj.name, EditorStyles.boldLabel);
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Rename", GUILayout.Width(70)))
                                _renameBuffer = Path.GetFileNameWithoutExtension(path);

                            if (GUILayout.Button("Reveal", GUILayout.Width(60)))
                            {
                                Selection.activeObject = _activeObj;
                                EditorGUIUtility.PingObject(_activeObj);
                            }
                        }
                        else
                        {
                            _renameBuffer = EditorGUILayout.TextField("New Name", _renameBuffer, GUILayout.MinWidth(220));
                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Apply", GUILayout.Width(70)))
                            {
                                if (!string.IsNullOrWhiteSpace(_renameBuffer))
                                {
                                    AssetDatabase.RenameAsset(path, _renameBuffer.Trim());
                                    AssetDatabase.SaveAssets();
                                    _renameBuffer = null;
                                }
                            }
                            if (GUILayout.Button("Cancel", GUILayout.Width(70))) _renameBuffer = null;
                        }
                    }

                    // show path dưới tên
                    EditorGUILayout.LabelField(path, EditorStyles.miniLabel);
                }

                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                // Inspector embed
                if (_activeEditor != null)
                    _activeEditor.OnInspectorGUI();

                EditorGUILayout.EndScrollView();
            }
        }

        private void CreatePreset(PresetTypeRegistry.Entry e)
        {
            // chọn chỗ lưu
            string defaultDir = $"Assets/PF/Configs/{e.DisplayName.Replace(' ', '_')}";
            EnsureFolders(defaultDir);

            string path = EditorUtility.SaveFilePanelInProject(
                $"Create {e.DisplayName}",
                e.SoType.Name,
                "asset",
                $"Chọn nơi lưu asset {e.DisplayName}.",
                defaultDir
            );
            if (string.IsNullOrEmpty(path)) return;

            var so = e.CreateDefault?.Invoke() ?? ScriptableObject.CreateInstance(e.SoType) as ScriptableObject;
            if (so == null)
            {
                EditorUtility.DisplayDialog("Create failed", $"Không thể tạo instance cho {e.DisplayName}.", "Close");
                return;
            }

            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Select(so);
        }

        private static void EnsureFolders(string leaf)
        {
            // tạo folder kiểu "Assets/AAA/BBB"
            var parts = leaf.Split('/');
            string cur = "";
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == 0) { cur = parts[0]; continue; }  // "Assets"
                string parent = string.Join("/", parts.Take(i));
                if (!AssetDatabase.IsValidFolder($"{parent}/{parts[i]}"))
                    AssetDatabase.CreateFolder(parent, parts[i]);
            }
        }

        private void Select(UnityEngine.Object obj)
        {
            _activeObj = obj;

            if (_activeEditor != null) UnityEngine.Object.DestroyImmediate(_activeEditor);
            _activeEditor = null;

            if (_activeObj != null)
                _activeEditor = Editor.CreateEditor(_activeObj); // dùng CustomEditor sẵn có
        }

        private void RepaintInspectors()
        {
            // chỉ cần repaint; danh sách asset tự query lại mỗi frame
            EditorWindow.focusedWindow?.Repaint();
        }
    }

    // Tự đăng ký tab Presets vào Control Panel khi domain reload
    [InitializeOnLoad]
    public static class PresetsTabAutoRegister
    {
        static PresetsTabAutoRegister()
        {
            ControlPanelTabs.Register(new PresetsTabProvider());
        }
    }
}
