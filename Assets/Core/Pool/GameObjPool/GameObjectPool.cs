using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对象池
/// </summary>
public class GameObjectPool
{
    /// <summary>
    /// 所有对象池的父物体
    /// </summary>
    private static Transform poolRoot;
    /// <summary>
    /// 对象池模板
    /// </summary>
    private GameObject template;
    /// <summary>
    /// 对象池链表
    /// </summary>
    private List<GameObject> pooledObjects;
    /// <summary>
    /// 初始化大小
    /// </summary>
    private readonly int initCount;
    /// <summary>
    /// 对象池数量
    /// </summary>
    private int currentCount;
    /// <summary>
    /// 对象池最大数量
    /// </summary>
    private readonly int maxCount;
    /// <summary>
    /// 是否锁定对象池大小
    /// </summary>
    private readonly bool lockPoolSize = false;
    /// <summary>
    /// 切换场景时是否卸载
    /// 后面默认值要改为false
    /// </summary>
    private bool dontDestroyOnLoad = true;

    public Transform objParent;

    /// <summary>
    /// 当前指向链表位置索引
    /// </summary>
    private int currentIndex = 0;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="_template"></param>
    /// <param name="_initCount"></param>
    /// <param name="_lookPoolSize"></param>
    public GameObjectPool(GameObject _template, bool _dontDestroyOnLoad = false, int _initCount = 5, int _maxCount = 10, bool _lookPoolSize = false)
    {
        template = _template;
        currentCount = initCount = _initCount;
        lockPoolSize = _lookPoolSize;
        dontDestroyOnLoad = _dontDestroyOnLoad;

        objParent = new GameObject(template.name + "Pool").transform;

        poolRoot = poolRoot ?? new GameObject("PoolRoot").transform;
        objParent.SetParent(poolRoot);

        if (dontDestroyOnLoad)
            Object.DontDestroyOnLoad(objParent);

        pooledObjects = new List<GameObject>();             // 初始化链表
        for (int i = 0; i < currentCount; ++i)
        {
            GameObject obj = Object.Instantiate(template); // 创建对象
            if (dontDestroyOnLoad)
                Object.DontDestroyOnLoad(obj);
            obj.SetActive(false);                           // 设置对象无效
            obj.transform.SetParent(objParent);
            pooledObjects.Add(obj);                         // 把对象添加到对象池中
        }
    }

    /// <summary>
    /// 获取对象池中可以使用的对象。
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; ++i)
        {
            //每一次遍历都是从上一次被使用的对象的下一个
            int Item = (currentIndex + i) % pooledObjects.Count;
            if (!pooledObjects[Item].activeSelf)
            {
                currentIndex = (Item + 1) % pooledObjects.Count;
                //返回第一个未激活的对象
                return pooledObjects[Item];
            }
        }

        //如果遍历完一遍对象库发现没有闲置对象且对象池未达到数量限制
        if (!lockPoolSize || currentCount < maxCount)
        {
            GameObject obj = Object.Instantiate(template);
            obj.transform.SetParent(objParent);
            if (dontDestroyOnLoad)
                Object.DontDestroyOnLoad(obj);
            pooledObjects.Add(obj);
            currentCount++;
            return obj;
        }

        //如果遍历完没有而且锁定了对象池大小，返回空。
        return null;
    }

    /// <summary>
    /// 将对象池的数量回归5
    /// </summary>
    public void Clear()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (i > initCount)
            {
                Object.Destroy(pooledObjects[i]);
            }
            else if (pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(false);
            }
        }
    }

    public void Test()
    {

    }
}