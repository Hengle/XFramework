using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

    private Timer time;

    private void Start()
    {
        Messenger.Instance.AddEventListener(MessageEventType.A, CallBack);
        Messenger.Instance.AddEventListener<int>(MessageEventType.A, CallBack);
        Messenger.Instance.BroadCastEventMsg(MessageEventType.A);
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
