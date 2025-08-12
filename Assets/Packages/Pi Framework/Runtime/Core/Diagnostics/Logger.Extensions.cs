// =====================
// PF.Logging.Extensions
// =====================
//
// Mục tiêu:
// - Cung cấp extension methods với [Conditional] để "xóa code" ở build Release.
// - Trace/Debug/Info/Warn: chỉ tồn tại khi bật symbol tương ứng.
// - Error/Fatal: luôn giữ (không gắn Conditional).
//
// Dev/Editor (Player Settings -> Scripting Define Symbols):
// PF_LOG_TRACE;PF_LOG_DEBUG;PF_LOG_INFO;PF_LOG_WARN
//
// Release: (để trống)

using System;
using System.Diagnostics;

#nullable enable
namespace PF.Core.Diagnostics
{
    public static class LoggerExtensions
    {
        // --- Levels strip-able ---
        [Conditional("PF_LOG")]
        public static void Verbose(this ILog l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Verbose, msg);
        }

        [Conditional("PF_LOG")]
        public static void Debug(this ILog l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Debug, msg);
        }

        [Conditional("PF_LOG")]
        public static void Info(this ILog l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Info, msg);
        }

        [Conditional("PF_LOG")]
        public static void Warn(this ILog l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Warn, msg);
        }

        // Biến thể lazy để tránh evaluate nặng ở Dev khi category đang tắt
        [Conditional("PF_LOG")]
        public static void Debug(this ILog l, Func<string> make)
        {
            if (l == null) return;
            if (!l.IsEnabled(LogLevel.Debug)) return;
            l.Log(LogLevel.Debug, make());
        }

        [Conditional("PF_LOG")]
        public static void Verbose(this ILog l, Func<string> make)
        {
            if (l == null) return;
            if (!l.IsEnabled(LogLevel.Verbose)) return;
            l.Log(LogLevel.Verbose, make());
        }

        // --- Levels luôn giữ ---
        public static void Error(this ILog l, string msg, Exception? ex = null)
        {
            if (l == null) return;
            l.Log(LogLevel.Error, msg, ex);
        }

        public static void Fatal(this ILog l, string msg, Exception? ex = null)
        {
            if (l == null) return;
            l.Log(LogLevel.Fatal, msg, ex);
        }
    }
}
