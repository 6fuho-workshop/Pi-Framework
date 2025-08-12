using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace PF.Internal
{
    internal class InternalUtil
    {
        [Obsolete]
        public static string PiMessage(string msg)
        {
            return $"<color=#ffff88>{msg}</color>";
        }
    }
}