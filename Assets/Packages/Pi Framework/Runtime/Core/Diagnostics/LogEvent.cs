using System;
using System.Collections.Generic;

#nullable enable
namespace PF.Core.Diagnostics
{
    public readonly struct LogEvent
    {
        public readonly DateTime Now;
        public readonly LogLevel Level;
        public readonly string Category;
        public readonly string Message;
        public readonly Exception? Exception;
        public readonly IReadOnlyDictionary<string, object>? Scope;

        public LogEvent(DateTime now, LogLevel level, string category, string message,
                        Exception? ex = null,
                        IReadOnlyDictionary<string, object>? scope = null)
        {
            Now = now;
            Level = level;
            Category = category;
            Message = message;
            Exception = ex;
            Scope = scope;
        }
    }
}