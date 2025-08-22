// =====================
// PF.Logging.Core (thuần C#)
// =====================
//
// Mục tiêu:
// - Logger AOT-safe, không phụ thuộc Unity.
// - Cho phép config level theo category/prefix (hot-reload).
// - Overhead rất thấp khi level bị tắt (check sớm IsEnabled + lazy message).
// - Scopes nhẹ bằng AsyncLocal (merge khi thật sự ghi).
//
// Sử dụng kèm PF.Logging.Extensions (Conditional) để strip Debug/Info/Warn ở Release.


#nullable enable
using PF.Contracts;
using PF.Logging.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace PF.Logging
{
    public sealed class PiLogger : ILogger
    {
        private LogLevelSwitch _switch;
        private readonly List<ILogSink> _sinks;
        private readonly Func<ILogFormatter> _getFormatter;

        public string Category { get; }

        public PiLogger(string category, LogLevelSwitch levelSwitch,
                      List<ILogSink> sinks, Func<ILogFormatter> getFormatter)
        {
            Category = category;
            _switch = levelSwitch;
            _sinks = sinks;
            _getFormatter = getFormatter;
        }

        // Fast path check – rẻ, 1 phép so sánh
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEnabled(LogLevel level) => level >= _switch.MinLevel;

        public IDisposable BeginScope(string key, object value) => ScopeProvider.BeginScope(key, value);
        public IDisposable BeginScope(IReadOnlyDictionary<string, object> kvs) => ScopeProvider.BeginScope(kvs);

        internal void RebindSwitch(LogLevelSwitch sw)
        {
            _switch = sw; // đổi sang switch của rule mới
        }

        // Ghi với message sẵn
        public void Log(LogLevel level, string message, Exception? ex = null)
        {
            if (!IsEnabled(level)) return;

            var ev = new LogEvent(
                DateTime.Now,
                level,
                Category,
                message,
                ex,
                ScopeProvider.CurrentSnapshotOrNull()
            );

            // Gửi tới tất cả sinks (sinks tự lo thread-safety)
            for (int i = 0; i < _sinks.Count; i++)
                _sinks[i]?.Write(in ev);
        }

        // Ghi với message lazy – chỉ build khi đã qua filter
        public void Log(LogLevel level, Func<string> messageFactory, Exception? ex = null)
        {
            if (!IsEnabled(level)) return;
            Log(level, messageFactory(), ex);
        }
    }
}
