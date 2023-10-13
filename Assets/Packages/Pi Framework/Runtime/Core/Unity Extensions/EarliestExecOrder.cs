using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PiFramework;
namespace PiFramework
{
    internal class EarliestExecOrder
    {
        internal void Awake() => PiBootstrap.instance.systemEvents.BeginAwake.Invoke();

        internal void Start() => PiBootstrap.instance.systemEvents.BeginStart.Invoke();

        internal void Update() => PiBootstrap.instance.systemEvents.BeginUpdate.Invoke();

        internal void FixedUpdate() => PiBootstrap.instance.systemEvents.BeginFixedUpdate.Invoke();

        internal void LateUpdate() => PiBootstrap.instance.systemEvents.BeginLateUpdate.Invoke();

        internal void OnApplicationQuit() => PiBootstrap.instance.systemEvents.BeginApplicationQuit.Invoke();
    }
}