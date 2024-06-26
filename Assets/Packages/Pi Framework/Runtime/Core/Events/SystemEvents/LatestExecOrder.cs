﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PiFramework;
namespace PiFramework
{
    [ExecutionOrder(32000)]
    public class LatestExecOrder : MonoBehaviour
    {
        private void Awake()
        {
            PiBase.systemEvents.finalAwake.Invoke();
        }

        //void Start() => PiBootstrap.instance.systemEvents.FinalStart.Invoke();
        void Start() {
            PiBase.systemEvents.finalStart.Invoke();
        }

        void Update() => PiBase.systemEvents.finalUpdate.Invoke();

        internal void FixedUpdate() => PiBase.systemEvents.finalFixedUpdate.Invoke();

        void LateUpdate() => PiBase.systemEvents.finalLateUpdate.Invoke();

        private void OnApplicationQuit() => PiBase.systemEvents.AppQuitPhase1.Invoke();
    }
}