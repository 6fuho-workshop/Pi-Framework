using PF.Internal;
using PF.KeyValueStore;
using PF.Core.Services;
using PF.Logging;

namespace PF
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

    public partial class P
    {
        //internal static PiServiceRegistry _services;
        public static IServiceRegistry Services { get; internal set; }

        public static PiSystemEvents SystemEvents { get; internal set; }

        public static EventBus EventBus { get; internal set; }

        public static PiConsole Console { get; internal set; }

        public static IPlayerPrefs PlayerPrefs { get; internal set; }

        /// <summary>
        /// the script attached to PiFramework GameObject
        /// </summary>
        public static PiRoot Root { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the system has been initialized (Settings and modules loaded).
        /// </summary>
        public static SystemStatus Status { get; internal set; }
    }
}