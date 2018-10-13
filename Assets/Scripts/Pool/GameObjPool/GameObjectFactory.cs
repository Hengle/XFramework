using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectFactory{

    /// <summary>
    /// 利用GameObject的int型Hash值，索引对应的Pool对象
    /// </summary>
    Dictionary<int, GameObjectPool> poolDictionary;
    /// <summary>
    /// 存储预制体及其名字的字典
    /// </summary>
    public Dictionary<string, GameObject> GameObjNameDict { get; private set; }

    public GameObjectFactory()
    {
        poolDictionary = new Dictionary<int, GameObjectPool>();

        GameObjNameDict = new Dictionary<string, GameObject>();
    }

    /// <summary>
    /// 为实体字典中的每一个对象创建一个Pool进行管理
    /// </summary>
    public IEnumerator GreatePool()
    {
        Debug.Log("creatPool");
        foreach (var item in GameObjNameDict)
        {
            Debug.Log("CreatPool Foreach");
            GameObjectPool _newPool = new GameObjectPool(item.Value);
            poolDictionary.Add(item.Value.GetHashCode(), _newPool);
            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// 配置对象池
    /// </summary>
    public void ConfigPool()
    {
        List<GameObject> objs = new List<GameObject>();
        // 根据需要add多个文件夹的预制体
        objs.AddRange(Resources.LoadAll<GameObject>("GameObjPool"));
        //objs.AddRange(Resources.LoadAll<GameObject>("***"));

        for (int i = 0, length = objs.Count; i < length; i++)
        {
            GameObjNameDict.Add(objs[i].name, objs[i]);
        }
    }

    /// <summary>
    /// 实例化GameObject方法，形式Unity原生方法一直，减小修改代价
    /// </summary>
    public GameObject Instantiate(GameObject _obj,Vector3 _pos,Quaternion _quaternion)
    {
        GameObject _objClone = Instantiate(_obj);

        if(_obj != null)
        {
            _objClone.transform.position = _pos;
            _objClone.transform.rotation = _quaternion;
            return _objClone;
        }
        return null;
    }

    /// <summary>
    /// 实例化GameObject方法
    /// </summary>
    public GameObject Instantiate(GameObject _obj)
    {
        GameObjectPool _pool = poolDictionary[_obj.GetHashCode()];
        if (null != _pool)
        {
            GameObject _objClone = _pool.GetPooledObject();
            _objClone.SetActive(true);
            return _objClone;
        }
        return null;
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
            return _objClone;
        }
        return null;
    }

    /// <summary>
    /// 通过名字实例化gameobj方法
    /// </summary>
    public GameObject Instantiate(string _name)
    {
        if (GameObjNameDict.TryGetValue(_name, out GameObject gameObjectTemplate))
        {
            return Instantiate(gameObjectTemplate);
        }
        return null;
    }
}
