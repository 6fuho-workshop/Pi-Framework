using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PF;

namespace PF
{
    /// <summary>
    /// Chứa các PiEvent liên quan đến vòng lặp và các event hệ thống
    /// </summary>
    public class PiSystemEvents
    {
        #region game init event

        /// <summary>
        /// Represents an event that is triggered when the Awake phase of the game initialization starts.
        /// </summary>
        /// <remarks>This event can be used to perform actions that need to occur at the beginning of the
        /// Awake phase. Subscribers should ensure that their event handlers execute quickly to avoid delaying the
        /// initialization process.</remarks>
        public readonly PiEvent OnFirstAwake = new();

        /// <summary>
        /// Represents an event that is triggered when the awake process is completed.
        /// </summary>
        /// <remarks>This event can be used to notify subscribers that the initialization or setup phase
        /// has finished.</remarks>
        public readonly PiEvent OnLastAwake = new();

        /// <summary>
        /// Represents an event that is triggered when a start operation begins.
        /// </summary>
        /// <remarks>This event can be used to subscribe to notifications when a start operation is
        /// initiated. It is a readonly field, meaning it is initialized once and cannot be reassigned.</remarks>
        public readonly PiEvent OnFirstStart = new();

        /// <summary>
        /// Represents an event that is triggered when the start process is finished.
        /// </summary>
        /// <remarks>This event can be used to notify subscribers that the start process has
        /// completed.</remarks>
        public readonly PiEvent OnLastStart = new();

        #endregion game init event

        #region game loop events

        /// <summary>
        /// Represents an event that is triggered when the update process starts in the game loop.
        /// </summary>
        /// <remarks>This event can be used to execute custom logic at the beginning of each update cycle.
        /// Subscribers should ensure that their event handlers execute quickly to avoid delaying the update
        /// process.</remarks>
        public readonly PiEvent OnFirstUpdate = new();

        /// <summary>
        /// Represents an event that is triggered when an update operation is completed.
        /// </summary>
        /// <remarks>This event can be used to notify subscribers that an update process has finished. 
        /// Subscribers can attach event handlers to perform actions upon the completion of the update.</remarks>
        public readonly PiEvent OnLastUpdate = new();

        /// <summary>
        /// Represents an event that is triggered at the start of a fixed update cycle.
        /// </summary>
        /// <remarks>This event is typically used in game development or simulation contexts where
        /// operations need to be synchronized with the fixed update loop. Subscribers to this event can perform actions
        /// that should occur at the beginning of each fixed update cycle.</remarks>
        public readonly PiEvent OnFirstFixedUpdate = new();

        /// <summary>
        /// Represents an event that is triggered when the fixed update cycle is completed.
        /// </summary>
        /// <remarks>This event can be used to perform actions that need to occur after the fixed update
        /// loop. It is typically used in game development or simulation contexts where fixed update cycles are employed
        /// to maintain consistent physics calculations.</remarks>
        public readonly PiEvent OnLastFixedUpdate = new();
            
        /// <summary>
        /// Represents an event that is triggered when the late update phase starts.
        /// </summary>
        /// <remarks>This event can be used to execute logic that should occur during the late update
        /// phase of a game loop or application cycle. Subscribers to this event should ensure that their handlers
        /// execute quickly to avoid performance issues.</remarks>
        public readonly PiEvent OnFirstLateUpdate = new();

        /// <summary>
        /// Represents an event that is triggered when the late update process is completed.
        /// </summary>
        /// <remarks>This event can be used to execute actions that should occur after the late update
        /// phase in the application's lifecycle. Subscribers can attach event handlers to perform tasks that need to be
        /// executed at this point.</remarks>
        public readonly PiEvent OnLastLateUpdate = new();

        #endregion game loop events

        /// <summary>
        /// Occurs when user clicked exit or back button on menu screen.<br/>
        /// Can be interrupted for further processing.
        /// </summary>
        public readonly HookableEvent OnShutdown = new();

        public readonly HookableEvent OnRestart = new();

        /// <summary>
        /// OnApplicationQuit
        /// </summary>
        public readonly PiEvent OnAppQuitPhase1 = new();

        /// <summary>
        /// Application.quitting
        /// </summary>
        public readonly PiEvent OnAppQuitPhase2 = new();

        /// <summary>
        /// OnDestroy
        /// </summary>
        public readonly PiEvent OnAppQuitPhase3 = new();

        public readonly PiEvent OnInitializeAfterSceneLoad = new();

        internal void Reset()
        {
            OnFirstAwake.UnregisterAll();
            OnLastAwake.UnregisterAll();
            OnFirstStart.UnregisterAll();
            OnLastStart.UnregisterAll();
            OnFirstUpdate.UnregisterAll();
            OnLastUpdate.UnregisterAll();
            OnFirstFixedUpdate.UnregisterAll();
            OnLastFixedUpdate.UnregisterAll();
            OnFirstLateUpdate.UnregisterAll();
            OnLastLateUpdate.UnregisterAll();
            //BeginOnGUI.RemoveAllListeners();

            OnShutdown.UnregisterAll();
            OnAppQuitPhase1.UnregisterAll();
            OnInitializeAfterSceneLoad.UnregisterAll();
        }
    }
}