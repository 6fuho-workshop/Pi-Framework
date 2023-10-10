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

            PiBootstrap.instance.SystemAwake(this);

            DisplayServices();
            PiBootstrap.instance.systemEvents.BeginAwake.Invoke();
            Application.quitting += () => PiBootstrap.SystemDestroy();
        }

        //Dispatch System Event
        void Start()
        {
            Debug.Log(InternalUtil.PiMessage("PiRoot Start"));
            PiBootstrap.instance.systemEvents.BeginStart.Invoke();
        }

        //Dispatch System Event
        void Update()
        {
            PiBootstrap.instance.systemEvents.BeginUpdate.Invoke();
        }

        //Dispatch System Event
        void LateUpdate()
        {
            PiBootstrap.instance.systemEvents.BeginLateUpdate.Invoke();
        }

        //Dispatch System Event
        void OnGUI()
        {
            PiBootstrap.instance.systemEvents.BeginOnGUI.Invoke();
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
            PiBootstrap.instance.systemEvents.BeginApplicationQuit.Invoke();
        }

        #endregion behaviours

        void DisplayServices()
        {
            PiServiceLocator.instance.container.transform.parent = transform;
        }

    }

}