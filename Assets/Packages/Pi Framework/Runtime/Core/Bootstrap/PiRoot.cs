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
        EarliestExecOrder earliest = new();

        
        void Awake()
        {
            if (PiBase.root != null)
            {
                //Nếu chỉ dùng GameObject.Destroy thì các script con sẽ vẫn gọi Awake
                gameObject.SetActive(false);
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            //Debug.Log(InternalUtil.PiMessage("PiRoot Awake"));
            GameObject.DontDestroyOnLoad(gameObject);
            PiBase.SystemStartup(this);

            DisplayServices();
            earliest.Awake();
        }

        #region Dispatch System Events

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

        #endregion

        void DisplayServices()
        {
            PiBase.services.viewContainer.transform.parent = transform;
        }

    }

}