using UnityEngine;
using System.Collections.Generic;
using System;
using PiFramework.Internal;

namespace PiFramework
{
    [ExecutionOrder(-32000)]
    [RequireComponent(typeof(HighestExecOrder))]
    public sealed class PiRoot : MonoBehaviour
    {
        //public bool EnableProfilerOnStart;
        internal static PiRoot instance;

        #region run in background
        /*
        [SerializeField]
        [Tooltip("Should the Player be running when the application is in the background?" +
            "\nThis property is ignored on Android and iOS.")]
        bool _runInBackground = true;

        /// <summary>
        /// Should the Player be running when the application is in the background?
        /// This property is ignored on Android and iOS.
        /// </summary>
        public bool runInBackground
        {
            get => _runInBackground;
            set
            {
                _runInBackground = value;
                Application.runInBackground = _runInBackground;
            }
        }
        */
        #endregion run in background

        #region behaviours
        void Awake()
        {
            if (instance != null)
            {
                //Nếu chỉ dùng GameObject.Destroy thì các script con sẽ vẫn gọi Awake
                gameObject.SetActive(false);
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            Debug.Log(InternalUtil.PiMessage("PiRoot Awake"));
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);

            PiCore.instance.SystemAwake(this);

            //if (EnableProfilerOnStart)
                //PiCore.instance.serviceLocator.GetService<PiProfiler>().Enabled = true;
            DisplayServices(); //đang không hiểu vì sao phần display service này chạy trên android bị lỗi 

            PiCore.instance.systemEvents.BeginAwake.Invoke();
            //Debug.Log(InternalUtil.PiMessage("PiRoot Awake Done"));

            //runInBackground = _runInBackground;
            Application.quitting += OnQuitting;
        }

        //Dispatch System Event
        void Start()
        {
            Debug.Log(InternalUtil.PiMessage("PiRoot Start"));
            PiCore.instance.systemEvents.BeginStart.Invoke();
        }

        void OnQuitting()
        {
            instance = null;
            PiCore.instance.Destroy();
        }

        void OnUnloading()
        {
            Debug.Log("OnUnloading");
        }
        bool OnWantsToQuit()
        {
            Debug.Log("OnWantsToQuit");
            return false;
        }

        //Dispatch System Event
        void Update()
        {
            PiCore.instance.systemEvents.BeginUpdate.Invoke();
        }

        //Dispatch System Event
        void LateUpdate()
        {
            PiCore.instance.systemEvents.BeginLateUpdate.Invoke();
        }

        //Dispatch System Event
        void OnGUI()
        {
            PiCore.instance.systemEvents.BeginOnGUI.Invoke();
        }

        bool _isQuitting;
        /// <summary>
        /// 
        /// </summary>
        public bool isQuitting
        {
            get { return _isQuitting; }
        }
        private void OnApplicationQuit()
        {
            _isQuitting = true;
            PiCore.instance.systemEvents.BeginApplicationQuit.Invoke();
        }

        #endregion behaviours

        void DisplayServices()
        {
            PiServiceLocator.instance.container.transform.parent = transform;
        }

    }

}