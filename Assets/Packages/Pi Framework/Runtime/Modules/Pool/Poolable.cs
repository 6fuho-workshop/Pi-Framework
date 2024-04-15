using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiFramework.Pool
{
    public class Poolable : MonoBehaviour
    {
        //Dùng Unity Pool thì không thể auto preload each frame đc
        //public enum PreloadStrategy { Immediate, AutoEachFrame}

        public int maxSize = 10000;
        public int defaultCapacity = 10;
        public int preloadCount = 0;
        public bool setParentOnCreate = true;
        public bool deactiveOnRelease;
        public bool hideOnRelease;
        //[Tooltip("Auto Preload Time since Preload() call")]
        //public float targetPreloadTime = 1f;

        public bool dontDestroyOnLoad;

    }
}