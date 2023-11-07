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
        public readonly PiEvent beginAwake = new();
        public readonly PiEvent finalAwake = new();
        public readonly PiEvent beginStart = new();
        public readonly PiEvent finalStart = new();

        //game loop event
        public readonly PiEvent beginUpdate = new();
        public readonly PiEvent finalUpdate = new();

        public readonly PiEvent beginFixedUpdate = new();
        public readonly PiEvent finalFixedUpdate = new();

        public readonly PiEvent beginLateUpdate = new();
        public readonly PiEvent finalLateUpdate = new();

        /// <summary>
        /// occurs when user click exit or back button on menu screen, args is a PendingInvoker
        /// </summary>
        public readonly Interruption onTriggerShutdown = new();
        public readonly PiEvent beginApplicationQuit = new();
        public readonly PiEvent finalApplicationQuit = new();


        //Common
        public readonly PiEvent userPaused = new();
        public readonly PiEvent userUnpaused  = new();

        public readonly PiEvent initializeAfterSceneLoad = new();

        internal void Reset()
        {
            beginAwake.RemoveAllListeners();
            finalAwake.RemoveAllListeners();
            beginStart.RemoveAllListeners();
            finalStart.RemoveAllListeners();
            beginUpdate.RemoveAllListeners();
            finalUpdate.RemoveAllListeners();
            beginFixedUpdate.RemoveAllListeners();
            finalFixedUpdate.RemoveAllListeners();
            beginLateUpdate.RemoveAllListeners();
            finalLateUpdate.RemoveAllListeners();
            //BeginOnGUI.RemoveAllListeners();
            
            onTriggerShutdown.RemoveAllListeners();
            beginApplicationQuit.RemoveAllListeners();
            finalApplicationQuit.RemoveAllListeners();
            userPaused.RemoveAllListeners();
            userUnpaused.RemoveAllListeners();

            initializeAfterSceneLoad.RemoveAllListeners();
        }
    }

    public class Interruption : PiEvent<PendingInvoker> { }
}