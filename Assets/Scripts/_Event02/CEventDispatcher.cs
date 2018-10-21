using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件分发委托
/// </summary>
/// <param name="evt"></param>
public delegate void CEventListenerDelegate(CBaseEvent evt);

/// <summary>
/// 事件分发管理类
/// </summary>
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

    private Dictionary<CEventType, CEventListenerDelegate> listeners = new Dictionary<CEventType, CEventListenerDelegate>();

    public void AddEventListener(CEventType eventType, CEventListenerDelegate listener)
    {
        if (!listeners.ContainsKey(eventType))
        {
            listeners.Add(eventType, listener);
        }
        else
        {
            listeners[eventType] += listener;
        }
    }

    public void RemoveEventListener(CEventType eventType, CEventListenerDelegate listener)
    {
        if (!listeners.ContainsKey(eventType))
        {
            Debug.Log("xhz,没有这个事件类型");
        }
        else
        {
            listeners[eventType] -= listener;
        }
    }

    public void DispatchEvent(CBaseEvent evt)
    {
        this.listeners[evt.Type]?.Invoke(evt);
    }
}