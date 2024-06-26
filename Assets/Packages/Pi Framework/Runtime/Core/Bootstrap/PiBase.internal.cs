﻿using System.Collections;
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
        public static bool initialized { get; private set; }

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
        /// PiRoot.Awake được gọi trước các hàm Awake khác trong scene
        /// Do đó Pi.initialized cũng hoàn thành trước các Awake
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void InitBeforeSceneLoad()
        {
            //Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: BeforeSceneLoad"));
            Bootstrap();
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
            systemEvents.initializeAfterSceneLoad.Invoke();
            //Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterSceneLoad"));
        }

        public static bool isQuitting
        {
            get
            {
                return root == null || root.isQuitting;
            }
        }

        //RuntimeInitializeLoadType.BeforeSceneLoad
        static void Bootstrap()
        {
            initialized = false;
            Application.quitting -= OnAppQuitting;
            Application.quitting += OnAppQuitting;

            _services = PiServiceRegistry.instance;
            _services.Reset();
            systemEvents = new GameObject("Pi.systemEvents").AddComponent<PiSystemEvents>();
            _services.AddService(typeof(PiSystemEvents), systemEvents, systemEvents.gameObject);

            typeEvents = new TypeEventSystem();
            _services.AddService(typeof(TypeEventSystem), typeEvents);

            playerPrefs = new PiPlayerPref();
            _services.AddService<IPlayerPrefs>(playerPrefs);

            console = new PiConsole();
            _services.AddService<PiConsole>(console);

            //Debug.Log(InternalUtil.PiMessage("Pi bootstrapped"));
        }

        /// <summary>
        /// Bootstrap phase 2: Configuration
        /// </summary>
        /// <param name="piRoot">PiRoot</param>
        internal static void SystemStartup(PiRoot piRoot)
        {
            root = piRoot;
            //gameObject = piRoot.gameObject;
            Preload();
            LoadSettings(piRoot);
            InitModules();
            initialized = true;
        }

        /// <summary>
        /// Boostrap phase 3, thiết kế cho các bước tương tác 
        /// sau khi đã xây dựng hầu hết các thành phần hệ thống
        /// </summary>
        static void Preload()
        {
            PreloadCommands();
        }

        static void LoadSettings(PiRoot root)
        {
            var sm = root.GetComponentInChildren<RuntimeSettingsManager>();
            sm.LoadSettings();
        }

        static void InitModules()
        {
            var modules = Object.FindObjectsByType<PiModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var m in modules)
            {
                _services.AddService(m.GetType(), m);
            }
            //có thể cho settings push to modules chỗ này

            foreach (var m in modules)
            {
                m._moduleInit();
            }
            //Debug.Log(InternalUtil.PiMessage("Modules Initialized"));
        }

        //todo: hoàn chỉnh phần restart
        static internal void Reset()
        {
            systemEvents.Reset();
            //Reset ServiceLocator ở bước cuối cùng
            _services.Reset();

            //re Initialize => sẽ gọi vào chỗ khác
            Bootstrap();
        }

        static void PreloadCommands()
        {
            console.RegisterCommand("Exit", InternalCommands.TriggerExit);
            console.RegisterCommand("Restart", InternalCommands.TriggerRestart);
        }

        static void OnAppQuitting()
        {
            Application.quitting -= OnAppQuitting;
            systemEvents.AppQuitPhase2.Invoke();
            systemEvents.AppQuitPhase3.Register(SystemDestroy);
        }

        /// <summary>
        /// Những gì là gốc rễ và nhiều liên đới thì Destroy sau cùng
        /// </summary>
        internal static void SystemDestroy()
        {
            Debug.Log("SystemDestroyed");
            root = null;
            //gameObject = null;
            playerPrefs = null;
            console = null;
            systemEvents = null;
            _services = null;
            typeEvents?.Clear();
            typeEvents = null;
            initialized = false;
        }
    }
}