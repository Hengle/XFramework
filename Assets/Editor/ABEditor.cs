using UnityEditor;
using System.IO;
using UnityEngine;
using System;

public class ABEditor
{
    [MenuItem("XDEDZL/AB/Build")]
    public static void BuildAssetBundles()
    {
        // AB包输出路径
        string outPath = Application.streamingAssetsPath + "/AssetBundles";
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

    [MenuItem("Tools/ResourceToAB")]
    static void BuildAssetBundle()
    {
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(Application.streamingAssetsPath);
        }

        //强制删除所有AssetBundle名称  
        string[] abNames = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < abNames.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(abNames[i], true);
        }

        //删除之前的ab文件
        FileInfo[] fs = new DirectoryInfo(Application.streamingAssetsPath + "AssetBundles").GetFiles();
        foreach (var f in fs)
        {
            f.Delete();
        }

        //打包Prefabs
        FindMoudles("Prefabs");
        //打包本地文件
        FindMoudles("Localization");
        //FindTypes("LocalFile/json", ".json");
        //打包贴图
        FindMoudles("Textures");

        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        AssetDatabase.Refresh();
    }

    //按模块组织
    private static void FindMoudles(string pathSuf, string type = null)
    {
        string path = Application.dataPath + "/Resources/" + pathSuf;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        DirectoryInfo dir = new DirectoryInfo(path);
        if (dir.Exists)
            Mark(dir, type);

        DirectoryInfo[] subDirs = dir.GetDirectories();
        foreach (DirectoryInfo dInfo in subDirs)
        {
            string itemPath = path + "/" + dInfo.Name;
            DirectoryInfo subItem = new DirectoryInfo(itemPath);
            if (subItem.Exists)
            {
                Mark(subItem, type);
            }
        }
    }

    //按类型组织
    private static void FindTypes(string pathSuf, string type)
    {
        string path = Application.dataPath + "/Resources/" + pathSuf;
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        DirectoryInfo dir = new DirectoryInfo(path);
        if (dir.Exists)
        {
            Mark(dir, type);
        }
    }

    private static void Mark(DirectoryInfo item, string type = null)
    {
        /*if(item==null) return;
        if (type!=null)
        {
            FileInfo[] files = item.GetFiles();
            foreach (var file in files)
            {
                if (file.Extension != type)
                    continue;
            }
        }
        else
        {
            FileInfo[] files = item.GetFiles();
            foreach (var file in files)
            {
                Debug.Log(file.Name);
                if (file.Extension == ".meta")
                    continue;                   
            }
        }*/
        string filepath = item.FullName.Substring(item.FullName.IndexOf("Assets", StringComparison.Ordinal));
        AssetImporter assetImporter = AssetImporter.GetAtPath(filepath);
        assetImporter.assetBundleName = item.Name;
    }

    [MenuItem("XDEDZL/TEST/Delete")]
    public static void AAA()
    {
        String a = "huangankai";
        String b = "liuyonggang";

        Debug.Log('b' > 'c');
    }
}