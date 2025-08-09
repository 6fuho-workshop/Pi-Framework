using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PF.Pool
{
    [AddComponentMenu("Pi/Poolable")]
    public class Poolable : MonoBehaviour
    {
        //Dùng Unity Pool thì không thể auto preload each frame đc
        //public enum PreloadStrategy { Immediate, AutoEachFrame}

        public int maxSize = 10000;
        public int defaultCapacity = 10;
        public int preloadCount = 0;
        public bool setParentOnCreate;
        public bool deactiveOnRelease;
        public bool hideOnRelease;
        public bool dontDestroyOnLoad;

        //internal bool released;//dùng để check lỗi double release  
        internal int poolID;
    }
}