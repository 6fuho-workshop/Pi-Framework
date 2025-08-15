using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

#nullable enable
namespace PF.Logging.Internal
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
                if (dto == null) return;

                // 1) Default
                if (!string.IsNullOrWhiteSpace(dto.@default) &&
                    Enum.TryParse<LogLevel>(dto.@default, true, out var d))
                {
                    _default.MinLevel = d; // cập nhật in-place: logger trỏ default thấy ngay
                }

                // 2) Chuẩn hoá overrides mới (replace semantics)
                var wanted = new Dictionary<string, LogLevel>(StringComparer.Ordinal);
                if (dto.overrides != null)
                {
                    foreach (var kv in dto.overrides)
                    {
                        // Chuẩn hoá prefix: bỏ dấu '*' cuối nếu có (PF.Core.* → "PF.Core.")
                        var prefix = kv.Key.EndsWith("*", StringComparison.Ordinal)
                                   ? kv.Key.Substring(0, kv.Key.Length - 1)
                                   : kv.Key;

                        if (!Enum.TryParse<LogLevel>(kv.Value, true, out var lv))
                            continue;

                        if (lv == LogLevel.Default) continue;   // không tạo rule cho Default
                        wanted[prefix] = lv;                    // giữ rule này với level tương ứng
                    }
                }

                // 3) Xây danh sách rule MỚI: reuse switch cũ nếu có, tạo switch mới nếu chưa có
                var old = new Dictionary<string, LogLevelSwitch>(StringComparer.Ordinal);
                foreach (var (prefix, sw) in _rules) old[prefix] = sw;

                var newRules = new List<(string prefix, LogLevelSwitch sw)>(wanted.Count);
                foreach (var kv in wanted)
                {
                    if (old.TryGetValue(kv.Key, out var sw))
                    {
                        sw.MinLevel = kv.Value; // cập nhật level
                        newRules.Add((kv.Key, sw)); // reuse switch cũ
                    }
                    else
                    {
                        newRules.Add((kv.Key, new LogLevelSwitch { MinLevel = kv.Value }));
                    }
                }

                // 4) Thay bộ rule (xoá hẳn rule cũ không còn trong JSON)
                _rules.Clear();
                _rules.AddRange(newRules);
            }
            catch
            {
                // Ngó lơ lỗi JSON
            }

            // 5) Sau khi thay bộ rule -> mọi logger phải trỏ đúng switch mới (longest-prefix thắng)
            //    (không recreate ILogger; chỉ gán lại switch)
            //    Làm bước này ở Log.ApplyRemoteConfigJson(...)
        }

        private sealed class LogConfigDto
        {
            public string? @default { get; set; }
            public Dictionary<string, string>? overrides { get; set; }
        }
    }
}