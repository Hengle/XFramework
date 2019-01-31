using UnityEditor;
using System.IO;
using UnityEngine;

public class ABEditor
{
    [MenuItem("XDEDZL/AB/Build")]
    public static void BuildAssetBundles()
    {
        // AB包输出路径
        string outPath = Application.streamingAssetsPath + "/QAB";
        // 检查路径是否存在
        CheckDirAndCreate(outPath);
        BuildPipeline.BuildAssetBundles(outPath, 0, EditorUserBuildSettings.activeBuildTarget);
        // 刚创建的文件夹和目录能马上再Project视窗中出现
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 判断路径是否存在,不存在则创建
    /// </summary>
    public static void CheckDirAndCreate(string dirPath)
    {
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }
}
