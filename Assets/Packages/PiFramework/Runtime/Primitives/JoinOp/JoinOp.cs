using System;
using System.Collections.Generic;
using PF.Contracts;

namespace PF.Primitives
{
    /// <summary>
    /// JoinOp = Joinable Operation, <br/>
    /// Represents a controllable execution step that can be delayed and coordinated by multiple external systems.
    /// 
    /// <para>
    /// <b>Purpose:</b> <br/>
    /// JoinOp allows different modules or components to "hold" a transitional step (such as scene changes, level completion, showing popups, etc.)
    /// and only proceeds when all participants have released their hold. This ensures that all required asynchronous or blocking operations
    /// are completed before the main flow continues.
    /// </para>
    /// 
    /// <para>
    /// <b>Use cases:</b> <br/>
    /// - Coordinating multiple systems during transitions (e.g., returning to home screen, finishing a level, showing interstitials, etc.) <br/>
    /// - Waiting for all modules to finish their own tasks before allowing the next step <br/>
    /// - Decoupling logic between systems that need to synchronize without direct dependencies
    /// </para>
    /// 
    /// <para>
    /// <b>Usage:</b> <br/>
    /// 1. Each participant calls <c>Register(participant)</c> to indicate it needs time to complete its work.<br/>
    /// 2. When done, it calls <c>Unregister()</c> on the returned token.<br/>
    /// 3. When all participants have unregistered, all completion callbacks are invoked.
    /// </para>
    /// 
    /// <para>
    /// <b>Thread safety:</b> <br/>
    /// All operations are thread-safe using a lock to protect internal state.
    /// </para>
    /// </summary>
    public class JoinOp
    {
        protected readonly HashSet<object> participants = new();
        protected readonly List<Action> onCompleteCallbacks = new();
        protected readonly object syncRoot = new();

        protected bool started;
        protected bool completed;

        /// <summary>
        /// Registers a participant to delay the step.
        /// Returns an <see cref="IUnregister"/> token for the participant to unregister itself when done.
        /// </summary>
        public virtual IUnregister Register(object participant)
        {
            if (participant == null)
                return new CustomUnregister(null, false);

            lock (syncRoot)
            {
                if (!participants.Contains(participant))
                {
                    participants.Add(participant);
                    OnRegister(participant);
                    TryCompleteLocked();
                }
            }

            return new CustomUnregister(() =>
            {
                lock (syncRoot)
                {
                    if (participants.Remove(participant))
                    {
                        OnUnregister(participant);
                        TryCompleteLocked();
                    }
                }
            });
        }

        /// <summary>
        /// Hook cho subclass khi participant được register.
        /// </summary>
        protected virtual void OnRegister(object participant) { }

        /// <summary>
        /// Hook cho subclass khi participant unregister.
        /// </summary>
        protected virtual void OnUnregister(object participant) { }

        /// <summary>
        /// Starts the step. Completion will occur automatically once all participants are released.
        /// </summary>
        public void Run(Action onComplete)
        {
            lock (syncRoot)
            {
                if (onComplete != null)
                    onCompleteCallbacks.Add(onComplete);

                started = true;
                TryCompleteLocked();
            }
        }

        /// <summary>
        /// Adds a callback to be invoked when the step is complete.
        /// If already completed, the callback will be called immediately.
        /// </summary>
        public void Callback(Action callback)
        {
            lock (syncRoot)
            {
                if (completed)
                    callback?.Invoke();
                else
                    onCompleteCallbacks.Add(callback);
            }
        }

        /// <summary>
        /// Tries to complete the step if all participants have been released.
        /// Invokes all registered callbacks and cleans up.
        /// </summary>
        protected virtual void TryCompleteLocked()
        {
            if (completed || !started || participants.Count > 0)
                return;

            completed = true;

            foreach (var cb in onCompleteCallbacks)
            {
                try { cb?.Invoke(); }
                catch (Exception) { }
            }

            onCompleteCallbacks.Clear();
        }

        /// <summary>
        /// Indicates whether the step has completed.
        /// </summary>
        public bool IsCompleted
        {
            get { lock (syncRoot) { return completed; } }
        }
    }
}