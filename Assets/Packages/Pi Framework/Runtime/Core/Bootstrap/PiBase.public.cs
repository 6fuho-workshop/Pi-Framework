using PiFramework.KeyValueStore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework
{
    public partial class PiBase
    {
        internal static PiServiceRegistry _services;
        public static IServiceRegistry services => _services;

        public static PiSystemEvents systemEvents { get; private set; }

        public static TypeEventSystem typeEvents { get; private set; }

        public static PiConsole console { get; private set; }

        public static IPlayerPrefs playerPrefs { get; private set; }

        /// <summary>
        /// the script attached to PiFramework GameObject
        /// </summary>
        public static PiRoot root { get; private set; }

        /// <summary>
        /// Root GameObject (PiFramework GameObject)
        /// </summary>
        //public static GameObject gameObject { get; private set; } //chua thuc su can thiet
    }
}