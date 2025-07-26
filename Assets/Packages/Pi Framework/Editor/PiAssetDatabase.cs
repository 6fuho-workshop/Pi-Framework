using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiEditor
{
    internal class PiAssetDatabase : ScriptableObject
    {
        [Serializable]
        public struct AssetCollection
        {
            public string assetType;
            public List<string> assetPaths;
        }

        public List<AssetCollection> assets;

        internal void Rebuild()
        {
            Debug.Log("Rebuild");
            var callbacks = TypeCache.GetMethodsWithAttribute<OnAssetModificationOfTypeAttribute>();
            List<Type> types = new List<Type>();
            foreach (var callback in callbacks)
            {
                var attr = callback.GetCustomAttribute<OnAssetModificationOfTypeAttribute>();
                if (!types.Contains(attr.assetType))
                    types.Add(attr.assetType);
            }

            assets = new List<AssetCollection>();

            foreach (var type in types)
            {
                AssetCollection ac = new AssetCollection();
                ac.assetType = type.FullName;
                ac.assetPaths = new List<string>();
                var assetFiltered = AssetDatabase.FindAssets("t:" + type.Name);
                foreach (var item in assetFiltered)
                {
                    ac.assetPaths.Add(AssetDatabase.GUIDToAssetPath(item));
                }
                assets.Add(ac);
            }
        }
    }
}
