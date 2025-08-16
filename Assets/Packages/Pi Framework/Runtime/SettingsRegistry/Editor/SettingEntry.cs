using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;

namespace PF.PiEditor.Settings
{
    [Serializable]
    [MovedFrom(true,"PiEditor.Settings",null, "SettingEntity")]
    public class SettingEntry
    {
        [FormerlySerializedAs("fullPath")]
        [HideInInspector] public string FullPath;

        [Tooltip("Đường dẫn tương đối so với pathPrefix. Ví dụ: video.fullscreen hoặc audio.volume.sfxVolume")]
        [FormerlySerializedAs("path")]
        public string RelativePath;

        [Tooltip("Kiểu dữ liệu: int, float, bool, string, enum; hoặc tên type đầy đủ (vd: MyGame.GraphicsQuality)")]
        [FormerlySerializedAs("type")]
        public string ValueType;

        [Tooltip("Biểu thức C# cho giá trị mặc định. Ví dụ: 0.75f, true, \"en\", new Vector3(1,1.5f,2). Để trống => dùng default(T).")]
        [FormerlySerializedAs("defaultValue")]
        public string DefaultExpression;

        [Tooltip("Nếu bật, property không có setter (chỉ đọc). Vẫn có thể bị override bởi deserialize/persist.")]
        [FormerlySerializedAs("readOnly")]
        public bool IsReadOnly = true;

        [Tooltip("Mô tả ngắn dùng cho Inspector/Control Panel.")]
        [FormerlySerializedAs("tooltip")]
        public string Description;

        [Tooltip("Chỉ áp dụng cho int/float. Bật để dùng min/max/step cho clamp & UI.")]
        [FormerlySerializedAs("useRange")]
        public bool HasRange;

        [Tooltip("Giới hạn dưới/Trên cho int/float; step gợi ý cho UI (không bắt buộc).")]
        [FormerlySerializedAs("min")]
        public float Min = 0.01f;

        [Tooltip("Giới hạn dưới/Trên cho int/float; step gợi ý cho UI (không bắt buộc).")]
        [FormerlySerializedAs("max")]
        public float Max = 0.01f;

        [Tooltip("Nếu true, giá trị được lưu/persist theo key storageKey (hoặc key mặc định).")]
        [FormerlySerializedAs("persitent")]
        public bool Persist;

        [FormerlySerializedAs("customKey")]
        [Tooltip("Tuỳ chọn override key persist. Mặc định: pf.v1.<fullPath>")]
        public string StorageKeyOverride;

        public string LeafName
        {
            get
            {
                if (string.IsNullOrEmpty(RelativePath)) return string.Empty;
                var idx = RelativePath.LastIndexOf('.');
                return idx < 0 ? RelativePath : RelativePath[(idx + 1)..];
            }
        }

        public string ParentNodePath
        {
            get
            {
                if (string.IsNullOrEmpty(RelativePath)) return string.Empty;
                var idx = RelativePath.LastIndexOf('.');
                return idx < 0 ? string.Empty : RelativePath[..idx];
            }
        }

        public bool IsValid()
        {
            if (string.IsNullOrEmpty(RelativePath)) return false;
            if (string.IsNullOrEmpty(ValueType)) return false;
            return true;
        }

        public void Validate(string pathPrefix)
        {
            // Chuẩn hoá dữ liệu chuỗi
            RelativePath = (RelativePath ?? "").Replace(" ", "").Trim('.');
            ValueType = (ValueType ?? "").Replace(" ", "");
            StorageKeyOverride = (StorageKeyOverride ?? "").Trim();

            // Chuẩn min/max/step
            if (Min > Max) (Min, Max) = (Max, Min);
            if (string.Equals(ValueType, "int", StringComparison.OrdinalIgnoreCase))
            {
                Min = Mathf.Floor(Min);
                Max = Mathf.Floor(Max);
            }
            if (!string.Equals(ValueType, "int", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(ValueType, "float", StringComparison.OrdinalIgnoreCase))
            {
                HasRange = false;
            }

            // ReadOnly thì không persist
            if (IsReadOnly) Persist = false;

            // Ký tự hợp lệ (dấu . làm phân cấp)
            if (RelativePath.IndexOfAny(new[] { '/', '\\' }) >= 0)
                throw new ArgumentException($"Use '.' instead of '/' in relativePath: {RelativePath}");

            if (!IsValid())
                throw new ArgumentException($"Invalid entry. relativePath and valueType are required.");

            // Tạo fullPath
            FullPath = string.IsNullOrEmpty(pathPrefix) ? RelativePath : $"{pathPrefix}.{RelativePath}";
        }
    }
}