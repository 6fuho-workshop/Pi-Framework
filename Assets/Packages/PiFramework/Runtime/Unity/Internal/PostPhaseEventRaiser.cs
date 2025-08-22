using PF.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PF.Unity.Internal
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
            Pi.systemEvents.OnLastAwake.Publish();
        }

        /// <summary>
        /// Called after all other Start methods. Raises the StartFinished event.
        /// </summary>
        private void Start()
        {
            Pi.systemEvents.OnLastStart.Publish();
        }

        /// <summary>
        /// Called after all other Update methods. Raises the UpdateFinished event.
        /// </summary>
        private void Update()
        {
            Pi.systemEvents.OnLastUpdate.Publish();
        }

        /// <summary>
        /// Called after all other FixedUpdate methods. Raises the FixedUpdateFinished event.
        /// </summary>
        private void FixedUpdate()
        {
            Pi.systemEvents.OnLastFixedUpdate.Publish();
        }

        /// <summary>
        /// Called after all other LateUpdate methods. Raises the LateUpdateFinished event.
        /// </summary>
        private void LateUpdate()
        {
            Pi.systemEvents.OnLastLateUpdate.Publish();
        }

        /// <summary>
        /// Called on application quit. Raises the AppQuitPhase1 event.
        /// </summary>
        private void OnApplicationQuit()
        {
            Pi.systemEvents.OnAppQuitPhase1.Publish();
        }
    }
}