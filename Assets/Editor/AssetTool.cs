using UnityEditor;
using UnityEngine;

public class AssetTool
{
    /// <summary>
    /// 批量创建Prefabs
    /// </summary>
    [MenuItem("XDEDZL/Inspector/批量创建预制体")]
    public static void CreatPrefabs()
    {
        foreach (var item in Selection.gameObjects)
        {
#if UNITY_2018
            PrefabUtility.SaveAsPrefabAsset(item, "Assets/Prefabs/" + item.name + ".prefab");
#else
            PrefabUtility.CreatePrefab("Assets/Prefabs/" + item.name + ".prefab", item);
#endif
        }
    }

    /// <summary>
    /// 批量Apply
    /// </summary>
    [MenuItem("XDEDZL/Inspector/批量Applay预制体")]
    public static void ReplacePrefabs()
    {
        foreach (var item in Selection.gameObjects)
        {
#if UNITY_2018
            PrefabUtility.ApplyPrefabInstance(item, InteractionMode.AutomatedAction);
#else
            Object prefabSource = PrefabUtility.GetPrefabParent(item);
            PrefabUtility.ReplacePrefab(item, prefabSource, ReplacePrefabOptions.ConnectToPrefab);
#endif
        }
    }
}