using UnityEngine;
using PF;
using Logger = PF.Contracts.ILogger;

namespace PF
{
    [CreateAssetMenu(fileName = "SystemConfiguration", menuName = "▶ Pi ◀/SystemConfiguration", order = 1)]
    public class SystemConfiguration : ScriptableObject
    {
        public string LogFilePath = "Logs/PiLogger.log";
        public bool UseUnityLogSink = true;
        // Called when an instance of ScriptableObject is created.
        private void Awake()
        {

        }

        // This function is called when the object is loaded.
        void OnEnable()
        {

        }

        // Editor-only function that Unity calls when the script is loaded or a value changes in the Inspector.
        private void OnValidate()
        {

        }
    }
}