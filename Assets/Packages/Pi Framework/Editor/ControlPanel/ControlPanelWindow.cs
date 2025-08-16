// Package/Editor/ControlPanel/ControlPanelWindow.cs
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PF.PiEditor.ControlPanel
{
    public class ControlPanelWindow : EditorWindow
    {
        [MenuItem("Pi/Control Panel")]
        public static void Open() => GetWindow<ControlPanelWindow>("PF Control Panel");

        private readonly Dictionary<string, ITab> _aliveTabs = new();
        private int _activeTabIndex = 0;
        private Vector2 _scroll;

        private void OnEnable()
        {
            // Recreate active tab instances
            EnsureActiveTabCreated();
        }

        private void OnDisable()
        {
            foreach (var t in _aliveTabs.Values)
            {
                try { t.OnDisable(); t.Dispose(); } catch { /* swallow */ }
            }
            _aliveTabs.Clear();
        }

        private void EnsureActiveTabCreated()
        {
            var providers = ControlPanelTabs.Providers;
            if (providers.Count == 0) return;

            _activeTabIndex = Mathf.Clamp(_activeTabIndex, 0, providers.Count - 1);
            var p = providers[_activeTabIndex];

            if (!_aliveTabs.TryGetValue(p.Id, out var tab) || tab == null)
            {
                tab = p.Create();
                _aliveTabs[p.Id] = tab;
                tab.OnEnable();
            }
        }

        private void OnGUI()
        {
            var providers = ControlPanelTabs.Providers;
            if (providers.Count == 0)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox("Chưa có tab nào được đăng ký. Hãy dùng PFControlPanelTabs.Register(...) trong các module Editor.", MessageType.Info);
                GUILayout.FlexibleSpace();
                return;
            }

            // --- Top level tabs ---
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                for (int i = 0; i < providers.Count; i++)
                {
                    bool on = i == _activeTabIndex;
                    if (GUILayout.Toggle(on, providers[i].Title, EditorStyles.toolbarButton) != on)
                    {
                        // chuyển tab
                        _activeTabIndex = i;
                        EnsureActiveTabCreated();
                        Repaint();
                    }
                }

                GUILayout.FlexibleSpace();
            }

            // --- Tab body scroll ---
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            {
                var p = providers[_activeTabIndex];
                if (_aliveTabs.TryGetValue(p.Id, out var tab) && tab != null)
                {
                    tab.OnGUI();
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}