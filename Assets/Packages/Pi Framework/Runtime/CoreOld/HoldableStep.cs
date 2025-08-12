using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PF
{
    /// <summary>
    /// Represents a controllable execution step that can be delayed by external systems 
    /// and only proceeds when all participants have released their hold.
    /// 
    /// This is useful in scenarios where multiple modules need to hook into a transitional 
    /// phase (e.g., returning to the home screen, finishing a level, showing interstitials, etc.)
    /// and coordinate their own tasks before allowing the main flow to continue.
    ///
    /// External systems can call <c>Hold(object token)</c> to declare that they need time to 
    /// perform something (e.g., show a popup, animate UI), and later call <c>Release(object token)</c> 
    /// to indicate they are done. The step will not complete until all holders have released.
    ///
    /// Auto-release is also supported: holders with null references will be automatically removed 
    /// during periodic updates (use <c>Update()</c> method).
    ///
    /// Example usage:
    /// <code>
    /// var step = new HoldableStep();
    ///
    /// // Module A wants to show a rate-us popup
    /// var tokenA = new object();
    /// step.Hold(tokenA);
    /// ShowRateUs(() => step.Release(tokenA));
    ///
    /// // Module B wants to wait for IAP offer display
    /// var tokenB = new object();
    /// step.Hold(tokenB);
    /// ShowIapOffer(() => step.Release(tokenB));
    ///
    /// // When all holders release, the final callback will be invoked
    /// step.OnComplete(() => {
    ///     EnableMainUI();
    /// });
    /// </code>
    /// </summary>

    public class HoldableStep
    {
        private readonly HashSet<object> holders = new();
        private readonly List<Action> onCompleteCallbacks = new();

        private bool started;
        private bool completed;

        /// <summary>
        /// Registers a holder to delay the step.
        /// The step will not complete until all holders are released.
        /// </summary>
        /// <param name="holder">Any non-null object uniquely representing the holder.</param>
        public void Hold(object holder)
        {
            if (holder == null)
            {
                Debug.LogError("[HoldableStep] Cannot hold with null holder.");
                return;
            }

            holders.Add(holder);
        }

        /// <summary>
        /// Releases a previously registered holder.
        /// If the holder was not found, a warning will be logged.
        /// </summary>
        /// <param name="holder">The same object previously used in Hold().</param>
        public void Release(object holder)
        {
            if (!holders.Remove(holder))
            {
                Debug.LogWarning($"[HoldableStep] Release failed — holder not found or already released: {holder}");
            }

            TryComplete();
        }

        /// <summary>
        /// Starts the step. Completion will occur automatically once all holders are released.
        /// </summary>
        /// <param name="onComplete">Optional callback to be invoked when the step completes.</param>
        public void Run(Action onComplete = null)
        {
            if (onComplete != null)
                onCompleteCallbacks.Add(onComplete);

            started = true;

            // Automatically check for null/destroyed holders each frame
            P.SystemEvents.OnFirstUpdate.Register(Update);

            TryComplete(); // Handle case where no holders exist
        }

        /// <summary>
        /// Adds a callback to be invoked when the step is complete.
        /// If already completed, the callback will be called immediately.
        /// </summary>
        /// <param name="callback">Callback to invoke on completion.</param>
        public void Callback(Action callback)
        {
            if (completed)
                callback?.Invoke();
            else
                onCompleteCallbacks.Add(callback);
        }

        /// <summary>
        /// Called every frame to automatically release any holders that have become null or destroyed.
        /// </summary>
        private void Update()
        {
            if (completed)
            {
                P.SystemEvents.OnFirstUpdate.Unregister(Update);
                return;
            }

            var nullHolders = holders.Where(IsNullOrDestroyed).ToList();

            foreach (var holder in nullHolders)
            {
                Debug.LogWarning($"[HoldableStep] Auto-releasing null or destroyed holder: {holder}");
                holders.Remove(holder);
            }

            TryComplete();
        }

        /// <summary>
        /// Determines if an object is null or a destroyed UnityEngine.Object.
        /// </summary>
        private bool IsNullOrDestroyed(object obj)
        {
            if (obj == null) return true;

            if (obj is UnityEngine.Object unityObj)
                return unityObj == null;

            return false;
        }

        /// <summary>
        /// Tries to complete the step if all holders have been released.
        /// Invokes all registered callbacks and cleans up.
        /// </summary>
        private void TryComplete()
        {
            if (completed || !started || holders.Count > 0)
                return;

            completed = true;
            P.SystemEvents.OnFirstUpdate.Unregister(Update);

            foreach (var cb in onCompleteCallbacks)
            {
                try { cb?.Invoke(); }
                catch (Exception ex)
                {
                    Debug.LogException(ex);
                }
            }

            onCompleteCallbacks.Clear();
        }
    }
}