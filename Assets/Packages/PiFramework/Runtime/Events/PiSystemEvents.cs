using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PF;
using PF.Events;
using PF.Contracts;
using PF.Primitives;

namespace PF.Events
{
    /// <summary>
    /// Chứa các PiEvent liên quan đến vòng lặp và các event hệ thống
    /// </summary>
    public class PiSystemEvents : ISystemEvents
    {
        // Unity lifecycle events
        internal readonly PiEvent OnFirstAwake = new();
        internal readonly PiEvent OnLastAwake = new();
        internal readonly PiEvent OnFirstStart = new();
        internal readonly PiEvent OnLastStart = new();
        internal readonly PiEvent OnFirstUpdate = new();
        internal readonly PiEvent OnLastUpdate = new();
        internal readonly PiEvent OnFirstFixedUpdate = new();
        internal readonly PiEvent OnLastFixedUpdate = new();
        internal readonly PiEvent OnFirstLateUpdate = new();
        internal readonly PiEvent OnLastLateUpdate = new();

        // Other events
        internal readonly PiEvent<ProgressJoinOp> OnAppLoading = new();
        internal readonly JoinOpEvent OnShutdown = new();
        internal readonly PiEvent OnRestart = new();
        internal readonly PiEvent OnInitializeAfterSceneLoad = new();
        internal readonly PiEvent OnAppQuitPhase1 = new();
        internal readonly PiEvent OnAppQuitPhase2 = new();
        internal readonly PiEvent OnAppQuitPhase3 = new();

        // Properties implementing ISystemEvents
        public IEvent FirstAwakeCalled => OnFirstAwake;
        public IEvent LastAwakeCalled => OnLastAwake;
        public IEvent FirstStartCalled => OnFirstStart;
        public IEvent LastStartCalled => OnLastStart;
        public IEvent FirstUpdateCalled => OnFirstUpdate;
        public IEvent LastUpdateCalled => OnLastUpdate;
        public IEvent FirstFixedUpdateCalled => OnFirstFixedUpdate;
        public IEvent LastFixedUpdateCalled => OnLastFixedUpdate;
        public IEvent FirstLateUpdateCalled => OnFirstLateUpdate;
        public IEvent LastLateUpdateCalled => OnLastLateUpdate;

        public IEvent<ProgressJoinOp> AppLoading => OnAppLoading;
        public IEvent<JoinOp> ShuttingDown => OnShutdown;
        public IEvent Restarting => OnRestart;
        public IEvent InitializedAfterSceneLoad => OnInitializeAfterSceneLoad;
        public IEvent AppQuittingPhase1 => OnAppQuitPhase1;
        public IEvent AppQuittingPhase2 => OnAppQuitPhase2;
        public IEvent AppQuittingPhase3 => OnAppQuitPhase3;

        internal void Dispose()
        {
            OnFirstAwake.Dispose();
            OnLastAwake.Dispose();
            OnFirstStart.Dispose();
            OnLastStart.Dispose();
            OnFirstUpdate.Dispose();
            OnLastUpdate.Dispose();
            OnFirstFixedUpdate.Dispose();
            OnLastFixedUpdate.Dispose();
            OnFirstLateUpdate.Dispose();
            OnLastLateUpdate.Dispose();

            OnAppLoading.Dispose();
            OnShutdown.Dispose();
            OnRestart.Dispose();
            OnInitializeAfterSceneLoad.Dispose();
            OnAppQuitPhase1.Dispose();
            OnAppQuitPhase2.Dispose();
            OnAppQuitPhase3.Dispose();
        }
    }
}