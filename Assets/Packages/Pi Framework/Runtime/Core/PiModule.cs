using UnityEngine;
using System.Collections;
namespace PiFramework
{
    public abstract class PiModule : MonoBehaviour
    {
        /// <summary>
        /// Được gọi sau PiRoot.Awake và trước các method Awake khác
        /// </summary>
        protected virtual void Initialize()
        {

        }

        /// <summary>
        /// Internal call
        /// </summary>
        internal void _moduleInit()
        {
            Initialize();
        }
    }
}