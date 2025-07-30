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
        public static IServiceRegistry services => PiServiceRegistry.instance;

        public static PiSystemEvents systemEvents { get; internal set; }

        public static EventBus typeEvents { get; internal set; }

        public static PiConsole console { get; internal set; }

        public static PiOperationManager operation { get; internal set; }

        public static IPlayerPrefs playerPrefs { get; internal set; }

        /// <summary>
        /// the script attached to PiFramework GameObject
        /// </summary>
        public static PiRoot root { get; internal set; }

        internal PiModule[] modules;
        /// <summary>
        /// Gets a value indicating whether the system has been initialized (Settings and modules loaded).
        /// </summary>
        public static SystemStatus status { get; internal set; }
    }
}