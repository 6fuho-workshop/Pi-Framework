using System;
using System.Diagnostics;
#nullable enable
namespace PF.Contracts
{
    public static class LoggerExtensions
    {
        // --- Levels strip-able ---
        [Conditional("PF_LOG")]
        public static void Trace(this ILogger l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Trace, msg);
        }

        [Conditional("PF_LOG")]
        public static void Debug(this ILogger l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Debug, msg);
        }

        [Conditional("PF_LOG")]
        public static void Info(this ILogger l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Info, msg);
        }

        [Conditional("PF_LOG")]
        public static void Warn(this ILogger l, string msg)
        {
            if (l == null) return;
            l.Log(LogLevel.Warn, msg);
        }

        // Biến thể lazy để tránh evaluate nặng ở Dev khi category đang tắt
        [Conditional("PF_LOG")]
        public static void Debug(this ILogger l, Func<string> make)
        {
            if (l == null) return;
            if (!l.IsEnabled(LogLevel.Debug)) return;
            l.Log(LogLevel.Debug, make());
        }

        [Conditional("PF_LOG")]
        public static void Trace(this ILogger l, Func<string> make)
        {
            if (l == null) return;
            if (!l.IsEnabled(LogLevel.Trace)) return;
            l.Log(LogLevel.Trace, make());
        }

        // --- Levels luôn giữ ---
        public static void Error(this ILogger l, string msg, Exception? ex = null)
        {
            if (l == null) return;
            l.Log(LogLevel.Error, msg, ex);
        }

        public static void Fatal(this ILogger l, string msg, Exception? ex = null)
        {
            if (l == null) return;
            l.Log(LogLevel.Fatal, msg, ex);
        }
    }
}
