using PathologicalGames;
using System.Collections.Generic;
using UnityEngine;
using JMResource;

/// <summary>
/// 所有池
/// </summary>

public abstract class GCPoolBase
{
    protected SpawnPool spawnPool;
    public delegate void OnLoadOK(GameObject go);
    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        if (null == spawnPool)
        {
            //string path = "";
            //if (GCPoolManager.Instance.IsStataicPool(PoolType))
            //{
            //    path = UtilityConfig.IphStaticPoolPath + PoolName;
            //}
            //else
            //{
            //    path = UtilityConfig.PoolPath + PoolName;
            //}

            //ResourceItem item = new ResourceItem(path, eRESOURCETYPE.RES_GAMEOBJ, eSTORAGETYPE.STORAGE_SCENE);
            //ResourceCenter.instance.loadSingleResource(item, (obj, name, args) =>
            //{
            //    if (null != obj)
            //    {
            //        GameObject effectGmobj = Object.Instantiate(obj) as GameObject;
            //        spawnPool = effectGmobj.GetComponent<SpawnPool>();
            //        spawnPool.name = PoolName;
            //    }
            //}, null);
        }
    }
    /// <summary>
    /// 按照名称生成池
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="callBack"></param>
    public abstract void SpawnPoolByName(string objName, OnLoadOK callBack);
    /// <summary>
    /// 当遇到请求缓存池中没有的资源的时候，会进入本地加载
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="pool"></param>
    /// <param name="callBack"></param>
    public void SpawnPoolByName(string objName, SpawnPool pool, OnLoadOK callBack)
    {
        LoadStruct loadStruct = new LoadStruct();
        loadStruct.objName = objName;
        loadStruct.poolType = PoolType;
        loadStruct.pool = pool;
        loadStruct.callBack = callBack;
        queLoadStruct.Enqueue(loadStruct);
        LoadNext();
    }
    /// <summary>
    /// 回池
    /// </summary>
    /// <param name="trs"></param>
    public virtual void DespawnObjToPool(Transform trs)
    {
        if (spawnPool.IsSpawned(trs))
        {
            trs.SetParent(spawnPool.group);
            spawnPool.Despawn(trs);
        }
        else
            Object.Destroy(trs.gameObject);
    }
    /// <summary>
    /// 加入池
    /// </summary>
    /// <param name="temobj"></param>
    public abstract void OutReroucesJoinToPool(GameObject temobj);
    /// <summary>
    /// 池名称
    /// </summary>
    protected abstract string PoolName
    {
        get;
    }
    /// <summary>
    /// 池类型
    /// </summary>
    protected abstract GCPoolType PoolType
    {
        get;
    }

    #region 静态方法
    private static Queue<LoadStruct> queLoadStruct = new Queue<LoadStruct>();
    private static bool nowLoad = false;
    /// <summary>
    /// 当从缓存池没有取到对应的prefab时，临时加载资源
    /// </summary>
    private static string m_prefabPath;
    /// <summary>
    /// 加载下一步
    /// </summary>
    private static void LoadNext()
    {
        if (!nowLoad && queLoadStruct.Count > 0)
        {
            LoadStruct loadStruct = queLoadStruct.Dequeue();
            Transform trm = loadStruct.pool.Spawn(loadStruct.objName);
            if (trm == null)
            {
                LoadPrefabOutPool(loadStruct.poolType,loadStruct.objName, loadStruct.callBack);
                nowLoad = true;
            }
            else
            {
                loadStruct.callBack(trm.gameObject);
                LoadNext();
            }
        }
    }
    /// <summary>
    /// 加载预制体
    /// </summary>
    /// <param name="PoolType">池类型</param>
    /// <param name="prefabName">预制体名称</param>
    /// <param name="callBack">回调</param>
    private static void LoadPrefabOutPool(GCPoolType PoolType,  string prefabName, OnLoadOK callBack)
    {
        //switch (PoolType)
        //{
        //    case GCPoolType.PUBLICPOOL:
        //        {
        //            m_prefabPath = UtilityConfig.PublicPrefabPath + prefabName;
        //            break;
        //        }
        //}

        //ResourceItem item = new ResourceItem(m_prefabPath, eRESOURCETYPE.RES_GAMEOBJ, eSTORAGETYPE.STORAGE_SCENE, true, true);
        //if (item == null)
        //{
        //    Debug.LogError("缓存池没有对象时Load需求的prefab失败,请检查路径是否正确,对应的prefab 名称为：" + prefabName);
        //}
        //ResourceCenter.instance.loadSingleResource(item, OnLoadSingleResourceOK, null, callBack);
    }

    private static void OnLoadSingleResourceOK(Object obj, string name, params object[] args)
    {
        OnLoadOK loadOKCallBack = args[0] as OnLoadOK;
        if (null != loadOKCallBack)
        {
            if (null != obj)
            {
                GameObject go = Object.Instantiate(obj) as GameObject;
                go.name = obj.name;
                loadOKCallBack(go);
            }
            else
            {
                loadOKCallBack(null);
            }
        }
        nowLoad = false;
        LoadNext();
    }

    #endregion 通用方法
}
public class LoadStruct
{
    public string objName;
    public GCPoolType poolType;
    public SpawnPool pool;
    public GCPoolBase.OnLoadOK callBack;
}


public enum GCPoolType
{
    PUBLICPOOL,
    CHARACTERPOOL,
    SKILLPOOL,
}