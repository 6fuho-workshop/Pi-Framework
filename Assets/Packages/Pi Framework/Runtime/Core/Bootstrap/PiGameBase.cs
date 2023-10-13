using UnityEngine;
using System.Collections.Generic;
using System;
using PiFramework.Internal;
using UnityEditor.SceneManagement;

namespace PiFramework
{
    //[RequireComponent(typeof(LatestExecOrder))]
    public class PiGameBase : MonoBehaviour
    {
        internal static PiGameBase instance;
        EarliestExecOrder earliest = new();

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
            gameObject.AddComponent<LatestExecOrder>();
            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);

            PiBootstrap.instance.SystemAwake(this);

            DisplayServices();
            earliest.Awake();
            Application.quitting += () => PiBootstrap.SystemDestroy();
        }

        //Dispatch System Event
        void Start() => earliest.Start();

        void Update() => earliest.Update();

        void FixedUpdate() => earliest.FixedUpdate();

        void LateUpdate() => earliest.LateUpdate();

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
            earliest.OnApplicationQuit();
        }

        #endregion behaviours

        void DisplayServices()
        {
            PiServiceLocator.instance.container.transform.parent = transform;
        }

    }

}