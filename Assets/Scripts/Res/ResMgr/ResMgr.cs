using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ResMgr : MonoSingleton<ResMgr>
{
    private Dictionary<string, string> mAssetPathDic = new Dictionary<string, string>();

    public string GetFileFullName(string assetName)
    {
        if(mAssetPathDic.Count == 0)
        {
            TextAsset tex = Resources.Load<TextAsset>("res");
            StringReader sr = new StringReader(tex.text);
            String fileName = sr.ReadLine();
            while(fileName != null)
            {
                Debug.Log("fileName = " + fileName);
                string[] ss = fileName.Split('=');
                mAssetPathDic.Add(ss[0], ss[1]);
                fileName = sr.ReadLine();
            }
        }
        assetName = mAssetPathDic[assetName] + "/" + assetName;
        return assetName;
    }

    /// <summary>
    /// 所有资源字典
    /// </summary>
    private Dictionary<string, AssetInfo> mDicAsset = new Dictionary<string, AssetInfo>();
    /// <summary>
    /// CPU个数
    /// </summary>
    private int mProcessorCount = 0;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        
    }

    /// <summary>
    /// 正在加载的列表
    /// </summary>
    public List<RequestInfo> mInLoads = new List<RequestInfo>();
    /// <summary>
    /// 等待加载的列表
    /// </summary>
    public Queue<RequestInfo> mWaitting = new Queue<RequestInfo>();
    /// <summary>
    /// 资源加载堆栈
    /// </summary>
    public Stack<List<string>> mAssetStack = new Stack<List<string>>();

    /// <summary>
    /// 加载资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="listener"></param>
    /// <param name="type"></param>
    /// <param name="isKeepInMemory"></param>
    /// <param name="isAsync"></param>
    public void Load(string assetName, IResLoadListener listener, Type type = null, bool isKeepInMemory = false, bool isAsync = true)
    {
        if (mDicAsset.ContainsKey(assetName))
        {
            listener.Finish(mDicAsset[assetName]);
            return;
        }
        if (isAsync)
        {
            LoadAsync(assetName, listener, type, isKeepInMemory);
        }
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="listener"></param>
    /// <param name="type"></param>
    /// <param name="isKeepInMemory"></param>
    private void LoadAsync(string assetName, IResLoadListener listener, Type type, bool isKeepInMemory)
    {
        for (int i = 0; i < mInLoads.Count; i++)
        {
            if(mInLoads[i].assetName == assetName)
            {
                mInLoads[i].AddListener(listener);
                return;
            }
        }

        foreach (RequestInfo info in mWaitting)
        {
            if(info.assetName == assetName)
            {

            }
        }
    }
}
