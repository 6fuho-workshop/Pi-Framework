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
            var pendingShutdown = new PendingAction(PiBase.root.gameObject, () => Application.Quit());
            PiBase.systemEvents.OnShutdown.Invoke(pendingShutdown);
        }

        internal static void TriggerRestart()
        {
            var pendingRestart = new PendingAction(PiBase.root.gameObject, () => SceneManager.LoadScene(0));
            PiBase.systemEvents.OnRestart.Invoke(pendingRestart);
        }
    }
}