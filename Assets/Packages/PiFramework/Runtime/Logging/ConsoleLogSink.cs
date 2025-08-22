// =====================
// PF.Logging.ConsoleSink
// =====================
//
// Mục tiêu:
// - In ra stdout/stderr theo level.
// - Giữ plain text để grep/CI dễ. Có thể bổ sung ANSI màu nếu muốn.

using PF.Contracts;
using PF.Logging;
using PF.Logging.Internal;
using System;
#nullable enable
namespace PF.Logging
{
    public sealed class ConsoleLogSink : ILogSink
    {
        private readonly ILogFormatter _formatter;
        private readonly FormatOptions _options = new() { AddTime = true, UseUtc = false };

        public ConsoleLogSink(ILogFormatter? formatter = null)
        {
            _formatter = formatter ?? new SimpleTextFormatter();
        }

        public void Write(in LogEvent e)
        {
            var line = _formatter.Format(in e, _options);
            switch (e.Level)
            {
                case LogLevel.Warn:
                case LogLevel.Error:
                case LogLevel.Fatal:
                    Console.Error.WriteLine(line);
                    break;
                default:
                    Console.Out.WriteLine(line);
                    break;
            }
        }
    }
}