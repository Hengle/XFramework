using System;
using UnityEngine;


//=====================================================================================/
/// <summary>
/// zhoujie
/// 事件分发处理类 继承MonoBehaviour
/// </summary>
//=====================================================================================.
public class JMEventDispatcherMonoBehaviour : MonoBehaviour
{
    private readonly JMEventDispatcher _dispatcher = new JMEventDispatcher();

    /// <summary>
    /// 分发消息
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="data"></param>
    public void DispatchEvent(JMEventDispatchType eventType, object data = null)
    {
        _dispatcher.DispatchEvent(eventType, data);
    }

    /// <summary>
    /// 注册监听
    /// </summary>
    public void RegistEvent(Action<object, JMEventArgs> fuc)
    {
        _dispatcher.RegistEvent(fuc);
    }

    /// <summary>
    /// 注销监听
    /// </summary>
    public void UnRegistEvent(Action<object, JMEventArgs> fuc)
    {
        _dispatcher.UnRegistEvent(fuc);
    }
}
