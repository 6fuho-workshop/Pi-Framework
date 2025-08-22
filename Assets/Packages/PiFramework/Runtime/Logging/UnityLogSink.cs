// =====================
// PF.Logging.UnitySink (đặt trong assembly tham chiếu UnityEngine)
// =====================
//
// Flow:
// - Map level sang Debug.Log/LogWarning/LogError của Unity.
// - Unity sẽ hiển thị icon/stack phù hợp.
// - Ở Release, các call Debug/Info/Warn đã strip ở Compile-time nên chủ yếu còn Error/Fatal.
//

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PF.Logging;
using PF.Logging.Internal;
using PF.Contracts;

#nullable enable
namespace PF.Logging
{
    public class UnityLogFormatter : ILogFormatter
    {
        public readonly Dictionary<LogLevel, string> LevelColors = new() {
            { LogLevel.Trace,   "#909090" },
            { LogLevel.Debug,   "#4B93EB" },
            { LogLevel.Info,    "#22BB22" },
            { LogLevel.Warn,    "#FFD700" },
            { LogLevel.Error,   "#CC2222" },
            { LogLevel.Fatal,   "#C235FF" }
        };

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

            string category = string.IsNullOrEmpty(e.Category) ? string.Empty : $"({e.Category}) \n";
            //var level = $"[{e.Level.ToString()[0]}] ";
            
            var color = LevelColors[e.Level];
            // Unity Rich Text: <color=#RRGGBB>text</color>
            //level = $"<color={color}>{level}</color>";

            //return $"{prefix}<color={color}>{level}{category}{e.Message}</color>{scope}{ex}";
            return $"{prefix}<color={color}>{e.Message}</color>\n{category}{scope}{ex}";
        }
    }

    public sealed class UnityLogSink : ILogSink
    {
        private readonly ILogFormatter _formatter;
        private readonly FormatOptions _options = new();

        public UnityLogSink(ILogFormatter? formatter = null)
        {
            _formatter = formatter ?? new SimpleTextFormatter();
        }

        public void Write(in LogEvent e)
        {
            var line = _formatter.Format(in e, _options);
            if (e.Exception != null)
                line = $"{line}\n{e.Exception}";

            switch (e.Level)
            {
                case LogLevel.Warn:
                    Debug.LogWarning(line);
                    break;
                case LogLevel.Error:
                case LogLevel.Fatal:
                    Debug.LogError(line);
                    break;
                default:
                    Debug.Log(line);
                    break;
            }
        }
    }
}
