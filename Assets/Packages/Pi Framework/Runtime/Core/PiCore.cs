﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PiFramework.Internal;


namespace PiFramework
{
    internal class PiCore
    {
        //internal PiServiceLocator serviceLocator => PiServiceLocator.instance;
        internal PiServiceLocator serviceLocator;
        internal PiSystemEvents systemEvents;
        internal PiApp app;
        internal PiConsole console;
        internal PiModule[] modules;

        /// <summary>
        /// static Constructor thực thi đầu tiên, tuy nhiên khi bỏ tính năng Reload Domain trong editor settings thì 
        /// static Constructor lại không được gọi.
        /// </summary>
        static PiCore()
        {
            Debug.Log("PiCore static ctor");
        }

        static PiCore _instance;
        internal static PiCore instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PiCore();
                return _instance;
            }
        }

        /// <summary>
        /// Ngay ở bước này thì Active Scene đã loaded nhưng chưa activate
        /// và đã có thể add/modify game object cho active scene.
        /// Tuy nhiên ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy (???)
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void SubsystemRegistration()
        {
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: SubsystemRegistration"));
        }

        /// <summary>
        /// chú ý: ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy(???)
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void InitAfterAssembliesLoaded()
        {
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterAssembliesLoaded"));
            //UnityEngine.SceneManagement.SceneManager.LoadScene("Play");
        }

        /// <summary>
        /// chú ý: ở android device thì mọi gameObject tạo ra trước 
        /// RuntimeInitializeLoadType.BeforeSceneLoad đều bị destroy (???)
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
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
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: BeforeSceneLoad"));
            instance.Initialize();
            instance.LoadConfiguraion();
        }

        /// <summary>
        /// các hàm Awake đã được chạy
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void InitAfterSceneLoad()
        {
            Debug.Log(InternalUtil.PiMessage("InitializeOnLoad: AfterSceneLoad"));
            SceneStartupHandle();
        }

        //todo: chuyển về class SceneStartup tự xử lý
        static void SceneStartupHandle()
        {
            SceneStartup startup = Object.FindAnyObjectByType<SceneStartup>();
            if (startup == null || !startup.enabled || string.IsNullOrEmpty(startup.redirect))
                return;

            Scene activeScene = SceneManager.GetActiveScene();
            foreach (var go in activeScene.GetRootGameObjects())
            {
                GameObject.DestroyImmediate(go);
            }
            SceneManager.LoadScene(startup.redirect);
        }


        void Initialize()
        {
            serviceLocator = PiServiceLocator.instance;
            serviceLocator.Reset();
            systemEvents = new GameObject("Pi.systemEvents").AddComponent<PiSystemEvents>();
            serviceLocator.AddService<PiProfiler>(new PiProfiler());
            serviceLocator.AddService<PiSystemEvents>(systemEvents, true);

            app = new PiApp();
            serviceLocator.AddService<PiApp>(app);

            serviceLocator.AddService<PiPlayerPref>(new PiPlayerPref());

            console = new PiConsole();
            serviceLocator.AddService<PiConsole>(console);
            Debug.Log(InternalUtil.PiMessage("Pi core Initialized"));
        }

        void LoadConfiguraion()
        {

        }
        /// <summary>
        /// Bootstrap phase 2: Configuration
        /// </summary>
        /// <param name="root">PiRoot</param>
        internal void  SystemAwake(PiRoot root)
        {
            serviceLocator.AddService<PiRoot>(root);
            SystemStartup();
            ModuleStartup();
        }

        /// <summary>
        /// Boostrap phase 3, thiết kế cho các bước tương tác 
        /// sau khi đã xây dựng hầu hết các thành phần hệ thống
        /// </summary>
        void SystemStartup()
        {
            PreloadCommands();
        }

        void ModuleStartup()
        {
            var modules = Object.FindObjectsByType<PiModule>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var m in modules)
            {
                serviceLocator.AddModule(m);
            }
            Debug.Log(InternalUtil.PiMessage("Modules registered"));
            //có thể cho settings push to modules chỗ này

            foreach (var m in modules)
            {
                m._moduleInit();
            }
            Debug.Log(InternalUtil.PiMessage("Modules Initialized"));
        }

        //todo: hoàn chỉnh phần restart
        internal void Restart()
        {
            systemEvents.Reset();
            //Reset ServiceLocator ở bước cuối cùng
            serviceLocator.Reset();

            //re Initialize
            Initialize();
        }

        void PreloadCommands()
        {
            console.RegisterCommand("Exit", InternalCommands.TriggerExit);
            console.RegisterCommand("Restart", InternalCommands.TriggerRestart);
        }

        internal void Destroy()
        {
            _instance = null;
        }
    }
}