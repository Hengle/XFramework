using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL.Mathematics;
using UniRx;

public class Test : MonoBehaviour {

    private Timer time;

    private void Start()
    {
        Observable.Range(1, 10).Where((x) => 
        x % 2 == 0).
        Subscribe((arg)=>
        {
            Debug.Log(arg);
        });
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
