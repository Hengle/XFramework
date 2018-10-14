using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectFactory{

    /// <summary>
    /// 利用GameObject的Name，索引对应的Pool对象,可以将字符串改成枚举
    /// </summary>
    private Dictionary<string, GameObjectPool> poolDictionary;

    public GameObjectFactory()
    {
        poolDictionary = new Dictionary<string, GameObjectPool>();
    }

    /// <summary>
    /// 配置对象池
    /// </summary>
    public IEnumerator CreatObjPool()
    {
        List<GameObject> objs = new List<GameObject>();
        // 根据需要add多个文件夹的预制体
        objs.AddRange(Resources.LoadAll<GameObject>("GameObjPool"));
        //objs.AddRange(Resources.LoadAll<GameObject>("***"));

        for (int i = 0, length = objs.Count; i < length; i++)
        {
            GameObjectPool _newPool = new GameObjectPool(objs[i]);
            poolDictionary.Add(objs[i].name, _newPool);
            yield return new WaitForFixedUpdate();
        }
    }
    
    /// <summary>
    /// 通过名字实例化gameobj方法
    /// </summary>
    public GameObject Instantiate(string _name, Vector3 _pos, Quaternion _quaternion)
    {
        GameObject _objClone = Instantiate(_name);

        if (_objClone != null)
        {
            _objClone.transform.position = _pos;
            _objClone.transform.rotation = _quaternion;
        }
        return _objClone;
    }

    /// <summary>
    /// 通过名字实例化gameobj方法
    /// </summary>
    public GameObject Instantiate(string _name)
    {
        poolDictionary.TryGetValue(_name, out GameObjectPool objPool);
        GameObject retrunObj = objPool?.GetPooledObject();
        retrunObj?.SetActive(true);
        return retrunObj;
    }
}
