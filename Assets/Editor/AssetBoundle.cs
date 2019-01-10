using UnityEditor;
using System.IO;

public class AssetBundle
{
    [MenuItem("Assets/Build AssetBundles")]
    public static void BuildAssetBundles()
    {
        string dir = "AssetBundle";
        if(Directory.Exists(dir) == false)
        {
            Directory.CreateDirectory(dir);
        }

        BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }
}
