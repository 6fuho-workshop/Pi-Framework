using UnityEngine;

namespace PiFramework.Internal
{
    /// <summary>
    /// PiRoot is the entry point MonoBehaviour for the Pi Framework.
    /// - Ensures only one instance exists (singleton pattern).
    /// - Survives scene changes (DontDestroyOnLoad).
    /// - Triggers system startup and dispatches framework lifecycle events.
    /// - Handles application quit and cleanup.
    /// - Attaches the service container to the root for hierarchy clarity.
    /// </summary>
    [RequireComponent(typeof(LatestExecOrder))]

    // [ExecutionOrder(-31000)] sets the execution order for PiRoot.
    // We choose -31000 to reserve the range -31001 to -32000 for tasks/scripts
    // that need to execute before the framework initializes.
    // This allows advanced users or system-level modules to hook into the startup
    // sequence even earlier than the framework's entry point if needed.
    [ExecutionOrder(-31000)]
    public class PiRoot : MonoBehaviour
    {
        // Indicates if this instance is the active singleton
        bool isSingleton;

        /// <summary>
        /// Awake is called when the script instance is being loaded.
        /// Ensures only one PiRoot exists, initializes the framework, and dispatches the beginAwake event.
        /// </summary>
        void Awake()
        {
            // Prevent duplicate PiRoot instances (singleton enforcement)
            if (PiBase.root != null)
            {
                Debug.LogWarning("Duplicate PiRoot detected, destroying this instance.");
                gameObject.SetActive(false);
                GameObject.DestroyImmediate(gameObject);
                return;
            }

            isSingleton = true;

            // Make this GameObject persistent across scene loads
            GameObject.DontDestroyOnLoad(gameObject);

            // Initialize the Pi Framework core systems
            PiBootstrapper.Bootstrap(this);

            // Attach the service container to this root for hierarchy organization
            AttachServiceContainer();

            // Dispatch the framework's beginAwake event for all listeners
            PiBase.systemEvents.beginAwake.Invoke();
        }

        #region Dispatch System Events

        /// <summary>
        /// Dispatches the beginStart event to notify all systems that Start has been called.
        /// </summary>
        void Start() => PiBase.systemEvents.beginStart.Invoke();

        /// <summary>
        /// Dispatches the beginUpdate event every frame.
        /// </summary>
        void Update() => PiBase.systemEvents.beginUpdate.Invoke();

        /// <summary>
        /// Dispatches the beginFixedUpdate event at fixed intervals.
        /// </summary>
        void FixedUpdate() => PiBase.systemEvents.beginFixedUpdate.Invoke();

        /// <summary>
        /// Dispatches the beginLateUpdate event after all Update calls.
        /// </summary>
        void LateUpdate() => PiBase.systemEvents.beginLateUpdate.Invoke();

        // Tracks if the application is quitting
        internal bool isQuitting { get; private set; }

        /// <summary>
        /// Handles application quit logic and dispatches the AppQuitPhase1 event.
        /// </summary>
        private void OnApplicationQuit()
        {
            isQuitting = true;
            PiBase.status = SystemStatus.Shutdown;
            PiBase.systemEvents.AppQuitPhase1.Invoke();
        }

        /// <summary>
        /// Handles cleanup when the PiRoot is destroyed and dispatches the AppQuitPhase3 event.
        /// </summary>
        private void OnDestroy()
        {
            if(isSingleton)
                PiBase.systemEvents.AppQuitPhase3.Invoke();
        }

        #endregion

        /// <summary>
        /// Attaches the framework's service container to this root GameObject for better hierarchy organization.
        /// </summary>
        void AttachServiceContainer()
        {
            PiServiceRegistry.instance.objectContainer.transform.parent = transform;
        }
    }
}