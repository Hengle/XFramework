using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : BaseObjPool {

    private Dictionary<string, GameObject> prefabDic; 

    public void InitModel()
    {
        
    }

    public GameObject Instantiate(string modelName)
    {
        return new GameObject();
    }
}
