// Assets/PF/Core/Diagnostics/Config/LogCategoryApplier.cs
using System.Text;
using System.Collections.Generic;
using PF.Logging;
using PF;
using System.Diagnostics;

public static class LogCategoryApplier
{
    /// <summary>
    /// Áp cấu hình từ SO: defaultLevel cho toàn hệ (VD: Error ở Prod, Verbose/Debug ở Dev),
    /// và overrides theo cây. Dùng Longest-prefix (mặc định của PF).
    /// </summary>
    public static void Apply(LogCategoryTree tree, LogLevel defaultLevel = LogLevel.Error)
    {
        if (tree == null) return;

        Dictionary<string, LogLevel> map = tree.BuildOverrides();

        // JSON: { "default":"Error", "overrides": { "PF.Core":"Warn", "PF.Core.Flow":"Debug", ... } }
        var sb = new StringBuilder(2048);
        sb.Append("{\"default\":\"").Append(defaultLevel.ToString()).Append("\",\"overrides\":{");

        bool first = true;
        foreach (var kv in map)
        {
            if (!first) sb.Append(',');
            first = false;
            sb.Append('\"').Append(kv.Key).Append("\":\"").Append(kv.Value.ToString()).Append('\"');
        }
        sb.Append("}}");

        // Đẩy vào PF
        UnityEngine.Debug.Log(sb.ToString());
        Log.ApplyRemoteConfigJson(sb.ToString());
    }
}
