using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    private Timer time;

    private void Start()
    {
        time = new Timer(1000, 3);
        time.AddEventListener(EventDispatchType.TIME_RUNCHANGE, AAA);
        //time.Start();

        StartCoroutine(BBB());


        var arr = Enum.GetValues(typeof(EventDispatchType));
        foreach (var item in arr)
        {
            Debug.Log((EventDispatchType)item);
        }
    }

    private void Update()
    {
        
    }

    IEnumerator BBB()
    {
        yield return new WaitForSeconds(2);
        time.Start();
        time.Stop();
    }

    public void AAA(object a, EventArgs b)
    {
        Debug.Log(b.data[0]);
    }
}

public class TestClass
{

}
