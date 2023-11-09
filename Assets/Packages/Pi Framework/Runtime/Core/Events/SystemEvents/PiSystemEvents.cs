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
        /// Occurs when user clicked exit or back button on menu screen.<br/>
        /// Can be interrupted for further processing.
        /// </summary>
        public readonly InterruptableEvent triggeredShutdown = new();

        public readonly InterruptableEvent triggeredRestart = new();

        /// <summary>
        /// OnApplicationQuit
        /// </summary>
        public readonly PiEvent AppQuitPhase1 = new();

        /// <summary>
        /// Application.quitting
        /// </summary>
        public readonly PiEvent AppQuitPhase2 = new();

        /// <summary>
        /// OnDestroy
        /// </summary>
        public readonly PiEvent AppQuitPhase3 = new();

        //Common
        public readonly PiEvent userPaused = new();
        public readonly PiEvent userUnpaused  = new();

        public readonly PiEvent initializeAfterSceneLoad = new();

        internal void Reset()
        {
            beginAwake.UnRegisterAll();
            finalAwake.UnRegisterAll();
            beginStart.UnRegisterAll();
            finalStart.UnRegisterAll();
            beginUpdate.UnRegisterAll();
            finalUpdate.UnRegisterAll();
            beginFixedUpdate.UnRegisterAll();
            finalFixedUpdate.UnRegisterAll();
            beginLateUpdate.UnRegisterAll();
            finalLateUpdate.UnRegisterAll();
            //BeginOnGUI.RemoveAllListeners();
            
            triggeredShutdown.UnRegisterAll();
            AppQuitPhase1.UnRegisterAll();
            userPaused.UnRegisterAll();
            userUnpaused.UnRegisterAll();

            initializeAfterSceneLoad.UnRegisterAll();
        }
    }
}