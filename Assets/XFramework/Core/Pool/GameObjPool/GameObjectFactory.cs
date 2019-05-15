using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XDEDZL.Pool
{
    public class GameObjectFactory : Singleton<GameObjectFactory>
    {
        /// <summary>
        /// 利用GameObject的int型Hash值，索引对应的Pool对象
        /// </summary>
        private readonly Dictionary<string, GameObjectPool> poolDic;
        /// <summary>
        /// 所有模型字典
        /// </summary>
        public Dictionary<string, GameObject> PoolTemplateDic { get; private set; }

        public GameObjectFactory()
        {
            PoolTemplateDic = new Dictionary<string, GameObject>();
            poolDic = new Dictionary<string, GameObjectPool>();
        }

        /// <summary>
        /// 创建一个对象池
        /// </summary>
        /// <param name="template"></param>
        public GameObjectPool CreatPool(GameObject template)
        {
            PoolTemplateDic.Add(template.name, template);
            GameObjectPool _newPol = new GameObjectPool(template);
            poolDic.Add(template.name, _newPol);
            return _newPol;
        }

        /// <summary>
        /// 通过名字实例化gameobj方法
        /// </summary>
        public GameObject Instantiate(string name, Vector3 pos = default, Quaternion quaternion = default)
        {
            if (!PoolTemplateDic.ContainsKey(name))
            {
                throw new System.Exception("已有名为" + name + "的对象池");
            }

            if (!poolDic.TryGetValue(name, out GameObjectPool pool))
            {
                pool = CreatPool(PoolTemplateDic[name]);
            }

            GameObject obj = pool.Allocate().obj;
            obj.transform.position = pos;
            obj.transform.rotation = quaternion;
            obj.SetActive(true);

            return obj;
        }
    }
}