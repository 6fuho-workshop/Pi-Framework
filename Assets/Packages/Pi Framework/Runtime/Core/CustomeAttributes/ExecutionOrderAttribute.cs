using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

namespace PiFramework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ExecutionOrderAttribute : Attribute
    {
        public int Order;
        public ExecutionOrderAttribute(int order)
        {
            Order = order;
        }
    }
}