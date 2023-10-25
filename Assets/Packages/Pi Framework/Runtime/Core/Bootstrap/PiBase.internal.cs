using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PiFramework.Internal;
using PiFramework.Settings;
using System.Runtime.InteropServices;
using PiFramework.KeyValueStore;

namespace PiFramework
{
    public partial class PiBase
    {
        internal PiModule[] modules;

        /// <summary>
        /// static Constructor thực thi đầu tiên, tuy nhiên khi bỏ tính năng Reload Domain trong editor settings thì 
        /// static Constructor lại không được gọi.
        /// </summary>
        //static PiBase()
        //{
            //Debug.Log("PiCore static ctor");
        //}

        

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
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitBeforeSceneLoad()
        {
            //Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: BeforeSceneLoad"));
            Bootstrap();
        }

        /// <summary>
        /// các hàm Awake đã được chạy
        /// </summary>
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitAfterSceneLoad()
        {
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterSceneLoad"));
        }

        //RuntimeInitializeLoadType.BeforeSceneLoad
        static void Bootstrap()
        {
            services = PiServiceLocator.instance;
            services.Reset();
            systemEvents = new GameObject("Pi.systemEvents").AddComponent<PiSystemEvents>();
            services.AddService<PiSystemEvents>(systemEvents, true);

            services.AddService<PiPlayerPref>(new PiPlayerPref());

            console = new PiConsole();
            services.AddService<PiConsole>(console);
            Debug.Log(InternalUtil.PiMessage("Pi bootstrap Initialized"));
        }

        /// <summary>
        /// Bootstrap phase 2: Configuration
        /// </summary>
        /// <param name="root">PiRoot</param>
        internal static void SystemStartup(PiLoader root)
        {
            Preload();
            LoadSettings(root);
            InitModules();
        }

        /// <summary>
        /// Boostrap phase 3, thiết kế cho các bước tương tác 
        /// sau khi đã xây dựng hầu hết các thành phần hệ thống
        /// </summary>
        static void Preload()
        {
            PreloadCommands();
        }

        static void LoadSettings(PiLoader root)
        {
            var sm = root.GetComponentInChildren<SettingManager>();
            sm.LoadSettings();
        }

        static void InitModules()
        {
            var modules = Object.FindObjectsByType<PiModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var m in modules)
            {
                services.AddModule(m);
            }
            //có thể cho settings push to modules chỗ này

            foreach (var m in modules)
            {
                m._moduleInit();
            }
            Debug.Log(InternalUtil.PiMessage("Modules Initialized"));
        }

        //todo: hoàn chỉnh phần restart
        static internal void Reset()
        {
            systemEvents.Reset();
            //Reset ServiceLocator ở bước cuối cùng
            services.Reset();

            //re Initialize => sẽ gọi vào chỗ khác
            Bootstrap();
        }

        static void PreloadCommands()
        {
            console.RegisterCommand("Exit", InternalCommands.TriggerExit);
            console.RegisterCommand("Restart", InternalCommands.TriggerRestart);
        }

        /// <summary>
        /// Những gì là gốc rễ và ít liên đới thì Destroy sau cùng
        /// </summary>
        internal static void SystemDestroy()
        {
            SettingManager.Destroy();
            PiLoader.instance = null;
        }
    }
}