using System;
using System.Collections.Generic;
using PF.Logging.Internal;

namespace PF.Logging
{
    // ------------- LoggerFactory (wiring trung tâm) -------------
    // Flow:
    // - Khởi tạo 1 factory, set formatter, add sinks.
    // - Mỗi lần Create(category) -> bind vào LogLevelSwitch phù hợp (theo LogConfig).
    public sealed class LoggerFactory : IDisposable
    {
        private readonly LogConfig _config;
        private readonly List<ILogSink> _sinks = new();
        internal List<ILogSink> Sinks => _sinks;    

        private ILogFormatter _formatter = new SimpleTextFormatter();

        public LoggerFactory(LogConfig config) { _config = config; }

        public LoggerFactory AddSink(ILogSink sink) { _sinks.Add(sink); return this; }
        public LoggerFactory SetFormatter(ILogFormatter formatter) { _formatter = formatter; return this; }

        public ILogger Create(string category)
            => new PiLogger(category, _config.GetSwitchFor(category), _sinks, () => _formatter);

        private bool _disposed = false;
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            // Dispose sinks nếu có
            for (int i = 0; i < _sinks.Count; i++)
            {
                if (_sinks[i] is IDisposable d)
                {
                    try { d.Dispose(); } catch { /* swallow */ }
                }
            }
            _sinks.Clear();
        }
    }
}
