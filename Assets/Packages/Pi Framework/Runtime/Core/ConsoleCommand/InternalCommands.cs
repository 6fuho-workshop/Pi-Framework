using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;


namespace PiFramework
{
    public class InternalCommands
    {
        internal static void TriggerExit()
        {
            var pendingShutdown = new PendingAction(PiBase.gameObject, () => Application.Quit());
            PiBase.systemEvents.triggeredShutdown.Invoke(pendingShutdown);
        }

        internal static void TriggerRestart()
        {
            var pendingRestart = new PendingAction(PiBase.gameObject, () => SceneManager.LoadScene(0));
            PiBase.systemEvents.triggeredRestart.Invoke(pendingRestart);
        }
    }
}