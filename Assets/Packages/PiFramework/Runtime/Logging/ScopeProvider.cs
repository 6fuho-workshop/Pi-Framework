using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

#nullable enable
namespace PF.Logging.Internal
{
    // ------------- Scope provider (AsyncLocal) -------------
    // Flow:
    // - BeginScope push dictionary vào stack của thread async-current.
    // - Khi ghi log & qua filter -> snapshot gộp các scope hiện hành.
    // - Dispose scope -> pop.
    public static class ScopeProvider
    {
        private static readonly AsyncLocal<Stack<Dictionary<string, object>>?> _scopes = new();

        public static IDisposable BeginScope(IReadOnlyDictionary<string, object> kvs)
        {
            _scopes.Value ??= new Stack<Dictionary<string, object>>();
            var dict = new Dictionary<string, object>(kvs);
            _scopes.Value.Push(dict);
            return new ScopeGuard(_scopes);
        }

        public static IDisposable BeginScope(string key, object value)
            => BeginScope(new Dictionary<string, object> { [key] = value });

        public static IReadOnlyDictionary<string, object>? CurrentSnapshotOrNull()
        {
            var st = _scopes.Value;
            if (st == null || st.Count == 0) return null;

            // Gộp shallow từ dưới lên; key trên cùng ghi đè.
            var merged = new Dictionary<string, object>(Math.Min(8, st.Count * 2));
            foreach (var d in st.ToArray()) // ToArray giữ thứ tự push
                foreach (var kv in d)
                    merged[kv.Key] = kv.Value;

            return merged;
        }

        private sealed class ScopeGuard : IDisposable
        {
            private readonly AsyncLocal<Stack<Dictionary<string, object>>?> _slot;
            private bool _disposed;
            public ScopeGuard(AsyncLocal<Stack<Dictionary<string, object>>?> slot) => _slot = slot;
            public void Dispose()
            {
                if (_disposed) return;
                var st = _slot.Value;
                if (st != null && st.Count > 0) st.Pop();
                _disposed = true;
            }
        }
    }
}