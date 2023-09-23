using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace PiFramework
{
    /// <summary>
    /// Add a Readonly Property in Pi class for quick access
    /// </summary>
    /// 
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [ComVisible(true)]
    [Obsolete]
    public class PinServiceAttribute : Attribute 
    {

        public string Name;
        public string PackageName;
        public Type Type;

        //public string Expression;
        public PinServiceAttribute(Type serviceType, string pinName, string packageName = "")
        {
            Name = pinName;
            Type = serviceType;
            PackageName = packageName;
        }
        
        public PinServiceAttribute(string pinName, string moduleName = "")
        {
            Name = pinName;
            PackageName = moduleName;
        }
    }
}