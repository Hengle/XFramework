//using System;
//using UnityEngine;

///// <summary>
///// 事件分发处理类 继承MonoBehaviour
///// </summary>
//public class EventDispatcherMonoBehaviour : MonoBehaviour
//{
//    private readonly EventDispatcher _dispatcher = new EventDispatcher();

//    /// <summary>
//    /// 分发消息
//    /// </summary>
//    /// <param name="eventType"></param>
//    /// <param name="data"></param>
//    public void DispatchEvent(EventDispatchType eventType, object data = null)
//    {
//        _dispatcher.DispatchEvent(eventType, data);
//    }

//    /// <summary>
//    /// 注册监听
//    /// </summary>
//    public void RegistEvent(Action<object, EventArgs> fuc)
//    {
//        _dispatcher.RegistEvent(fuc);
//    }

//    /// <summary>
//    /// 注销监听
//    /// </summary>
//    public void UnRegistEvent(Action<object, EventArgs> fuc)
//    {
//        _dispatcher.UnRegistEvent(fuc);
//    }
//}