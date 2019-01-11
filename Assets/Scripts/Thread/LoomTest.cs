using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class LoomTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Loom.Initialize();

        DoLoomThread();
    }

    void DoLoomThread()
    {
        int a = 10;
        Loom.RunAsync(() =>
        {
            Thread.Sleep(1000);
            Loom.QueueOnMainThread(() =>
            {
                GameObject go = new GameObject("asd");
            });
            GameObject oo = new GameObject("dasda");
        });
    }
}
