using UnityEngine;
using System.Collections;
using System;

namespace PiFramework
{
    /// <summary>
    /// more security
    /// </summary>

    public class PiPlayerPref
    {
        string _prefix = string.Empty;

        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(_prefix + key);
        }

        internal PiPlayerPref()
        {

        }
        public PiPlayerPref CreateInstance()
        {
            return new PiPlayerPref();
        }

        public PiPlayerPref SetPrefix(string prefix)
        {
            _prefix = prefix;
            return this;
        }

        /// <summary>
        /// PlayerPref của unity không có get/set bool.
        /// Việc dùng tạm int thay cho bool dẫn đến việc khi getBool có thể nhầm lẫn giữa return false
        /// và chưa có pref nên ta
        /// dùng 2 số là 1 và -1 để quy định true/false thay vì dùng sô 0
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool GetBool(string key, bool defaultValue = false)
        {
            var i = GetInt(key);
            if (i == 0)
            {
                SetBool(key, defaultValue);
                return defaultValue;
            }
            else if (i == -1)
                return false;
            else
                return true;
        }

        public void SetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(_prefix + key, value ? 1 : -1);
        }

        public int GetInt(string key, int defaultValue = 0)
        {
            return PlayerPrefs.GetInt(_prefix + key, defaultValue);
        }

        public float GetFloat(string key, float defaultValue = 0f)
        {
            return PlayerPrefs.GetFloat(_prefix + key, defaultValue);
        }

        public void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(_prefix + key, value);
        }

        public void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(_prefix + key, value);
        }

        public string GetString(string key, string defaultValue = "")
        {
            return PlayerPrefs.GetString(_prefix + key, defaultValue);
        }

        public void SetString(string key, string value)
        {
            PlayerPrefs.SetString(_prefix + key, value);
        }

        public byte[] GetBinary(string key, byte[] defaultValue = null)
        {
            key = _prefix + key;
            
            if (PlayerPrefs.HasKey(key))
            {
                return System.Convert.FromBase64String(PlayerPrefs.GetString(key));
            }
            else
            {
                defaultValue ??= new byte[0];
                SetBinary(key, defaultValue);
                return defaultValue;
            }
        }

        public void SetBinary(string key, byte[] value)
        {
            PlayerPrefs.SetString(_prefix + key, System.Convert.ToBase64String(value));
        }

        public void Save()
        {
            PlayerPrefs.Save();
        }

        public void DeleteKey(string key)
        {
            PlayerPrefs.DeleteKey(_prefix + key);
        }


    }
}