// Assets/PF/Core/Diagnostics/Config/LogCategoryTree.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using PF.Logging.Internal;
using PF.Contracts;

namespace PF.Logging
{
    [CreateAssetMenu(menuName = "▶ Pi ◀/Log Category Tree", fileName = "LogCategoryTree", order = 10)]
    public sealed class LogCategoryTree : ScriptableObject
    {
        [Serializable]
        public sealed class Node
        {
            public string Name;                       // "PF.Core" hoặc "Bootstrap"
            public LogLevel Level = LogLevel.Inherit; // Default => bỏ qua, không tạo rule
            public List<Node> Children = new();
        }

        public Node Root;

        // Dev hot-patch (Editor/Playmode)
        public event Action Changed;
#if UNITY_EDITOR
        private void OnValidate() => Changed?.Invoke();
#endif

        // Flatten thành map prefix -> level (bỏ Default)
        public Dictionary<string, LogLevel> BuildOverrides()
        {
            var dict = new Dictionary<string, LogLevel>(128, StringComparer.Ordinal);
            if (Root != null) Walk(Root, null, dict);
            return dict;
        }

        private static void Walk(Node n, string parent, Dictionary<string, LogLevel> outMap)
        {
            if (n == null || string.IsNullOrWhiteSpace(n.Name)) return;

            var path = string.IsNullOrEmpty(parent) ? n.Name : $"{parent}.{n.Name}";

            // Chỉ ghi rule khi KHÁC Default
            if (n.Level != LogLevel.Inherit)
                outMap[path] = n.Level; // cho phép cả None, Debug, Verbose, ...

            // Duyệt con
            if (n.Children != null)
                for (int i = 0; i < n.Children.Count; i++)
                    Walk(n.Children[i], path, outMap);
        }
    }
}