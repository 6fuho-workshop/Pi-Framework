using UnityEngine;
using System.Collections.Generic;
using System;
using PiFramework.Internal;


namespace PiFramework
{
    [RequireComponent(typeof(LatestExecOrder))]
    [ExecutionOrder(-31000)]
    public class PiLoader : MonoBehaviour
    {
        internal static PiLoader instance;
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

            instance = this;
            GameObject.DontDestroyOnLoad(gameObject);

            PiBase.SystemStartup(this);

            DisplayServices();
            earliest.Awake();
            Application.quitting += () => PiBase.SystemDestroy();
        }

        //Dispatch System Event
        void Start() => earliest.Start();

        void Update() => earliest.Update();

        void FixedUpdate() => earliest.FixedUpdate();

        void LateUpdate() => earliest.LateUpdate();

        public bool isQuitting { get; private set; }

        private void OnApplicationQuit()
        {
            isQuitting = true;
            earliest.OnApplicationQuit();
        }

        #endregion behaviours

        void DisplayServices()
        {
            PiServiceLocator.instance.container.transform.parent = transform;
        }

    }

}