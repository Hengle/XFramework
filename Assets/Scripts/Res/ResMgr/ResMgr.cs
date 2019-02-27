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
                info.AddListener(listener);
                return;
            }
        }

        RequestInfo requestInfo = new RequestInfo();
        requestInfo.assetName = assetName;
        requestInfo.AddListener(listener);
        requestInfo.isKeepInMemory = isKeepInMemory;
        requestInfo.type = type ?? typeof(GameObject);
        mWaitting.Enqueue(requestInfo);
    }

    /// <summary>
    /// 从资源字典中取得一个资源
    /// </summary>
    /// <param name="assetName"></param>
    /// <returns></returns>
    public AssetInfo GetAsset(string assetName)
    {
        AssetInfo info = null;
        mDicAsset.TryGetValue(assetName, out info);
        return info;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="assetName"></param>
    public void ReleaseAsset(string assetName)
    {
        AssetInfo info = null;
        mDicAsset.TryGetValue(assetName, out info);

        if (info != null && !info.isKeepInMemory)
        {
            mDicAsset.Remove(assetName);
        }
    }

    /// <summary>
    /// 修改资源是否常驻内存
    /// </summary>
    /// <param name="assetName"></param>
    /// <param name="IsKeepInMemory"></param>
    public void IsKeepInMemory(string assetName, bool IsKeepInMemory)
    {
        AssetInfo info = null;
        mDicAsset.TryGetValue(assetName, out info);

        if(info != null)
        {
            info.isKeepInMemory = IsKeepInMemory;
        }
    }

    /// <summary>
    /// 把资源压入顶层栈内
    /// </summary>
    /// <param name="assetName"></param>
    public void AddAssetToName(string assetName)
    {
        if(mAssetStack.Count == 0)
        {
            mAssetStack.Push(new List<string>() { assetName });
        }

        List<string> list = mAssetStack.Peek();
        list.Add(assetName);
    }

    /// <summary>
    /// 开始让资源入栈
    /// </summary>
    public void PushAssetStack()
    {
        List<string> list = new List<string>();
        foreach (var item in mDicAsset)
        {
            item.Value.stackCount++;
            list.Add(item.Key);
        }

        mAssetStack.Push(list);
    }

    /// <summary>
    /// 释放栈内资源
    /// </summary>
    public void PopAssetStack()
    {
        if (mAssetStack.Count == 0)
            return;

        List<string> list = mAssetStack.Pop();
        List<string> removeList = new List<string>();
        AssetInfo info = null;
        for (int i = 0; i < list.Count; i++)
        {
            if(mDicAsset.TryGetValue(list[i],out info))
            {
                info.stackCount--;
                if(info.stackCount < 1 && !info.isKeepInMemory)
                {
                    removeList.Add(list[i]);
                }
            }
        }

        for (int i = 0; i < removeList.Count; i++)
        {
            if (mDicAsset.ContainsKey(removeList[i]))
                mDicAsset.Remove(removeList[i]);
        }

        GC();
    }

    /// <summary>
    /// 释放内存
    /// </summary>
    public void GC()
    {
        Resources.UnloadUnusedAssets();
        System.GC.Collect();
    }

    private void Update()
    {
        if (mInLoads.Count > 0)
        {
            for (int i = mInLoads.Count - 1; i >= 0; i--)
            {
                if (mInLoads[i].IsDone)
                {
                    RequestInfo info = mInLoads[i];
                    //SendEvent(EventDef.ResLoadFinish, info);
                    mInLoads.RemoveAt(i);
                }
            }
        }

        while (mInLoads.Count < mProcessorCount && mWaitting.Count > 0)
        {
            RequestInfo info = mWaitting.Dequeue();
            mInLoads.Add(info);
            info.LoadAsync();
        }
    }

    public bool HandleEvent(int id, object param1, object param2)
    {
        switch (id)
        {
            case /*EventDef.ResLoadFinish*/1:
                RequestInfo info = param1 as RequestInfo;
                if (info != null)
                {
                    if (info.Asset != null)
                    {
                        AssetInfo asset = new AssetInfo();
                        asset.isKeepInMemory = info.isKeepInMemory;
                        asset.asset = info.Asset;
                        if (!mDicAsset.ContainsKey(info.assetName))
                        {
                            mDicAsset.Add(info.assetName, asset);
                        }

                        for (int i = 0; i < info.listeners.Count; i++)
                        {
                            if (info.listeners[i] != null)
                            {
                                info.listeners[i].Finish(info.Asset);
                            }
                        }
                        AddAssetToName(info.assetName);
                    }
                }
                else
                {
                    for (int i = 0; i < info.listeners.Count; i++)
                    {
                        if (info.listeners[i] != null)
                        {
                            info.listeners[i].Failure();
                        }
                    }
                }
                return false;
        }
        return false;
    }

    public int EventPriority()
    {
        return 0;
        Dictionary<int, int> a = new Dictionary<int, int>();
    }
}
