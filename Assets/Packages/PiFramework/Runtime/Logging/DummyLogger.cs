using PF.Contracts;
using System;
using System.Collections.Generic;

#nullable enable
namespace PF.Logging.Internal
{
    public class DummyLogger : ILogger
    {
        public string Category => string.Empty;
        public bool IsEnabled(LogLevel level) => false;
        public IDisposable BeginScope(string key, object value) => null!;
        public IDisposable BeginScope(IReadOnlyDictionary<string, object> kvs) => null!;
        public void Log(LogLevel level, string message, Exception? ex = null)
        {
        }
        public void Log(LogLevel level, Func<string> messageFactory, Exception? ex = null)
        {
        }
    }
}