using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池
/// </summary>
public class GameObjectPool{

    private GameObject myObject;                          // 对象perfabs
    private int pooledAmount;                             // 对象池初始大小
    private bool lockPoolSize = false;                    // 是否锁定对象池大小

    private List<GameObject> pooledObjects;                 // 对象池链表

    private int currentIndex = 0;                           // 当前指向链表位置索引

    public GameObjectPool(GameObject _Obj, int _pooledAmount = 5, bool _lookPoolSize = false)
    {
        myObject = _Obj;
        pooledAmount = _pooledAmount;
        lockPoolSize = _lookPoolSize;

        pooledObjects = new List<GameObject>();             // 初始化链表
        for (int i = 0; i < pooledAmount; ++i)
        {
            Debug.Log("创建对象");
            GameObject obj = GameObject.Instantiate(myObject); // 创建对象
            GameObject.DontDestroyOnLoad(obj);
            obj.SetActive(false);                           // 设置对象无效
            pooledObjects.Add(obj);                         // 把对象添加到链表（对象池）中
        }
    }

    public GameObject GetPooledObject()                     // 获取对象池中可以使用的对象。
    {
        for (int i = 0; i < pooledObjects.Count; ++i)       // 把对象池遍历一遍
        {
            //这里简单优化了一下，每一次遍历都是从上一次被使用的对象的下一个，而不是每次遍历从0开始。
            //例如上一次获取了第4个对象，currentIndex就为5，这里从索引5开始遍历，这是一种贪心算法。
            int Item = (currentIndex + i) % pooledObjects.Count;
            if (!pooledObjects[Item].activeInHierarchy) //判断该对象是否在场景中激活。
            {
                currentIndex = (Item + 1) % pooledObjects.Count;
                return pooledObjects[Item];             //找到没有被激活的对象并返回
            }
        }

        //如果遍历完一遍对象库发现没有可以用的，执行下面
        if (!lockPoolSize)                               //如果没有锁定对象池大小，创建对象并添加到对象池中。
        {
            GameObject obj = GameObject.Instantiate(myObject);
            GameObject.DontDestroyOnLoad(obj);
            pooledObjects.Add(obj);
            return obj;
        }

        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }
}
