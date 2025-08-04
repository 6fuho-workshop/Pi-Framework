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
            var pendingShutdown = new PendingAction(PiBase.Root.gameObject, () => Application.Quit());
            PiBase.SystemEvents.OnShutdown.Invoke(pendingShutdown);
        }

        internal static void TriggerRestart()
        {
            var pendingRestart = new PendingAction(PiBase.Root.gameObject, () => SceneManager.LoadScene(0));
            PiBase.SystemEvents.OnRestart.Invoke(pendingRestart);
        }
    }
}