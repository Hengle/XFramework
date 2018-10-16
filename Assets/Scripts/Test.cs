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

    private void Start()
    {
        cube = GameObject.Find("Cube").transform;
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            cube.rotation = cube.rotation.SubtractRotation(v);
        }
    }
}

public class Player
{
    public int a;
}
