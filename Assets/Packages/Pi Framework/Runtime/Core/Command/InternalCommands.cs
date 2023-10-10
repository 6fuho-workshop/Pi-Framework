using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace PiFramework
{
    public class InternalCommands
    {
        internal static void TriggerExit()
        {
            var pendingShutdown = new PendingInvoker().AddCallback(delegate () {
                Application.Quit();
            });
            PiBootstrap.instance.systemEvents.OnTriggerShutdown.Invoke(pendingShutdown);
        }

        internal static void TriggerRestart()
        {
            var pendingShutdown = new PendingInvoker().AddCallback(delegate () {
                SceneManager.LoadScene(0);
            });
            
            PiBootstrap.instance.systemEvents.OnTriggerShutdown.Invoke(pendingShutdown);
        }
    }
}