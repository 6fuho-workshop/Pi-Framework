using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PiFramework;
namespace PiFramework
{
    [ExecutionOrder(32000)]
    internal class LatestExecOrder : MonoBehaviour
    {
        private void Awake()
        {
            PiBase.systemEvents.FinalAwake.Invoke();
        }

        //void Start() => PiBootstrap.instance.systemEvents.FinalStart.Invoke();
        void Start() {
            PiBase.systemEvents.FinalStart.Invoke();
        }

        void Update() => PiBase.systemEvents.FinalUpdate.Invoke();

        internal void FixedUpdate() => PiBase.systemEvents.FinalFixedUpdate.Invoke();

        void LateUpdate() => PiBase.systemEvents.FinalLateUpdate.Invoke();

        private void OnApplicationQuit() => PiBase.systemEvents.FinalApplicationQuit.Invoke();
    }
}