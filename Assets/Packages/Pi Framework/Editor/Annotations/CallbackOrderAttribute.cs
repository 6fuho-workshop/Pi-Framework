using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.PiEditor.Callbacks
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public abstract class CallbackOrderAttribute : Attribute
    {
        protected int _callbackOrder;

        internal int callbackOrder => _callbackOrder;
    }
}