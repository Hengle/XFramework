using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : BaseObjPool {



    public void InitModel()
    {
        
    }

    public GameObject Instantiate(string modelName)
    {
        return new GameObject();
    }
}
