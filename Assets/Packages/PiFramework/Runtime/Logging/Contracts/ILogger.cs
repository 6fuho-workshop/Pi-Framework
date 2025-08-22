using System;
using System.Collections.Generic;

#nullable enable
namespace PF.Contracts
{
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
    /// A generic interface for logging where the category name is derived from the specified
    /// <typeparamref name="TCategoryName"/> type name.
    /// Generally used to enable activation of a named <see cref="ILogger"/> from dependency injection.
    /// </summary>
    /// <typeparam name="TCategoryName">The type whose name is used for the logger category name.</typeparam>
    public interface ILogger<out TCategoryName> : ILogger
    {

    }
}
