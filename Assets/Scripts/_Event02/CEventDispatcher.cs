using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CEventListenerDelegate(CBaseEvent evt);

public class CEventDispatcher {

    static CEventDispatcher instance;
    public static CEventDispatcher GetInstence()
    {
        if (instance == null)
        {
            instance = new CEventDispatcher();
        }
        return instance;
    }

    private Hashtable listeners = new Hashtable();

    public void AddEventListener(CEventType eventType, CEventListenerDelegate listener)
    {
        CEventListenerDelegate cEventListenerDelegate = this.listeners[eventType] as CEventListenerDelegate;
        cEventListenerDelegate = (CEventListenerDelegate)Delegate.Combine(cEventListenerDelegate, listener);
        this.listeners[eventType] = cEventListenerDelegate;
    }

    public void RemoveEventListener(CEventType eventType, CEventListenerDelegate listener)
    {
        CEventListenerDelegate cEventListenerDelegate = this.listeners[eventType] as CEventListenerDelegate;
        if(cEventListenerDelegate != null)
        {
            cEventListenerDelegate = (CEventListenerDelegate)Delegate.Remove(cEventListenerDelegate, listener);
        }
        this.listeners[eventType] = cEventListenerDelegate;
    }

    public void DispatchEvent(CBaseEvent evt)
    {
        CEventListenerDelegate cEventListenerDelegate = this.listeners[evt] as CEventListenerDelegate;
        if(cEventListenerDelegate != null)
        {
            cEventListenerDelegate(evt);
        }
        else
        {
            Debug.Log(evt.Sender + "没有事件");
        }
    }
}
