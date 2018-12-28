using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoMonoTest
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void Init()
    {
        Debug.Log("HAKULAMATATA");
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void UP()
    {

    }
}