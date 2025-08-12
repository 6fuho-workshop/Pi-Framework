using System;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY_2021_3_OR_NEWER
using PF.Unity.Diagnostics;
#endif

namespace PF.Core.Diagnostics
{
    public class PiLog
    {
        private static LoggerFactory Factory = null;
        private static LogConfig Config = null;
        private static readonly Dictionary<string, ILog> _loggerCache = new();
        private static bool _isInitialized = false;

        public static ILog Default = null;
        internal static ILog Bootstrap = null;
        internal static ILog Core = null;
        public static ILog Flow = null;
        public static ILog Assets = null;
        public static ILog Net = null;
        public static ILog Save = null;
        
        public static ILog Get(string category)
        {
            if (!_isInitialized)
                return new DummyLogger();

            if (string.IsNullOrEmpty(category))
                return Default;

            if (_loggerCache.TryGetValue(category, out var logger))
                return logger;
            // Tạo mới logger cho category nếu chưa có
            logger = Factory.Create(category);
            _loggerCache[category] = logger;

            return logger;
        }

        public static void Init(bool useUnitySink, string filePath = null)
        {
            _isInitialized = true;
            Config = new LogConfig();
            Config.SetDefault(LogLevel.Debug);
            Config.SetPrefix("Flow", LogLevel.Debug); // Dev bật sâu cho Flow

            Factory = new LoggerFactory(Config)
                //.AddSink(new ConsoleLogSink()) // ConsoleLogSink không hoạt động trên Unity nhưng có ghi vào editor.log
                .SetFormatter(new SimpleTextFormatter());
                

#if UNITY_2021_3_OR_NEWER
            if (useUnitySink) Factory.AddSink(new UnityLogSink( new UnityLogFormatter()));
#endif
            if (!string.IsNullOrEmpty(filePath))
                Factory.AddSink(new FileLogSink(filePath));

            Default = Factory.Create("");
            Bootstrap = Get("Bootstrap");
            Core = Get("Core");
            Flow = Get("Flow");
            Assets = Get("Assets");
            Net = Get("Net");
            Save = Get("Save");
        }

        internal static void Shutdown()
        {
            Factory?.Dispose();
            _loggerCache.Clear();
            Factory = null!;
            Config = null!;
            Default = Core = Flow = Assets = Net = Save = Bootstrap = null!;
            _isInitialized = false;
        }
        public static void ApplyRemoteConfigJson(string json) => Config.ApplyJson(json);

        // ====== Static convenience methods cho Default ======

        public static IDisposable BeginScope(string key, object value) => ScopeProvider.BeginScope(key, value);
        public static IDisposable BeginScope(IReadOnlyDictionary<string, object> kvs) => ScopeProvider.BeginScope(kvs);

        [Conditional("PF_LOG")]
        public static void Debug(string msg) => Default.Log(LogLevel.Debug, msg);

        [Conditional("PF_LOG")]
        public static void Verbose(string msg) => Default.Log(LogLevel.Verbose, msg);

        [Conditional("PF_LOG")]
        public static void Info(string msg) => Default.Log(LogLevel.Info, msg);

        [Conditional("PF_LOG")]
        public static void Warn(string msg) => Default.Log(LogLevel.Warn, msg);

        // Biến thể lazy để tránh evaluate nặng ở Dev khi category đang tắt
        [Conditional("PF_LOG")]
        public static void Debug(Func<string> make)
        {
            if (!Default.IsEnabled(LogLevel.Debug)) return;
            Default.Log(LogLevel.Debug, make());
        }

        [Conditional("PF_LOG")]
        public static void Verbose(Func<string> make)
        {
            if (!Default.IsEnabled(LogLevel.Verbose)) return;
            Default.Log(LogLevel.Verbose, make());
        }

        public static void Error(string msg, Exception ex = null) =>
            Default.Log(LogLevel.Error, msg, ex);

        public static void Fatal(string msg, Exception ex = null) =>
            Default.Log(LogLevel.Fatal, msg, ex);
    }
}