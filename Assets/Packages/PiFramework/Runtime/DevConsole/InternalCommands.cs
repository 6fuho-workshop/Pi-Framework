using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
using PF.Primitives;


namespace PF
{
    public class InternalCommands
    {
        internal static void TriggerExit()
        {
            var joinOp = new JoinOp();

            Pi.systemEvents.OnShutdown.Publish(joinOp);
            joinOp.Run(() =>
            {
                Application.Quit();
            });
        }

        internal static void TriggerRestart()
        {
            throw new NotImplementedException("Restart command is not implemented yet. Please implement the restart logic in your application.");
            //var pendingRestart = new PendingAction(Pi.Root.gameObject, () => SceneManager.LoadScene(0));
            //Pi.SystemEvents.OnRestart.Publish(pendingRestart);
        }
    }
}