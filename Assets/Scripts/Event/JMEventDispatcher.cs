//=====================================================================================/

using System;

/// <summary>
/// 事件分发处理类
/// </summary>
//=====================================================================================.
public class JMEventDispatcher
{    
    private event Action<object, JMEventArgs> EventListener;

    /// <summary>
    /// 分发消息
    /// </summary>    
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public void DispatchEvent(JMEventDispatchType eventType, object data = null)
    {
        if (null != EventListener)
        {
            EventListener(this, new JMEventArgs(eventType, data));
        }
    }

    /// <summary>
    /// 注册监听
    /// </summary>
    public void RegistEvent(Action<object, JMEventArgs> fuc)
    {
        EventListener -= fuc;
        EventListener += fuc;
    }

    /// <summary>
    /// 注销监听
    /// </summary>
    public void UnRegistEvent(Action<object, JMEventArgs> fuc)
    {
        EventListener -= fuc;
    }
}
