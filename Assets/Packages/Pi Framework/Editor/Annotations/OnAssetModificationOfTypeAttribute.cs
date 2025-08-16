using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PF.PiEditor
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class OnAssetModificationOfTypeAttribute : Attribute
    {
        // Start is called before the first frame update
        public Type assetType { get; }

        public OnAssetModificationOfTypeAttribute(Type assetType)
        {
            if (assetType == null) throw new ArgumentNullException(nameof(assetType));
            this.assetType = assetType;
        }
    }
}