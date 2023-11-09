using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PiFramework;
namespace PiFramework
{
    internal class EarliestExecOrder
    {
        internal void Awake() => PiBase.systemEvents.beginAwake.Invoke();

        internal void Start() => PiBase.systemEvents.beginStart.Invoke();

        internal void Update() => PiBase.systemEvents.beginUpdate.Invoke();

        internal void FixedUpdate() => PiBase.systemEvents.beginFixedUpdate.Invoke();

        internal void LateUpdate() => PiBase.systemEvents.beginLateUpdate.Invoke();

        internal void OnApplicationQuit() => PiBase.systemEvents.beginAppQuit.Invoke();
    }
}