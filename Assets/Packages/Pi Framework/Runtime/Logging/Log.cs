using System;
using System.Collections.Generic;
using System.Diagnostics;
using PF.Logging.Internal;
using PF.Logging;

namespace PF
{
    public class Log
    {
        private static LoggerFactory Factory = null;
        private static LogConfig Config = null;
        private static readonly Dictionary<string, ILogger> _loggerCache = new();
        private static bool _isInitialized = false;

        public static ILogger Default { get; private set; } = null;
        internal static ILogger Bootstrap { get; private set; } = null;
        internal static ILogger Core { get; private set; } = null;
        public static ILogger Gameplay { get; private set; } = null;
        public static ILogger World { get; private set; } = null;
        public static ILogger Net { get; private set; } = null;
        public static ILogger Save { get; private set; } = null;

        public static ILogger UI { get; private set; } = null;
        public static ILogger Audio { get; private set; } = null;
        public static ILogger Input { get; private set; } = null;
        public static ILogger Camera { get; private set; } = null;
        public static ILogger Analytics { get; private set; } = null;
        public static ILogger Ads { get; private set; } = null;
        public static ILogger Iap { get; private set; } = null;

        public static ILogger Get(string category)
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
            Config = new LogConfig();
            Config.SetDefault(LogLevel.Trace);
            //Config.SetPrefix("Flow", LogLevel.Debug); // Dev bật sâu cho Flow

            Factory = new LoggerFactory(Config)
                //.AddSink(new ConsoleLogSink()) // ConsoleLogSink không hoạt động trên Unity nhưng có ghi vào editor.log
                .SetFormatter(new SimpleTextFormatter());


#if UNITY_2021_3_OR_NEWER
            if (useUnitySink) Factory.AddSink(new UnityLogSink(new UnityLogFormatter()));
#endif
            if (!string.IsNullOrEmpty(filePath))
                Factory.AddSink(new FileLogSink(filePath));

            _isInitialized = true; // ready for creating loggers

            Default = Get("General");
            Bootstrap = Get("PF.Bootstrap");
            Core = Get("PF.Core");
            Gameplay = Get("flow");
            World = Get("assets");
            Net = Get("net");
            Save = Get("save");

            UI = Get("UI");
            Audio = Get("Audio");
            Input = Get("Input");
            Camera = Get("Camera");
            Analytics = Get("Analytics");
            Ads = Get("Ads");
            Iap = Get("Iap");

            Bootstrap.Info($"Log ON with {Factory.Sinks.Count} sinks.");
        }

        public static void RebindAll()
        {
            if (!_isInitialized || Factory == null) return;

            // mọi logger trong cache (category -> ILogger)
            foreach (var kv in _loggerCache)
                if (kv.Value is PiLogger pl)
                    pl.RebindSwitch(Config.GetSwitchFor(kv.Key)); // lấy switch của rule thắng hiện tại
        }

        internal static void Shutdown()
        {
            _isInitialized = false; // chặn Get() tạo logger mới, chỉ tạo dummy logger
            _loggerCache.Clear();
            Bootstrap.Info("Log: OFF");
            Factory?.Dispose(); // sau bước này sẽ k có log
            Factory = null!;
            Config = null!;
            
            Default = Core = Gameplay = World = Net = Save = Bootstrap = null!;
            UI = Audio = Input = Camera = Analytics = Ads = Iap = null!;
        }
        public static void ApplyRemoteConfigJson(string json)
        {
            Config.ApplyJson(json); // replace toàn bộ rule set
            RebindAll();            // LUÔN rebind, vì có thể có rule bị xoá → cate rơi về default/prefix khác
        }

        // ====== Static convenience methods cho Default ======

        public static IDisposable BeginScope(string key, object value) => ScopeProvider.BeginScope(key, value);
        public static IDisposable BeginScope(IReadOnlyDictionary<string, object> kvs) => ScopeProvider.BeginScope(kvs);

        [Conditional("PF_LOG")]
        public static void Debug(string msg) => Default.Log(LogLevel.Debug, msg);

        [Conditional("PF_LOG")]
        public static void Trace(string msg) => Default.Log(LogLevel.Trace, msg);

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
        public static void Trace(Func<string> make)
        {
            if (!Default.IsEnabled(LogLevel.Trace)) return;
            Default.Log(LogLevel.Trace, make());
        }

        public static void Error(string msg, Exception ex = null) =>
            Default.Log(LogLevel.Error, msg, ex);

        public static void Fatal(string msg, Exception ex = null) =>
            Default.Log(LogLevel.Fatal, msg, ex);
    }
}