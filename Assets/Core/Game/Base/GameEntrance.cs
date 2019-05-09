using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XDEDZL;

public class GameEntrance : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        GameEntry.ModelUpdate(Time.deltaTime, Time.unscaledDeltaTime);
    }
}