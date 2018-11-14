using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseObjPool {

    protected string path;
    protected int poolAmount;
    protected PoolType poolType;
    protected bool isLock;
    protected GameObject prefabObj;      // 预制
    protected List<GameObject> objList;
    protected int currentIndex;

    /// <summary>
    /// 从对象池获取物体
    /// </summary>
    /// <returns></returns>
    public virtual GameObject GetObj()
    {
        for (int i = 0; i < objList.Count; ++i)       //把对象池遍历一遍
        {
            //每一次遍历都是从上一次被使用的对象的下一个，而不是每次遍历从0开始。
            int Item = (currentIndex + i) % objList.Count;
            if (!objList[Item].activeInHierarchy)
            {
                currentIndex = (Item + 1) % objList.Count;
                return objList[Item];
            }
        }

        //如果没有找到并且没有锁定对象池大小，创建对象并添加到对象池中。
        if (!isLock)          
        {
            GameObject obj = GameObject.Instantiate(prefabObj);
            GameObject.DontDestroyOnLoad(obj);
            objList.Add(obj);
            return obj;
        }

        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }
}
