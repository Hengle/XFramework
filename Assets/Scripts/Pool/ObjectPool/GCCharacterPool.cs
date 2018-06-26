using PathologicalGames;
using UnityEngine;

/// <summary>
/// 角色池
/// </summary>
public class GCCharacterPool : GCPoolBase
{
    private readonly string strPoolName = "CharacterPool";

    /// <summary>
    /// 出池，由于存在异步加载的可能，加载完成调用回调
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="callback"></param>
    public override void SpawnPoolByName(string objName, OnLoadOK callback)
    {
        Transform trs = spawnPool.Spawn(objName);
        if (trs)
            callback(trs.gameObject);
        else
            SpawnPoolByName(objName, spawnPool, callback);
    }
    /// <summary>
    /// 资源加载好进入缓存池
    /// </summary>
    /// <param name="temobj"></param>
    public override void OutReroucesJoinToPool(GameObject temobj)
    {
        if (!spawnPool.prefabPools.ContainsKey(temobj.name))
        {
            spawnPool.m_bIsNotPrefabRes = true;
            PrefabPool prefabPool = new PrefabPool(temobj.transform);
            prefabPool.preloadAmount = 20;
            prefabPool.preloadTime = true;
            prefabPool.preloadFrames = 1;
            prefabPool.cullDespawned = true;
            prefabPool.cullAbove = 30;
            prefabPool.cullDelay = 10;
            prefabPool.cullMaxPerPass = 3;
            spawnPool._perPrefabPoolOptions.Add(prefabPool);
            spawnPool.CreatePrefabPool(prefabPool);
        }
    }


    /// <summary>
    /// 回收
    /// </summary>
    /// <param name="trns"></param>
    public override void DespawnObjToPool(Transform trs)
    {
        base.DespawnObjToPool(trs);
    }
    /// <summary>
    /// 池名称
    /// </summary>
    protected override string PoolName
    {
        get
        {
            return strPoolName;
        }
    }
    /// <summary>
    /// 池类型
    /// </summary>
    protected override GCPoolType PoolType
    {
        get
        {
            return GCPoolType.CHARACTERPOOL;
        }
    }
}