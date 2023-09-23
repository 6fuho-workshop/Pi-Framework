using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PiFramework.Internal;
using PiFramework;


public static partial class Pi
{
    #region Initialize

    public static PiServiceLocator serviceLocator { get; } = PiServiceLocator.instance;
    static Pi()
    {
        //ServiceLocator = PiServiceLocator.Instance;
    }

    #endregion Initialize


    #region Static Variables

    static Dictionary<string, object> _vars = new Dictionary<string, object>();
    static Dictionary<string, Action> _varListeners = new Dictionary<string, Action>();
    public static T GetVar<T>(string key)
    {
        return GetVar<T>(key, default(T));
    }

    public static void AddVarChangeHandler(string key, Action callback)
    {
        if (_varListeners.ContainsKey(key))
        {
            _varListeners[key] += callback;
        }
        else
        {
            _varListeners[key] = callback;
        }
    }

    public static void RemoveVarChangeHandler(string key, Action callback)
    {
        if (_varListeners.ContainsKey(key))
            _varListeners[key] -= callback;
    }

    public static T GetVar<T>(string key, object defaultValue)
    {
        object tmp;
        if (_vars.TryGetValue(key, out tmp))
        {
            return (T)tmp;
        }
        else
        {
            _vars[key] = defaultValue;
            return (T)defaultValue;
        }
    }

    public static void SetVar(string key, object value)
    {
        _vars[key] = value;
        Action tmp;
        if (_varListeners.TryGetValue(key, out tmp))
        {
            if (tmp != null)
                tmp.Invoke();
        }
    }

    public static void DeleteVar(string key)
    {
        _vars.Remove(key);
    }

    #endregion Static Variables
}
