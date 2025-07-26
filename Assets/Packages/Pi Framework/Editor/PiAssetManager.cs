using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine;
using PiEditor.Utils;
using System.Reflection;


namespace PiEditor
{
    internal class PiAssetManager : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            PE.Initialize();
            if (importedAssets.Length == 0 && deletedAssets.Length == 0 && movedAssets.Length == 0)
                return;

            var db = GetPiAssetDatabase();
            var callbacks = TypeCache.GetMethodsWithAttribute<OnAssetModificationOfTypeAttribute>();

            foreach (var callback in callbacks)
            {
                var attribute = callback.GetCustomAttribute<OnAssetModificationOfTypeAttribute>();
                var typeName = attribute.assetType.FullName;

                var paramCount = callback.GetParameters().Length;
                if (paramCount == 0)
                {
                    callback.Invoke(null, null);
                }
                else
                {
                    callback.Invoke(null, new object[]
                    {
                    importedAssets,
                    deletedAssets,
                    movedAssets,
                    movedFromAssetPaths,
                    didDomainReload
                    });
                }
            }
        }

        static void RebuildAssetDatabsae()
        {
            GetPiAssetDatabase(true);
        }

        static PiAssetDatabase GetPiAssetDatabase(bool rebuild = false)
        {
            var path = PiPath.editorPath + "/AssetDatabase.asset";
            PiAssetDatabase db = AssetDatabase.LoadAssetAtPath<PiAssetDatabase>(path);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<PiAssetDatabase>();
                db.Rebuild();
                AssetDatabase.CreateAsset(db, path);
            }
            else if (rebuild)
            {
                db.Rebuild();
            }

            return db;
        }


    }
}