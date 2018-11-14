using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AObjPool : BaseObjPool {

    /// <summary>
    /// 构造函数，初始化对象池
    /// </summary>
    /// <param name="_count"></param>
    /// <param name="_isLock"></param>
    public AObjPool(int _count = 5, bool _isLock = false)
    {
        path = "Prefabs/A";
        poolAmount = _count;
        poolType = PoolType.A;
        isLock = _isLock;
        prefabObj = Resources.Load(path) as GameObject;
        objList = new List<GameObject>();
        currentIndex = 0;

        for (int i = 0; i < poolAmount; ++i)
        {
            GameObject obj = GameObject.Instantiate(prefabObj); //创建对象
            GameObject.DontDestroyOnLoad(obj);
            obj.SetActive(false);                               //设置对象无效
            objList.Add(obj);                                   //把对象添加到链表（对象池）中
        }
    }
}
