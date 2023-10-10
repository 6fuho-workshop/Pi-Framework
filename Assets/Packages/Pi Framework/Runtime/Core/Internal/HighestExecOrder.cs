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
            PiBootstrap.instance.systemEvents.FinalAwake.Invoke();
        }

        //Dispatch System Event
        void Start()
        {
            PiBootstrap.instance.systemEvents.FinalStart.Invoke();
        }

        //Dispatch System Event
        void Update()
        {
            PiBootstrap.instance.systemEvents.FinalUpdate.Invoke();
        }

        //Dispatch System Event
        void LateUpdate()
        {
            PiBootstrap.instance.systemEvents.FinalLateUpdate.Invoke();
        }

        //Dispatch System Event
        void OnGUI()
        {
            PiBootstrap.instance.systemEvents.FinalOnGUI.Invoke();
        }

        private void OnApplicationQuit()
        {
            PiBootstrap.instance.systemEvents.FinalApplicationQuit.Invoke();
        }
    }
}