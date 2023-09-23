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
        public UnityEvent BeginAwake = new UnityEvent();
        public UnityEvent FinalAwake = new UnityEvent();
        public UnityEvent BeginStart = new UnityEvent();
        public UnityEvent FinalStart = new UnityEvent();

        //game loop event
        public UnityEvent BeginUpdate = new UnityEvent();
        public UnityEvent FinalUpdate = new UnityEvent();
        
        public UnityEvent BeginLateUpdate = new UnityEvent();
        public UnityEvent FinalLateUpdate = new UnityEvent();

        public UnityEvent BeginOnGUI = new UnityEvent();
        public UnityEvent FinalOnGUI = new UnityEvent();

        /// <summary>
        /// occurs when the game started and begin loading resources,loading next scene and show loading screen, args is a PiProgress object
        /// </summary>
        public UnityEvent StartSceneBeginLoading = new UnityEvent();

        /// <summary>
        /// occurs when application begin loading the PlayScene async, , args is a PiProgress object
        /// </summary>
        public UnityEvent PlaySceneBeginLoading = new UnityEvent();

        /// <summary>
        /// occurs when user click exit or back button on menu screen, args is a PendingInvoker
        /// </summary>
        public UnityEvent<PendingInvoker> OnTriggerShutdown = new UnityEvent<PendingInvoker>();
        public UnityEvent BeginApplicationQuit = new UnityEvent();
        public UnityEvent FinalApplicationQuit = new UnityEvent();


        //Common
        public UnityEvent UserPaused = new UnityEvent();
        public UnityEvent UserUnpaused = new UnityEvent();

        internal PiSystemEvents() { }

        internal void Reset()
        {
            BeginAwake.RemoveAllListeners();
            FinalAwake.RemoveAllListeners();
            BeginStart.RemoveAllListeners();
            FinalStart.RemoveAllListeners();
            BeginUpdate.RemoveAllListeners();
            BeginLateUpdate.RemoveAllListeners();
            BeginOnGUI.RemoveAllListeners();
            
            StartSceneBeginLoading.RemoveAllListeners();
            PlaySceneBeginLoading.RemoveAllListeners();
            OnTriggerShutdown.RemoveAllListeners();
            BeginApplicationQuit.RemoveAllListeners();
            FinalApplicationQuit.RemoveAllListeners();
            UserPaused.RemoveAllListeners();
            UserUnpaused.RemoveAllListeners();
        }
    }
}