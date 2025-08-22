using PF.Internal;
using PF.KeyValueStore;
using PF.Events;
using PF.DI;
using PF.Contracts;

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

    public class Pi
    {
        //internal static PiServiceRegistry _services;
        public static IServiceRegistry Services { get; internal set; }

        internal static PiSystemEvents systemEvents;
        public static ISystemEvents SystemEvents => systemEvents;

        public static IEventBus EventBus { get; internal set; }

        public static IPiConsole Console { get; internal set; }

        public static IPlayerPrefs PlayerPrefs { get; internal set; }

        internal static SystemConfiguration Config { get; set; }
        /// <summary>
        /// the script attached to PiFramework GameObject
        /// </summary>
        public static PiRoot Root { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether the system has been initialized (Settings and modules loaded).
        /// </summary>
        public static SystemStatus Status { get; internal set; }

        protected Pi()
        {
        }
    }
}