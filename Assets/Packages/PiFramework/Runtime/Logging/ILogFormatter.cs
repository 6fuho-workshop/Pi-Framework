
using System.Collections.Generic;
using System.Linq;

namespace PF.Logging.Internal
{
    public interface ILogFormatter
    {
        // Trả về dòng text cuối cùng (Console/File).
        string Format(in LogEvent e, FormatOptions opt);
    }

    public sealed class FormatOptions
    {
        public bool AddTime = false;     // default: false
        public string TimeFormat = "HH:mm:ss.fff";
        public bool UseUtc = false;
    }

    // Formatter mặc định: 1 dòng dễ grep
    public sealed class SimpleTextFormatter : ILogFormatter
    {
        // Ghi chú: ưu tiên đường thẳng, có thể tối ưu thêm bằng ArrayPool StringBuilder nếu cần.
        public string Format(in LogEvent e, FormatOptions opt)
        {
            var prefix = string.Empty;
            if (opt.AddTime)
            {
                var ts = opt.UseUtc ? e.Now.ToUniversalTime() : e.Now;
                prefix = $"{ts.ToString(opt.TimeFormat)} ";
            }

            string scope = string.Empty;
            if (e.Scope is { Count: > 0 })
            {
                // k:v,k2:v2
                var parts = string.Join(",", e.Scope.Select(kv => $"{kv.Key}:{kv.Value}"));
                scope = " | " + parts;
            }

            string ex = e.Exception is null ? string.Empty
                                            : $" | ex:{e.Exception.GetType().Name} {e.Exception.Message}";

            string category = string.IsNullOrEmpty(e.Category) ? string.Empty : $"({e.Category}) ";

            return $"{prefix}[{e.Level.ToString()[0]}] {category}{e.Message}{scope}{ex}";
        }
    }
}