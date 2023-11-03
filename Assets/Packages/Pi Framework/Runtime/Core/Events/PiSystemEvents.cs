using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PiFramework;

namespace PiFramework
{
    /// <summary>
    /// Chứa các Unityevent liên quan đến vòng lặp và các event hệ thống
    /// </summary>
    public class PiSystemEvents: MonoBehaviour
    {
        //game init event
        public UnityEvent BeginAwake = new();
        public UnityEvent FinalAwake = new();
        public UnityEvent BeginStart = new();
        public UnityEvent FinalStart = new();

        //game loop event
        public UnityEvent BeginUpdate = new();
        public UnityEvent FinalUpdate = new();

        public UnityEvent BeginFixedUpdate = new();
        public UnityEvent FinalFixedUpdate = new();

        public UnityEvent BeginLateUpdate = new();
        public UnityEvent FinalLateUpdate = new();

        //public UnityEvent BeginOnGUI = new();
        //public UnityEvent FinalOnGUI = new();


        /// <summary>
        /// occurs when user click exit or back button on menu screen, args is a PendingInvoker
        /// </summary>
        public UnityEvent<PendingInvoker> OnTriggerShutdown = new UnityEvent<PendingInvoker>();
        public UnityEvent BeginApplicationQuit = new();
        public UnityEvent FinalApplicationQuit = new();


        //Common
        public UnityEvent UserPaused = new();
        public UnityEvent UserUnpaused = new();

        internal PiSystemEvents() { }

        internal void Reset()
        {
            BeginAwake.RemoveAllListeners();
            FinalAwake.RemoveAllListeners();
            BeginStart.RemoveAllListeners();
            FinalStart.RemoveAllListeners();
            BeginUpdate.RemoveAllListeners();
            FinalUpdate.RemoveAllListeners();
            BeginFixedUpdate.RemoveAllListeners();
            FinalFixedUpdate.RemoveAllListeners();
            BeginLateUpdate.RemoveAllListeners();
            FinalLateUpdate.RemoveAllListeners();
            //BeginOnGUI.RemoveAllListeners();
            
            OnTriggerShutdown.RemoveAllListeners();
            BeginApplicationQuit.RemoveAllListeners();
            FinalApplicationQuit.RemoveAllListeners();
            UserPaused.RemoveAllListeners();
            UserUnpaused.RemoveAllListeners();
        }
    }
}