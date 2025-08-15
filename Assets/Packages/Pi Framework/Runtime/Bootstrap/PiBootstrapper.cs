using PF.Logging;
using PF.Core.Services.Unity;
using PF.KeyValueStore;
using PF.Core.Settings;
using System;
using UnityEngine;
using PF.Core.Services;
using ILogger = PF.Logging.ILogger;

namespace PF.Internal
{
    /// <summary>
    /// Provides a set of initialization methods that are executed at various stages of the Unity runtime lifecycle.
    /// </summary>
    /// <remarks>The <see cref="PiBootstrapper"/> class contains static methods marked with <see
    /// cref="RuntimeInitializeOnLoadMethodAttribute"/>  to perform specific initialization tasks at predefined points
    /// during the Unity runtime startup process. These methods are  executed automatically by Unity and are used to set
    /// up the Pi framework and related components.  Note that certain behaviors may vary depending on the platform. For
    /// example, on Android devices, any GameObjects created  before <see
    /// cref="RuntimeInitializeLoadType.BeforeSceneLoad"/> may be destroyed.</remarks>
    public static class PiBootstrapper
    {
        static ILogger logger;
        static bool _initialized; 

        /// <summary>
        /// Ngay ở bước này thì Active Scene đã loaded nhưng chưa activate
        /// và đã có thể add/modify game object cho active scene.
        /// Tuy nhiên ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy (???)
        /// </summary>
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SubsystemRegistration()
        {

        }

        /// <summary>
        /// chú ý: ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy(???)
        /// </summary>
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void InitAfterAssembliesLoaded()
        {
            Debug.Log("InitializeOnLoad: AfterAssembliesLoaded");
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Play");
        }

        /// <summary>
        /// chú ý: ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy (???)
        /// </summary>
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void InitBeforeSplashScreen()
        {
            Debug.Log("InitializeOnLoad: BeforeSplashScreen");
        }

        /// <summary>
        /// các hàm Awake sẽ chạy sau bước này và trước AfterSceneLoad. 
        /// Lúc này nếu gọi activeScene.GetRootGameObjects() sẽ empty
        /// PiRoot.Awake được gọi trước các hàm Awake khác trong scene
        /// Do đó Pi.initialized cũng hoàn thành trước các Awake
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitBeforeSceneLoad()
        {
            Initialize();

            var pi = GameObject.Instantiate(Resources.Load<GameObject>("PiFramework"));
            pi.name = "PiFramework";
            logger.Trace("Ready for Scene Load");
        }

        /// <summary>
        /// các hàm Awake đã được chạy
        /// Toàn bộ RuntimeInitializeOnLoadMethods chỉ được chạy 1 lần duy nhất
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitAfterSceneLoad()
        {
            logger.Trace("InitAfterSceneLoad");
            P.SystemEvents.OnInitializeAfterSceneLoad.Invoke();
            //Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterSceneLoad"));
        }

        /// <summary>
        /// Initializes the core components and services.
        /// </summary>
        static void Initialize()
        {
            P.Status = SystemStatus.None;

            if (_initialized)
            {
                ResetStatics();
            }
                
            _initialized = true;

            //var log = new PiLog();
            //P.Log = log;
            Log.Init(useUnitySink: true, filePath: "log.txt");

            logger = Log.Bootstrap;
            logger.Trace("PiFramework Initializing (BeforeSceneLoad)");
            P.Status = SystemStatus.CoreInit;

            Application.quitting -= OnAppQuitting;
            Application.quitting += OnAppQuitting;

            var services = new PiServiceRegistry();
            P.Services = services;
            //services.Clear(disposeInstances: true, removeFactories: true);

            //var systemEvents = new GameObject("Pi.systemEvents").AddComponent<PiSystemEvents>();
            //services.RegisterWithGO<PiSystemEvents>(systemEvents);
            P.SystemEvents = new PiSystemEvents();

            var typeEvents = new EventBus();
            services.Register(typeof(EventBus), typeEvents);
            P.EventBus = typeEvents;

            var playerPrefs = new PiPlayerPref();
            services.Register<IPlayerPrefs>(playerPrefs);
            P.PlayerPrefs = playerPrefs;

            var console = new PiConsole();
            services.Register<PiConsole>(console);
            P.Console = console;

            logger.Trace("PiFramework Initialized");
        }
        

        static void OnAppQuitting()
        {
            Application.quitting -= OnAppQuitting;
            P.SystemEvents.OnAppQuitPhase2.Invoke();
            P.SystemEvents.OnAppQuitPhase3.Register(SystemDestroy);
        }


        internal static void SystemDestroy()
        {
            logger.Trace("System Destroying");
            ResetStatics();
        }

        /// <summary>
        /// Những gì là gốc rễ và nhiều liên đới thì Destroy sau cùng
        /// </summary>
        internal static void ResetStatics()
        {
            if (!_initialized)
                return; // Không cần reset nếu chưa khởi tạo
            
            P.Root = null;
            P.PlayerPrefs = null;
            P.Console = null;
            P.EventBus?.Clear();
            P.EventBus = null;
            logger = null;
            Log.Shutdown();
            _initialized = false;
            
            P.SystemEvents.Reset();
            P.SystemEvents = null;

            P.Services.Clear(disposeInstances: true, removeFactories: true);
            P.Services = null;

        }

        internal static void Bootstrap(PiRoot root)
        {
            // This method is called to bootstrap the Pi Framework.
            P.Root = root;
            P.Status = SystemStatus.Configuration;
            Preload();//tạm thời đặt preload trước LoadSettings vì ta muốn phần LoadSettings có thể chạy batch commands.
            LoadSettings(root);
            RegisterAdditionServices();

            P.Status = SystemStatus.ModulesInit;
            RegisterModules();
            P.Status = SystemStatus.Ready;
        }

        static void Preload()
        {
            PreloadCommands();
        }

        static void RegisterAdditionServices()
        {
            // Register additional services here if needed
            // For example, you might want to register a custom logger or analytics service
        }

        static void LoadSettings(PiRoot root)
        {
            var sm = root.GetComponentInChildren<RuntimeSettingsManager>();
            sm.LoadSettings();
        }

        static void RegisterModules()
        {
            //var modules = Object.FindObjectsByType<PiModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var modules = P.Root.gameObject.GetComponentsInChildren<PiModule>(true);

            IServiceRegistry services = P.Services;

            foreach (var m in modules)
            {
                services.Register(m.GetType(), m);
            }
            //có thể cho settings push to modules chỗ này

            foreach (var m in modules)
            {
                m.ModuleInitInternal();
            }
            //Debug.Log(InternalUtil.PiMessage("Modules Initialized"));
        }

        static void PreloadCommands()
        {
            var console = P.Console;
            console.RegisterCommand("exit", InternalCommands.TriggerExit);
            console.RegisterCommand("restart", InternalCommands.TriggerRestart);
        }

        //todo: hoàn chỉnh phần restart
        static internal void Reset()
        {
            //P.SystemEvents.Reset();
            //Reset ServiceLocator ở bước cuối cùng
            //PiServiceRegistry.Instance.Clear(disposeInstances: true, removeFactories: true);

            //re Initialize => sẽ gọi vào chỗ khác
            //Bootstrap();
        }
    }
}