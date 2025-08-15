using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;


namespace PF
{
    public class InternalCommands
    {
        internal static void TriggerExit()
        {
            var pendingShutdown = new PendingAction(P.Root.gameObject, () => Application.Quit());
            P.SystemEvents.OnShutdown.Invoke(pendingShutdown);
        }

        internal static void TriggerRestart()
        {
            var pendingRestart = new PendingAction(P.Root.gameObject, () => SceneManager.LoadScene(0));
            P.SystemEvents.OnRestart.Invoke(pendingRestart);
        }
    }
}