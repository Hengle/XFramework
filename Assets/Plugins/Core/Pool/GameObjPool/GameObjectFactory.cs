using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectFactory
{

    /// <summary>
    /// 利用GameObject的int型Hash值，索引对应的Pool对象
    /// </summary>
    private Dictionary<string, GameObjectPool> poolDic;
    /// <summary>
    /// 所有模型字典
    /// </summary>
    public Dictionary<string, GameObject> poolTemplateDic { get; private set; }
    /// <summary>
    /// 这个用于LoadingTest的测试
    /// </summary>
    private bool isLoad = true;

    public GameObjectFactory()
    {
        poolTemplateDic = new Dictionary<string, GameObject>();
        poolDic = new Dictionary<string, GameObjectPool>();
    }

    /// <summary>
    /// 读取并预处理指定模型
    /// </summary>
    /// <returns></returns>
    public IEnumerator InitPool()
    {
        if (isLoad)
        {
            //从Resource制定路径下读取模型
            List<UnityEngine.Object> objs = new List<UnityEngine.Object>();
            objs.AddRange(Resources.LoadAll("GameObjectPoll", typeof(GameObject)));
            //objs.AddRange(Resources.LoadAll(" ***path*** ", typeof(GameObject)));

            //便利所有模型进行处理
            foreach (var _obj in objs)
            {
                //将模型加入到字典中
                poolTemplateDic.Add(_obj.name, _obj as GameObject);
            }

            isLoad = false;

            yield return null;

            // 创建对象池
            foreach (var item in poolTemplateDic)
            {
                CreatPool(item.Value);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    /// <summary>
    /// 创建一个对象池
    /// </summary>
    /// <param name="template"></param>
    public void CreatPool(GameObject template)
    {
        GameObjectPool _newPol = new GameObjectPool(template);
        poolDic.Add(template.name, _newPol);
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
        GameObjectPool pool;
        if (poolDic.TryGetValue(_name, out pool))
        {
            GameObject obj = pool.GetPooledObject();
            obj.SetActive(true);
            return obj;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 清理对象池
    /// </summary>
    public void Clear()
    {
        foreach (var item in poolDic)
        {
            item.Value.Clear();
        }
    }
}
