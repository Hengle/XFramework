using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    private void Start()
    {
        Singleton<GameObjectFactory>.Instance.ConfigPool();
        StartCoroutine(Singleton<GameObjectFactory>.Instance.GreatePool()); 
    }
}
