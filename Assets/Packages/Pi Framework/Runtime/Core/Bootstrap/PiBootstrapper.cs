using PiFramework.Internal;
using PiFramework.KeyValueStore;
using PiFramework.Settings;
using UnityEngine;

namespace PiFramework.Internal
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
    public class PiBootstrapper
    {
        /// <summary>
        /// Ngay ở bước này thì Active Scene đã loaded nhưng chưa activate
        /// và đã có thể add/modify game object cho active scene.
        /// Tuy nhiên ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy (???)
        /// </summary>
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SubsystemRegistration()
        {
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: SubsystemRegistration"));
        }

        /// <summary>
        /// chú ý: ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy(???)
        /// </summary>
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void InitAfterAssembliesLoaded()
        {
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterAssembliesLoaded"));
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Play");
        }

        /// <summary>
        /// chú ý: ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy (???)
        /// </summary>
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void InitBeforeSplashScreen()
        {
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: BeforeSplashScreen"));
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
            //Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: BeforeSceneLoad"));
            Initialize();
            var pi = GameObject.Instantiate(Resources.Load<GameObject>("PiFramework"));
            pi.name = "PiFramework";
        }

        /// <summary>
        /// các hàm Awake đã được chạy
        /// Toàn bộ RuntimeInitializeOnLoadMethods chỉ được chạy 1 lần duy nhất
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitAfterSceneLoad()
        {
            PiBase.systemEvents.OnInitializeAfterSceneLoad.Invoke();
            //Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterSceneLoad"));
        }

        /// <summary>
        /// Initializes the core components and services.
        /// </summary>
        static void Initialize()
        {
            PiBase.status = SystemStatus.CoreInit;

            var services = PiServiceRegistry.instance;

            Application.quitting -= OnAppQuitting;
            Application.quitting += OnAppQuitting;
            
            services.Reset();

            var systemEvents = new GameObject("Pi.systemEvents").AddComponent<PiSystemEvents>();
            services.AddService(typeof(PiSystemEvents), systemEvents, systemEvents.gameObject);
            PiBase.systemEvents = systemEvents;

            var typeEvents = new EventBus();
            services.AddService(typeof(EventBus), typeEvents);
            PiBase.typeEvents = typeEvents;

            var playerPrefs = new PiPlayerPref();
            services.AddService<IPlayerPrefs>(playerPrefs);
            PiBase.playerPrefs = playerPrefs;

            var console = new PiConsole();
            services.AddService<PiConsole>(console);
            PiBase.console = console;
        }

        static void OnAppQuitting()
        {
            Application.quitting -= OnAppQuitting;
            PiBase.systemEvents.OnAppQuitPhase2.Invoke();
            PiBase.systemEvents.OnAppQuitPhase3.Register(SystemDestroy);
        }

        /// <summary>
        /// Những gì là gốc rễ và nhiều liên đới thì Destroy sau cùng
        /// </summary>
        internal static void SystemDestroy()
        {
            Debug.Log("System Destroyed");
            PiBase.root = null;
            PiBase.playerPrefs = null;
            PiBase.console = null;
            PiBase.systemEvents = null;
            PiBase.typeEvents?.Clear();
            PiBase.typeEvents = null;
            PiBase.status = SystemStatus.None;
        }

        internal static void Bootstrap(PiRoot root)
        {
            // This method is called to bootstrap the Pi Framework.
            PiBase.root = root;
            PiBase.status = SystemStatus.Configuration;
            Preload();//tạm thời đặt preload trước LoadSettings vì ta muốn phần LoadSettings có thể chạy batch commands.
            LoadSettings(root);
            RegisterAdditionServices();

            PiBase.status = SystemStatus.ModulesInit;
            RegisterModules();
            PiBase.status = SystemStatus.Ready;
        }

        static void Preload()
        {
            PreloadCommands();
        }

        static void RegisterAdditionServices()
        {
            // Register additional services here if needed
            // For example, you might want to register a custom logger or analytics service
            var services = PiServiceRegistry.instance;

            var opManager = new GameObject("Pi.operation").AddComponent<PiOperationManager>();
            services.AddService(typeof(PiOperationManager), opManager, opManager.gameObject);
            PiBase.operation = opManager;
        }

        static void LoadSettings(PiRoot root)
        {
            var sm = root.GetComponentInChildren<RuntimeSettingsManager>();
            sm.LoadSettings();
        }

        static void RegisterModules()
        {
            //var modules = Object.FindObjectsByType<PiModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var modules = PiBase.root.gameObject.GetComponentsInChildren<PiModule>(true);
            PiServiceRegistry services = PiServiceRegistry.instance;
            foreach (var m in modules)
            {
                services.AddService(m.GetType(), m);
            }
            //có thể cho settings push to modules chỗ này

            foreach (var m in modules)
            {
                m._moduleInit();
            }
            //Debug.Log(InternalUtil.PiMessage("Modules Initialized"));
        }

        static void PreloadCommands()
        {
            var console = PiBase.console;
            console.RegisterCommand("exit", InternalCommands.TriggerExit);
            console.RegisterCommand("restart", InternalCommands.TriggerRestart);
        }

        //todo: hoàn chỉnh phần restart
        static internal void Reset()
        {
            PiBase.systemEvents.Reset();
            //Reset ServiceLocator ở bước cuối cùng
            PiServiceRegistry.instance.Reset();

            //re Initialize => sẽ gọi vào chỗ khác
            //Bootstrap();
        }
    }
}