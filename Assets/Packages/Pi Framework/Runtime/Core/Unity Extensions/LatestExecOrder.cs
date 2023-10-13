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
            PiBootstrap.instance.systemEvents.FinalAwake.Invoke();
            print("LatestExecOrder awake");
        }

        //void Start() => PiBootstrap.instance.systemEvents.FinalStart.Invoke();
        void Start() {
            PiBootstrap.instance.systemEvents.FinalStart.Invoke();
            print("LatestExecOrder Start");
        }

        void Update() => PiBootstrap.instance.systemEvents.FinalUpdate.Invoke();

        internal void FixedUpdate() => PiBootstrap.instance.systemEvents.FinalFixedUpdate.Invoke();

        void LateUpdate() => PiBootstrap.instance.systemEvents.FinalLateUpdate.Invoke();

        private void OnApplicationQuit() => PiBootstrap.instance.systemEvents.FinalApplicationQuit.Invoke();
    }
}