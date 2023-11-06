using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PiFramework;

namespace PiFramework
{
    /// <summary>
    /// Chứa các PiEvent liên quan đến vòng lặp và các event hệ thống
    /// </summary>
    public class PiSystemEvents: MonoBehaviour
    {
        //game init event
        public readonly PiEvent BeginAwake = new();
        public readonly PiEvent FinalAwake = new();
        public readonly PiEvent BeginStart = new();
        public readonly PiEvent FinalStart = new();

        //game loop event
        public readonly PiEvent BeginUpdate = new();
        public readonly PiEvent FinalUpdate = new();

        public readonly PiEvent BeginFixedUpdate = new();
        public readonly PiEvent FinalFixedUpdate = new();

        public readonly PiEvent BeginLateUpdate = new();
        public readonly PiEvent FinalLateUpdate = new();

        /// <summary>
        /// occurs when user click exit or back button on menu screen, args is a PendingInvoker
        /// </summary>
        public readonly PiEvent<PendingInvoker> OnTriggerShutdown = new();
        public readonly PiEvent BeginApplicationQuit = new();
        public readonly PiEvent FinalApplicationQuit = new();


        //Common
        public readonly PiEvent UserPaused = new();
        public readonly PiEvent UserUnpaused  = new();

        public readonly PiEvent InitializeAfterSceneLoad = new();

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

            InitializeAfterSceneLoad.RemoveAllListeners();
        }
    }
}