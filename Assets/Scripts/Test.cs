using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Test : MonoBehaviour {

    Transform cube;
    public float f;
    public Vector3 v;
    private List<GameObject> objs = new List<GameObject>();
    private bool isShow = true;

    private void Start()
    {
        StartCoroutine( Singleton<GameObjectFactory>.Instance.CreatObjPool());
        for (int i = 0; i < 1000; i++)
        {
            GameObject obj = Singleton<GameObjectFactory>.Instance.Instantiate("Cube");
            obj.transform.position = Vector3.zero + new Vector3(Random.Range(-100, 100), Random.Range(-100, 100), Random.Range(-100, 100));
            objs.Add(obj);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            isShow = !isShow;
            for (int i = 0,length = objs.Count; i < length; i++)
            {
                objs[i].SetActive(isShow);
            }
        }


    }
}
