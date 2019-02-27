using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源信息
/// </summary>
public class AssetInfo
{
    /// <summary>
    /// 资源
    /// </summary>
    public object asset;
    /// <summary>
    /// 是否常驻内存
    /// </summary>
    public bool isKeepInMemory;
    /// <summary>
    /// 资源堆栈数量
    /// </summary>
    public int stackCount = 0;
}

/// <summary>
/// 资源加载信息
/// </summary>
public class RequestInfo
{
    /// <summary>
    /// 资源反馈信息
    /// </summary>
    public ResourceRequest request;
    /// <summary>
    /// 是否常驻内存
    /// </summary>
    public bool isKeepInMemory;
    /// <summary>
    /// 加载完成后的回调
    /// </summary>
    public List<IResLoadListener> listeners;

    public void AddListener(IResLoadListener listener)
    {
        if(listeners == null)
        {
            listeners = new List<IResLoadListener>() { listener };
        }
        else if(!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    /// <summary>
    /// 资源名称
    /// </summary>
    public string assetName;

    public string assetFullName
    {
        get
        {
            //return ResMgr.Instance.GetFileFullName(assetName);
            return "";
        }
    }

    /// <summary>
    /// 资源类型
    /// </summary>
    public Type type;

    /// <summary>
    /// 资源是否加载完成
    /// </summary>
    public bool IsDone
    {
        get
        {
            return (request != null && request.isDone);
        }
    }

    /// <summary>
    /// 加载到的资源
    /// </summary>
    public object Asset
    {
        get
        {
            return request != null ? request.asset : null;
        }
    }

    public void LoadAsync()
    {
        request = Resources.LoadAsync(assetFullName, type);
        
    }
}
