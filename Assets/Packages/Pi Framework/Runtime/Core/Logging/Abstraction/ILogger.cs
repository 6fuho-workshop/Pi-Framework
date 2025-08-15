using System;
using System.Collections.Generic;
using PF.Logging.Internal;

#nullable enable
namespace PF.Logging
{

    /// Debug – log cho dev, giúp gỡ lỗi.
    /// Verbose – chi tiết hơn Info nhưng ít spam hơn Debug, thường để “mổ xẻ” quy trình.
    /// Info – thông tin chung, quan trọng nhưng không báo lỗi.
    /// Warning – cảnh báo có thể gây lỗi nhưng vẫn chạy được.
    /// Error – lỗi nghiêm trọng ảnh hưởng tính năng, nhưng chưa crash.
    /// Fatal – lỗi nghiêm trọng nhất, khiến game phải dừng hoặc reset.
    /// có thể cấu hình log filter theo môi trường:
    /// Development build: bật từ Debug trở lên(để debug tối đa).
    /// Production build: bật từ Error trở lên(để không flood log).
    /// QA/Testing build: bật từ Debug trở lên
    public enum LogLevel
    {
        Default = 100,   // CHỈ dành cho cấu hình (SO/JSON). Không set vào MinLevel runtime.
        Trace = 0,
        Debug = 1,
        Info = 2,
        Warn = 3,
        Error = 4,
        Fatal = 5,
        None = 99
    }

    /// <summary>
    /// Interface for logging functionality, supporting multiple log levels, category, and structured scopes.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the log category associated with this logger.
        /// </summary>
        string Category { get; }

        /// <summary>
        /// Determines whether logging is enabled for the specified <paramref name="level"/>.
        /// </summary>
        /// <param name="level">The log level to check.</param>
        /// <returns>True if logging is enabled for the level; otherwise, false.</returns>
        bool IsEnabled(LogLevel level);

        /// <summary>
        /// Begins a logging scope with a single key-value pair.
        /// </summary>
        /// <param name="key">The scope key.</param>
        /// <param name="value">The scope value.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
        IDisposable BeginScope(string key, object value);

        /// <summary>
        /// Begins a logging scope with multiple key-value pairs.
        /// </summary>
        /// <param name="kvs">The key-value pairs for the scope.</param>
        /// <returns>An <see cref="IDisposable"/> that ends the scope on dispose.</returns>
        IDisposable BeginScope(IReadOnlyDictionary<string, object> kvs);

        // Lối vào "lõi" (extensions sẽ gọi vào đây).

        /// <summary>
        /// Logs a message at the specified log level.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="message">The log message.</param>
        /// <param name="ex">Optional exception to include in the log.</param>
        void Log(LogLevel level, string message, Exception? ex = null);

        /// <summary>
        /// Logs a message at the specified log level using a message factory.
        /// </summary>
        /// <param name="level">The log level.</param>
        /// <param name="messageFactory">A function that produces the log message.</param>
        /// <param name="ex">Optional exception to include in the log.</param>
        void Log(LogLevel level, Func<string> messageFactory, Exception? ex = null);
    }

    /// <summary>
    /// Represents a log sink that receives log events for processing and output.
    /// Implementations must ensure thread-safety if required.
    /// </summary>
    public interface ILogSink
    {
        // Sink tự đảm bảo thread-safety nội bộ nếu cần.

        /// <summary>
        /// Writes a log event to the sink.
        /// </summary>
        /// <param name="e">The log event to write.</param>
        void Write(in LogEvent e);
    }

    internal class DummyLogger: ILogger{
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
