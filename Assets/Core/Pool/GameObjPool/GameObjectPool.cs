using System.Collections.Generic;
using UnityEngine;
using XDEDZL.Pool;
using System;

namespace XDEDZL.Pool
{
    /// <summary>
    /// 对象池
    /// </summary>
    public class GameObjectPool : IPool<GameObject>
    {
        /// <summary>
        /// 所有对象池的父物体
        /// </summary>
        private static Transform poolRoot;
        /// <summary>
        /// 当前对象池的父物体
        /// </summary>
        public Transform objParent;

        /// <summary>
        /// 对象池模板
        /// </summary>
        private readonly GameObject template;
        /// <summary>
        /// 初始化大小
        /// </summary>
        private readonly int initCount;
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
        private readonly bool dontDestroyOnLoad = true;

        /// <summary>
        /// 对象池链表
        /// </summary>
        private readonly List<Info> pooledObjects;
        /// <summary>
        /// 回收对象方法
        /// </summary>
        private readonly Func<GameObject, bool> RecycleAction;
        /// <summary>
        /// 对象池数量
        /// </summary>
        private int currentCount;

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
        public GameObjectPool(GameObject _template,Func<GameObject,bool> _RecycleAction = null, bool _dontDestroyOnLoad = false, int _initCount = 5, int _maxCount = 10, bool _lookPoolSize = false)
        {
            template = _template;
            initCount = _initCount;
            lockPoolSize = _lookPoolSize;
            dontDestroyOnLoad = _dontDestroyOnLoad;
            maxCount = _maxCount;

            poolRoot = poolRoot ?? new GameObject("PoolRoot").transform;
            objParent = new GameObject(template.name + "Pool").transform;
            objParent.SetParent(poolRoot);

            if (dontDestroyOnLoad)
                GameObject.DontDestroyOnLoad(objParent);

            pooledObjects = new List<Info>();             // 初始化链表
            for (int i = 0; i < currentCount; ++i)
            {
                Create(template, false);
            }

            RecycleAction = _RecycleAction ?? ((obj) => { obj.SetActive(false); return true; });
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
                    GameObject.Destroy(pooledObjects[i].obj);
                    pooledObjects.Remove(pooledObjects[i]);
                }
                else if (pooledObjects[i].obj.activeInHierarchy)
                {
                    pooledObjects[i].obj.SetActive(false);
                }
            }
        }

        /// <summary>
        /// 获取对象池中可以使用的对象。
        /// </summary>
        public GameObject Allocate()
        {
            for (int i = 0; i < pooledObjects.Count; ++i)
            {
                //每一次遍历都是从上一次被使用的对象的下一个
                int Item = (currentIndex + i) % pooledObjects.Count;
                if (!pooledObjects[Item].used)
                {
                    currentIndex = (Item + 1) % pooledObjects.Count;
                    //返回第一个未激活的对象
                    pooledObjects[Item].used = true;
                    return pooledObjects[Item].obj;
                }
            }

            //如果遍历完一遍对象库发现没有闲置对象且对象池未达到数量限制
            if (!lockPoolSize || currentCount < maxCount)
            {
                Info info = Create(template);
                info.used = false;
                return Create(template).obj;
            }

            return null;
        }

        public bool Recycle(GameObject obj)
        {
            return RecycleAction.Invoke(obj);
        }

        /// <summary>
        /// 为对象池新增一个对象
        /// </summary>
        private Info Create(GameObject template,bool isShow = true)
        {
            Info info = new Info(GameObject.Instantiate(template, objParent), false);
            if (dontDestroyOnLoad)
                GameObject.DontDestroyOnLoad(info.obj);
            pooledObjects.Add(info);
            currentCount++;
            if (!isShow)
                info.obj.SetActive(false);
            return info;
        }

        public class Info
        {
            public GameObject obj;
            public bool used;

            public Info(GameObject _obj,bool _used)
            {
                obj = _obj;
                used = _used;
            }
        }
    }
}