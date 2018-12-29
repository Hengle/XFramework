using System.Collections.Generic;
using UnityEngine;

public enum PoolType
{
    A,
    B,
}

public class PoolManager_0 : MonoBehaviour {

    private Dictionary<PoolType, BaseObjPool> poolDic;
	void Start () {
        Init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Init()
    {
        poolDic.Add(PoolType.A, new AObjPool());
        //poolDic.Add(PoolType.B, new BObjPool());
    }
   
    public GameObject Instantiate(PoolType poolType)
    {
        BaseObjPool objPool;
        poolDic.TryGetValue(poolType, out objPool);
        return objPool.GetObj();
    }
}
