using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ModuleObjectNameAttribute : Attribute
    {
        public string Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">Module Name</param>
        public ModuleObjectNameAttribute(string name)
        {
            Name = name;
        }
    }
}