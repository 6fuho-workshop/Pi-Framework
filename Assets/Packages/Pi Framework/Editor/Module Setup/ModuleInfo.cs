using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PiEditor
{
    [Serializable]
    public class ModuleInfo
    {
        public string moduleName;
        public string moduleCode;
        public string[] dependencies;
        public PinServiceInfo[] pinServices;


        [Serializable]
        public class PinServiceInfo
        {
            /// <summary>
            /// pin name để tạo property Pi.propertyName
            /// </summary>
            public string name;

            /// <summary>
            /// Type fuleName (include namespace)
            /// </summary>
            public string fullType;
        }

    }
}