using UnityEditor;
public static class PFReserialize
{
    [MenuItem("PF/Dev/Force Reserialize All")]
    static void Run() => AssetDatabase.ForceReserializeAssets();
}