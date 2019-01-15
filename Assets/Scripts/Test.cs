using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Vector3 forworad;
    public Vector3 up;
    SpawnPool spawnPool;
    // Start is called before the first frame update
    void Start()
    {
        spawnPool = GetComponent<SpawnPool>();

        StartCoroutine(GameObjectFactory.Instance.InitPool());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)) 
        {
            transform.rotation = Quaternion.LookRotation(forworad, up);
        }
    }

    AAA a = new AAA();
    AAA b = new AAA();

}

public class AAA
{

}
