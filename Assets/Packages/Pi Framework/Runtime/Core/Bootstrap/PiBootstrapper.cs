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
            PiBase.SystemEvents.OnInitializeAfterSceneLoad.Invoke();
            //Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterSceneLoad"));
        }

        /// <summary>
        /// Initializes the core components and services.
        /// </summary>
        static void Initialize()
        {
            PiBase.Status = SystemStatus.CoreInit;

            var services = PiServiceRegistry.Instance;

            Application.quitting -= OnAppQuitting;
            Application.quitting += OnAppQuitting;
            
            services.ResetRegistry();

            var systemEvents = new GameObject("Pi.systemEvents").AddComponent<PiSystemEvents>();
            services.AddServiceAndGameObject<PiSystemEvents>(systemEvents);
            PiBase.SystemEvents = systemEvents;

            var typeEvents = new EventBus();
            services.AddService(typeof(EventBus), typeEvents);
            PiBase.TypeEvents = typeEvents;

            var playerPrefs = new PiPlayerPref();
            services.AddService<IPlayerPrefs>(playerPrefs);
            PiBase.PlayerPrefs = playerPrefs;

            var console = new PiConsole();
            services.AddService<PiConsole>(console);
            PiBase.Console = console;
        }

        static void OnAppQuitting()
        {
            Application.quitting -= OnAppQuitting;
            PiBase.SystemEvents.OnAppQuitPhase2.Invoke();
            PiBase.SystemEvents.OnAppQuitPhase3.Register(SystemDestroy);
        }

        /// <summary>
        /// Những gì là gốc rễ và nhiều liên đới thì Destroy sau cùng
        /// </summary>
        internal static void SystemDestroy()
        {
            Debug.Log("System Destroyed");
            PiBase.Root = null;
            PiBase.PlayerPrefs = null;
            PiBase.Console = null;
            PiBase.SystemEvents = null;
            PiBase.TypeEvents?.Clear();
            PiBase.TypeEvents = null;
            PiBase.Status = SystemStatus.None;
        }

        internal static void Bootstrap(PiRoot root)
        {
            // This method is called to bootstrap the Pi Framework.
            PiBase.Root = root;
            PiBase.Status = SystemStatus.Configuration;
            Preload();//tạm thời đặt preload trước LoadSettings vì ta muốn phần LoadSettings có thể chạy batch commands.
            LoadSettings(root);
            RegisterAdditionServices();

            PiBase.Status = SystemStatus.ModulesInit;
            RegisterModules();
            PiBase.Status = SystemStatus.Ready;
        }

        static void Preload()
        {
            PreloadCommands();
        }

        static void RegisterAdditionServices()
        {
            // Register additional services here if needed
            // For example, you might want to register a custom logger or analytics service
            var services = PiServiceRegistry.Instance;
        }

        static void LoadSettings(PiRoot root)
        {
            var sm = root.GetComponentInChildren<RuntimeSettingsManager>();
            sm.LoadSettings();
        }

        static void RegisterModules()
        {
            //var modules = Object.FindObjectsByType<PiModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            var modules = PiBase.Root.gameObject.GetComponentsInChildren<PiModule>(true);
            PiServiceRegistry services = PiServiceRegistry.Instance;
            foreach (var m in modules)
            {
                services.AddService(m.GetType(), m);
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
            var console = PiBase.Console;
            console.RegisterCommand("exit", InternalCommands.TriggerExit);
            console.RegisterCommand("restart", InternalCommands.TriggerRestart);
        }

        //todo: hoàn chỉnh phần restart
        static internal void Reset()
        {
            PiBase.SystemEvents.Reset();
            //Reset ServiceLocator ở bước cuối cùng
            PiServiceRegistry.Instance.ResetRegistry();

            //re Initialize => sẽ gọi vào chỗ khác
            //Bootstrap();
        }
    }
}