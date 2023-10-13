namespace PiFramework.KeyValueStore
{
    public interface IKeyValueStore
    {
        bool HasKey(string key);
        void DeleteKey(string key);
        void DeleteAll();

        bool TryGetBool(string key, out bool value);
        bool TryGetInt(string key, out int value);
        bool TryGetLong(string key, out long value);
        bool TryGetFloat(string key, out float value);
        bool TryGetDouble(string key, out double value);
        bool TryGetString(string key, out string value);
        bool TryGetBytes(string key, out byte[] value);

        void SetBool(string key, bool value);
        void SetInt(string key, int value);
        void SetLong(string key, long value);
        void SetFloat(string key, float value);
        void SetDouble(string key, double value);
        void SetString(string key, string value);
        void SetBytes(string key, byte[] value);

        public bool GetBool(string key, bool defaultValue = default)
        {
            return TryGetBool(key, out bool value) ? value : defaultValue;
        }
        public int GetInt(string key, int defaultValue = default)
        {
            return TryGetInt(key, out int value) ? value : defaultValue;
        }
        public long GetLong(string key, long defaultValue = default)
        {
            return TryGetLong(key, out long value) ? value : defaultValue;
        }
        public float GetFloat(string key, float defaultValue = default)
        {
            return TryGetFloat(key, out float value) ? value : defaultValue;
        }
        public double GetDouble(string key, double defaultValue = default)
        {
            return TryGetDouble(key, out double value) ? value : defaultValue;
        }
        public string GetString(string key, string defaultValue = default)
        {
            return TryGetString(key, out string value) ? value : defaultValue;
        }
        public byte[] GetBytes(string key, byte[] defaultValue = default)
        {
            return TryGetBytes(key, out byte[] value) ? value : defaultValue;
        }
    }
}