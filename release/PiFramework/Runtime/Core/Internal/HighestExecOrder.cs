using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PiFramework;
namespace PiFramework.Internal
{
    [ExecutionOrder(32000)]
    internal class HighestExecOrder : MonoBehaviour
    {
        private void Awake()
        {
            PiCore.instance.systemEvents.FinalAwake.Invoke();
        }

        //Dispatch System Event
        void Start()
        {
            PiCore.instance.systemEvents.FinalStart.Invoke();
        }

        //Dispatch System Event
        void Update()
        {
            PiCore.instance.systemEvents.FinalUpdate.Invoke();
        }

        //Dispatch System Event
        void LateUpdate()
        {
            PiCore.instance.systemEvents.FinalLateUpdate.Invoke();
        }

        //Dispatch System Event
        void OnGUI()
        {
            PiCore.instance.systemEvents.FinalOnGUI.Invoke();
        }

        private void OnApplicationQuit()
        {
            PiCore.instance.systemEvents.FinalApplicationQuit.Invoke();
        }
    }
}