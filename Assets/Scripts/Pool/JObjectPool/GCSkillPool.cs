using PathologicalGames;
using UnityEngine;

public class GCSkillPool : GCPoolBase
{
    private readonly string strPoolName = "SkillPool";


    /// <summary>
    /// 请使用同步家在出池(出池，由于存在异步加载的可能，加载完成调用回调)
    /// </summary>
    /// <param name="objName"></param>
    /// <param name="callback"></param>
   //[System.Obsolete("Use WZSkillPool.SpawnPool instead.")]
    public override void SpawnPoolByName(string objName, OnLoadOK callback)
    {
        Transform trs = spawnPool.Spawn(objName);
        if(trs)
            callback(trs.gameObject);
        else
            SpawnPoolByName(objName, spawnPool, callback);
    }

    /// <summary>
    /// 技能出池，同步加载，即时返回
    /// </summary>
    /// <param name="objName"></param>
    /// <returns></returns>
    public GameObject SpawnPool(string objName)
    {
        Transform trs = spawnPool.Spawn(objName);
        //if (trs)
        //{
        //    trs.transform.SetParent(UITools.Instance.Skill, false);
        //    trs.transform.localScale = Vector3.one;
        //    return trs.gameObject;
        //}
        //else
        //    return PreloadManager.Instance.GetSkill();
        return null;
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
            prefabPool.preloadAmount = 30;
            prefabPool.preloadTime = true;
            prefabPool.preloadFrames = 1;
            prefabPool.cullDespawned = true;
            prefabPool.cullAbove = 50;
            prefabPool.cullDelay = 10;
            prefabPool.cullMaxPerPass = 3;
            spawnPool._perPrefabPoolOptions.Add(prefabPool);
            spawnPool.CreatePrefabPool(prefabPool);
        }
    }

    public override void DespawnObjToPool(Transform trs)
    {
        base.DespawnObjToPool(trs);
    }
    protected override string PoolName
    {
        get
        {
            return strPoolName;
        }
    }

    protected override GCPoolType PoolType
    {
        get
        {
            return GCPoolType.SKILLPOOL;
        }
    }
}