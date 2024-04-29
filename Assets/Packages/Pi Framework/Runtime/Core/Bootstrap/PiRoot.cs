using UnityEngine;
using System.Collections.Generic;
using System;
using PiFramework.Internal;


namespace PiFramework
{
    [RequireComponent(typeof(LatestExecOrder))]
    [ExecutionOrder(-31000)]
    public class PiRoot : MonoBehaviour
    {
        bool isSingleton;
        void Awake()
        {
            if (PiBase.root != null)
            {
                //Nếu chỉ dùng GameObject.Destroy thì các script con sẽ vẫn gọi Awake
                gameObject.SetActive(false);
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            isSingleton = true;

            Debug.Log(InternalUtil.PiMessage("PiRoot Awake"));
            GameObject.DontDestroyOnLoad(gameObject);
            PiBase.SystemStartup(this);

            DisplayServices();

            PiBase.systemEvents.beginAwake.Invoke();
        }

        #region Dispatch System Events

        void Start() => PiBase.systemEvents.beginStart.Invoke();

        void Update() => PiBase.systemEvents.beginUpdate.Invoke();

        void FixedUpdate() => PiBase.systemEvents.beginFixedUpdate.Invoke();

        void LateUpdate() => PiBase.systemEvents.beginLateUpdate.Invoke();

        internal bool isQuitting { get; private set; }

        private void OnApplicationQuit()
        {
            isQuitting = true;
            PiBase.systemEvents.AppQuitPhase1.Invoke();
        }

        private void OnDestroy()
        {
            if(isSingleton)
                PiBase.systemEvents.AppQuitPhase3.Invoke();
        }

        #endregion

        void DisplayServices()
        {
            PiBase._services.viewContainer.transform.parent = transform;
        }

    }

}