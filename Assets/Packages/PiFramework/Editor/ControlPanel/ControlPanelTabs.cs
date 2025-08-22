// Package/Editor/ControlPanel/PFControlPanelTabs.cs
using System;
using System.Collections.Generic;
using UnityEditor;

namespace PF.PiEditor.ControlPanel
{
    /// <summary>Factory provider để khởi tạo tab. Cho phép module đăng ký mà không giữ instance sống qua domain reload.</summary>
    public interface ITabProvider
    {
        string Id { get; }
        string Title { get; }
        int Order { get; }            // sắp xếp tab trái→phải
        ITab Create();              // tạo instance tab khi cần
    }

    /// <summary>Contract cho một tab trong Control Panel.</summary>
    public interface ITab : IDisposable
    {
        string Id { get; }
        string Title { get; }
        void OnEnable();
        void OnDisable();
        void OnGUI();                 // vẽ nội dung tab
    }

    /// <summary>Registry toàn cục cho các Tab của PF Control Panel.</summary>
    public static class ControlPanelTabs
    {
        private static readonly List<ITabProvider> _providers = new();

        /// <summary>Module gọi hàm này ở Editor thời điểm init để thêm tab mới.</summary>
        public static void Register(ITabProvider provider)
        {
            if (provider == null) return;
            // tránh trùng Id
            int idx = _providers.FindIndex(p => p.Id == provider.Id);
            if (idx >= 0) _providers[idx] = provider; else _providers.Add(provider);
            _providers.Sort((a, b) =>
            {
                int c = a.Order.CompareTo(b.Order);
                return c != 0 ? c : string.Compare(a.Title, b.Title, StringComparison.Ordinal);
            });
        }

        internal static IReadOnlyList<ITabProvider> Providers => _providers;
    }
}