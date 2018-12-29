using PathologicalGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
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
            GameObjectFactory.Instance.Instantiate("Cube", Vector3.up);
            GameObjectFactory.Instance.Instantiate("Cube");
        }
    }
}
