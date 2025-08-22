using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using UnityEngine;
using PF.PiEditor.Utils;
using System.Reflection;


namespace PF.PiEditor
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
            var path = PathHelper.PiDataEditorDir + "/AssetDatabase.asset";
            PiAssetDatabase db = AssetDatabase.LoadAssetAtPath<PiAssetDatabase>(path);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<PiAssetDatabase>();
                db.Rebuild();
                Debug.Log("[PF] Tạo mới PiAssetDatabase tại: " + path);
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