using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PiFramework;
namespace PiFramework
{
    internal class EarliestExecOrder
    {
        internal void Awake() => PiBase.systemEvents.BeginAwake.Invoke();

        internal void Start() => PiBase.systemEvents.BeginStart.Invoke();

        internal void Update() => PiBase.systemEvents.BeginUpdate.Invoke();

        internal void FixedUpdate() => PiBase.systemEvents.BeginFixedUpdate.Invoke();

        internal void LateUpdate() => PiBase.systemEvents.BeginLateUpdate.Invoke();

        internal void OnApplicationQuit() => PiBase.systemEvents.BeginApplicationQuit.Invoke();
    }
}