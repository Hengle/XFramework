using System;
using System.Collections.Generic;

/// <summary>
/// 消息类型
/// </summary>
public class MessageEventType
{    
    /// <summary>
    /// 读表完成
    /// </summary>
    public const string EVENT_GAMEDB_OK = "EVENT_GAMEDB_OK";

    /// <summary>
    /// 隐藏UI
    /// </summary>
    public const string HIDE_UI_ON_CHANGE_SCENE = "HIDE_UI_ON_CHANGE_SCENE";

    /// <summary>
    /// 正在切换场景
    /// </summary>
    public const string ON_CHANGESTAGE_START = "ON_CHANGESTAGE_START";

    /// <summary>
    /// 当场景切换完成
    /// </summary>
    public const string ON_CHANGESTAGE_ALLREADY = "ON_CHANGESTAGE_ALLREADY";
    
    /// <summary>
    /// 音乐开关
    /// </summary>
    public const string ON_CHANGESOUND_TOGGLE = "ON_CHANGESOUND_TOGGLE";

    #region 战斗相关
    /// <summary>
    /// 移除战斗单元
    /// </summary>
    public const string REMOVE_UNIT = "REMOVE_UNIT";

    /// <summary>
    /// 战斗场景长按
    /// </summary>
    public const string EVENT_BATTLESCENE_ONPRESSDOWN = "EVENT_BATTLESCENE_ONPRESSDOWN";

    /// <summary>
    /// 战斗场景松开长按
    /// </summary>
    public const string EVENT_BATTLESCENE_ONPRESSUP = "EVENT_BATTLESCENE_ONPRESSUP";

    /// <summary>
    /// 战斗场景离开长按
    /// </summary>
    public const string EVENT_BATTLESCENE_ONPRESSEXIT = "EVENT_BATTLESCENE_ONPRESSEXIT";

    /// <summary>
    /// 战斗场景单元技能释放完毕
    /// </summary>
    public const string EVENT_BATTLESCENE_UNITSKILLCOMPLETERELEASE = "EVENT_BATTLESCENE_UNITSKILLCOMPLETERELEASE";

    /// <summary>
    /// 战场单元全部就位
    /// </summary>
    public const string EVENT_BATTLESCENE_UNIT_INPLACE = "EVENT_BATTLESCENE_UNIT_INPLACE";

    /// <summary>
    /// 技能特写动画结束
    /// </summary>
    public const string EVENT_TASK_BATTLESKILL_ANIMATION_END = "EVENT_TASK_BATTLESKILLBEGIN";
    #endregion
}

public class BroadcastException : Exception
{
    public BroadcastException(string msg) : base(msg)
    {
    }
}

public class List : Exception
{
    public List(string msg) : base(msg)
    {
    }
}

//=====================================================================================/
/// <summary>
/// zhoujie
/// 消息类 全局类消息
/// </summary>
//=====================================================================================.
public class Messenger : SingletonTemplate<Messenger>
{
    public delegate void Callback();

    public delegate void Callback<T>(T arg1);

    public delegate void Callback<T, U>(T arg1, U arg2);

    public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);

    public Dictionary<string, Delegate> m_eventDictionary = new Dictionary<string, Delegate>();

    ~Messenger()
    {
        Cleanup();
    }

    #region AddEventListener

    public void AddEventListener(string eventType, Callback handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback)m_eventDictionary[eventType] + handler;
    }

    //一个参数 parameter
    public void AddEventListener<T>(string eventType, Callback<T> handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T>)m_eventDictionary[eventType] + handler;
    }

    //两个参数 parameter
    public void AddEventListener<T, U>(string eventType, Callback<T, U> handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U>)m_eventDictionary[eventType] + handler;
    }

    //三个参数 parameter
    public void AddEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        OnListenerAdding(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U, V>)m_eventDictionary[eventType] + handler;
    }

    #endregion AddEventListener

    #region RemoveEventListener

    public void RemoveEventListener(string eventType, Callback handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    public void RemoveEventListener<T>(string eventType, Callback<T> handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T>)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    public void RemoveEventListener<T, U>(string eventType, Callback<T, U> handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U>)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    public void RemoveEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        OnListenerRemoving(eventType, handler);
        m_eventDictionary[eventType] = (Callback<T, U, V>)m_eventDictionary[eventType] - handler;
        OnListenerRemoved(eventType);
    }

    #endregion RemoveEventListener

    #region OnListenerAdding OnListenerRemoving

    private void OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
        if (!m_eventDictionary.ContainsKey(eventType))
        {
            m_eventDictionary.Add(eventType, null);
        }

        Delegate d = m_eventDictionary[eventType];

        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {
            throw new List(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    private void OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                throw new List(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
            }
            else if (d.GetType() != listenerBeingRemoved.GetType())
            {
                throw new List(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
            }
        }
        else
        {
            throw new List(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
        }
    }

    private void OnListenerRemoved(string eventType)
    {
        if (m_eventDictionary[eventType] == null)
        {
            m_eventDictionary.Remove(eventType);
        }
    }

    #endregion OnListenerAdding OnListenerRemoving

    #region BroadCastEventMsg

    public void BroadCastEventMsg(string eventType)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback callback = d as Callback;

            if (callback != null)
            {
                callback();
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public void BroadCastEventMsg<T>(string eventType, T arg1)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback<T> callback = d as Callback<T>;

            if (callback != null)
            {
                callback(arg1);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public void BroadCastEventMsg<T, U>(string eventType, T arg1, U arg2)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback<T, U> callback = d as Callback<T, U>;

            if (callback != null)
            {
                callback(arg1, arg2);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    public void BroadCastEventMsg<T, U, V>(string eventType, T arg1, U arg2, V arg3)
    {
        Delegate d;
        if (m_eventDictionary.TryGetValue(eventType, out d))
        {
            Callback<T, U, V> callback = d as Callback<T, U, V>;

            if (callback != null)
            {
                callback(arg1, arg2, arg3);
            }
            else
            {
                throw CreateBroadcastSignatureException(eventType);
            }
        }
    }

    #endregion BroadCastEventMsg

    #region CheckEventListener

    public bool CheckEventListener(string eventType, Callback handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    //Single parameter
    public bool CheckEventListener<T>(string eventType, Callback<T> handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    //Two parameters
    public bool CheckEventListener<T, U>(string eventType, Callback<T, U> handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    //Three parameters
    public bool CheckEventListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        if (m_eventDictionary.ContainsKey(eventType))
        {
            Delegate d = m_eventDictionary[eventType];

            if (d == null)
            {
                return false;
            }
            else if (d.GetType() != handler.GetType())
            {
                return false;
            }

            return true;
        }

        return false;
    }

    public BroadcastException CreateBroadcastSignatureException(string eventType)
    {
        return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }

    #endregion CheckEventListener

    public void Cleanup()
    {
        m_eventDictionary.Clear();
    }

    public void Log()
    {
        
    }
}