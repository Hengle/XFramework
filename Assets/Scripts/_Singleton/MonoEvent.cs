using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoEvent : Singleton<MonoEvent>
{
    public MonoEvent()
    {
        singletonType = SingletonType.GlobalInstance;
    }

    public event Action UPDATE;
    public event Action FIXEDUPDATE;
    public event Action ONGUI;
    public event Action LATEUPDATE;

    private void Update()
    {
        if (UPDATE != null)
        {
            UPDATE();
        }
    }

    private void FixedUpdate()
    {
        if (FIXEDUPDATE != null)
        {
            FIXEDUPDATE();
        }
    }

    private void OnGUI()
    {
        if (ONGUI != null)
        {
            ONGUI();
        }
    }

    private void LateUpdate()
    {
        if (LATEUPDATE != null)
        {
            LATEUPDATE();
        }
    }
}