using UnityEngine;
using UnityEngine.Tilemaps;

namespace PF.Internal
{
    /// <summary>
    /// LatestExecOrder is a MonoBehaviour attached to the PiFramework entry GameObject.
    /// It is set to the highest execution order (32000) to ensure its Unity event methods
    /// (Awake, Start, Update, etc.) are called last in each frame phase.
    /// This allows the framework to raise "finished" events at the end of each Unity phase,
    /// enabling other systems to hook into the very end of the lifecycle for safe, post-processing logic.
    /// </summary>
    [ExecutionOrder(32000)]
    public class PostPhaseEventRaiser : MonoBehaviour
    {
        /// <summary>
        /// Called after all other Awake methods. Raises the AwakeFinished event.
        /// </summary>
        private void Awake()
        {
            PiBase.SystemEvents.OnLastAwake?.Invoke();
        }

        /// <summary>
        /// Called after all other Start methods. Raises the StartFinished event.
        /// </summary>
        private void Start()
        {
            PiBase.SystemEvents.OnLastStart?.Invoke();
        }

        /// <summary>
        /// Called after all other Update methods. Raises the UpdateFinished event.
        /// </summary>
        private void Update()
        {
            PiBase.SystemEvents.OnLastUpdate?.Invoke();
        }

        /// <summary>
        /// Called after all other FixedUpdate methods. Raises the FixedUpdateFinished event.
        /// </summary>
        private void FixedUpdate()
        {
            PiBase.SystemEvents.OnLastFixedUpdate?.Invoke();
        }

        /// <summary>
        /// Called after all other LateUpdate methods. Raises the LateUpdateFinished event.
        /// </summary>
        private void LateUpdate()
        {
            PiBase.SystemEvents.OnLastLateUpdate?.Invoke();
        }

        /// <summary>
        /// Called on application quit. Raises the AppQuitPhase1 event.
        /// </summary>
        private void OnApplicationQuit()
        {
            PiBase.SystemEvents.OnAppQuitPhase1?.Invoke();
        }
    }
}