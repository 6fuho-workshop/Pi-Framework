using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

#nullable enable
namespace PF.Core.Diagnostics
{
    // ------------- Runtime Level Switch & Config trung tâm -------------
    // Flow:
    // - LogConfig giữ default và danh sách rule theo prefix.
    // - LoggerFactory khi Create(category) sẽ lấy "switch" tốt nhất (prefix dài nhất).
    // - Update level runtime chỉ cần đổi giá trị trong switch => logger thấy ngay.
    public sealed class LogLevelSwitch
    {
        private int _min;

        public LogLevel MinLevel
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => (LogLevel)Volatile.Read(ref _min);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Volatile.Write(ref _min, (int)value);
        }
    }

    public sealed class LogConfig
    {
        private readonly LogLevelSwitch _default = new() { MinLevel = LogLevel.Info };
        // Lưu prefix rule; khi match: "prefix dài nhất thắng".
        private readonly List<(string prefix, LogLevelSwitch sw)> _rules = new();

        public void SetDefault(LogLevel level) => _default.MinLevel = level;

        public void SetPrefix(string prefix, LogLevel level)
        {
            // Chuẩn hoá: bỏ dấu '*' cuối nếu có (để người dùng tiện ghi "PF.Core.*")
            if (prefix.EndsWith("*", StringComparison.Ordinal))
                prefix = prefix.Substring(0, prefix.Length - 1);

            var idx = _rules.FindIndex(r => r.prefix == prefix);
            if (idx >= 0) _rules[idx].sw.MinLevel = level;
            else _rules.Add((prefix, new LogLevelSwitch { MinLevel = level }));
        }

        // Tìm switch phù hợp nhất cho category
        public LogLevelSwitch GetSwitchFor(string category)
        {
            LogLevelSwitch best = _default; int bestLen = -1;
            foreach (var (prefix, sw) in _rules)
            {
                if (category.StartsWith(prefix, StringComparison.Ordinal) && prefix.Length > bestLen)
                {
                    best = sw; bestLen = prefix.Length;
                }
            }
            return best;
        }

        // Nạp nhanh từ JSON: { "default":"Info", "overrides": { "PF.Core.*":"Warn", "PF.Core.Flow":"Debug" } }
        public void ApplyJson(string json)
        {
            try
            {
                var dto = JsonConvert.DeserializeObject<LogConfigDto>(json);
                //var dto = System.Text.Json.JsonSerializer.Deserialize<LogConfigDto>(json);
                if (dto == null) return;

                if (!string.IsNullOrWhiteSpace(dto.@default) &&
                    Enum.TryParse<LogLevel>(dto.@default, true, out var d))
                    SetDefault(d);

                if (dto.overrides != null)
                {
                    foreach (var kv in dto.overrides)
                    {
                        if (Enum.TryParse<LogLevel>(kv.Value, true, out var lv))
                            SetPrefix(kv.Key, lv);
                    }
                }
            }
            catch { /* ignore json errors intentionally */ }
        }

        private sealed class LogConfigDto
        {
            public string? @default { get; set; }
            public Dictionary<string, string>? overrides { get; set; }
        }
    }
}