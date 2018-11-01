using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL.Mathematics;

public class Test : MonoBehaviour {

    private Timer time;

    private void Start()
    {



        Debug.Log(Math3d.ProjectVectorOnPlane(Vector3.up, new Vector3(2, 2, 0)));
    }

    private void Update()
    {
        
    }

    public void CallBack()
    {
        Debug.Log("CallBack");
    }

    public void CallBack(int a)
    {
        Debug.Log("CallBack T");
    }
}

public class TestClass
{

}
