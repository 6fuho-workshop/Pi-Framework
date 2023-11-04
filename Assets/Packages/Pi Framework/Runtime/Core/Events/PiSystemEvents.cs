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
        public readonly UnityEvent BeginAwake = new();
        public readonly UnityEvent FinalAwake = new();
        public readonly UnityEvent BeginStart = new();
        public readonly UnityEvent FinalStart = new();

        //game loop event
        public readonly UnityEvent BeginUpdate = new();
        public readonly UnityEvent FinalUpdate = new();

        public readonly UnityEvent BeginFixedUpdate = new();
        public readonly UnityEvent FinalFixedUpdate = new();

        public readonly UnityEvent BeginLateUpdate = new();
        public readonly UnityEvent FinalLateUpdate = new();

        //public readonly UnityEvent BeginOnGUI = new();
        //public readonly UnityEvent FinalOnGUI = new();


        /// <summary>
        /// occurs when user click exit or back button on menu screen, args is a PendingInvoker
        /// </summary>
        public readonly UnityEvent<PendingInvoker> OnTriggerShutdown = new UnityEvent<PendingInvoker>();
        public readonly UnityEvent BeginApplicationQuit = new();
        public readonly UnityEvent FinalApplicationQuit = new();


        //Common
        public readonly UnityEvent UserPaused = new();
        public readonly UnityEvent UserUnpaused  = new();

        internal PiSystemEvents() { }

        public readonly UnityEvent InitializeAfterSceneLoad = new();

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