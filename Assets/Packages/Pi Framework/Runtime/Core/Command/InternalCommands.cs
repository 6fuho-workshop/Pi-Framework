using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace PiFramework
{
    public class InternalCommands
    {
        internal static void TriggerExit()
        {
            var pendingShutdown = new PendingInvoker(PiBase.gameObject).AddCallback(() => Application.Quit());
            PiBase.systemEvents.onTriggerShutdown.Invoke(pendingShutdown);
        }

        internal static void TriggerRestart()
        {
            var pendingShutdown = new PendingInvoker(PiBase.gameObject).AddCallback(() => SceneManager.LoadScene(0));
            PiBase.systemEvents.onTriggerShutdown.Invoke(pendingShutdown);
        }
}
}