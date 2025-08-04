using UnityEngine;
using PiFramework.Internal;
using PiFramework.Settings;
using PiFramework.KeyValueStore;

namespace PiFramework
{
    public enum SystemStatus
    {
        None,
        CoreInit,
        Configuration,
        ModulesInit,
        Ready,
        Shutdown
    }

    public partial class PiBase
    {
        //internal static PiServiceRegistry _services;
        public static IServiceRegistry Services => PiServiceRegistry.Instance;

        public static PiSystemEvents SystemEvents { get; internal set; }

        public static EventBus TypeEvents { get; internal set; }

        public static PiConsole Console { get; internal set; }

        public static IPlayerPrefs PlayerPrefs { get; internal set; }

        /// <summary>
        /// the script attached to PiFramework GameObject
        /// </summary>
        public static PiRoot Root { get; internal set; }

        internal PiModule[] modules;
        /// <summary>
        /// Gets a value indicating whether the system has been initialized (Settings and modules loaded).
        /// </summary>
        public static SystemStatus Status { get; internal set; }
    }
}