using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class ECSMain : MonoBehaviour
{
    public GameObject rotatorObj; 

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10000; i++)
        {
            float x = Random.Range(-50, 50);
            float y = Random.Range(-50, 50);
            float z = Random.Range(-50, 50);
            Instantiate(rotatorObj,new Vector3(x,y,z), Quaternion.identity);
        }
    }
}
