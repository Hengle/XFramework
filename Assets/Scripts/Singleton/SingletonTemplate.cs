using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 不继承mono的单例基类，如果需要在单例中写Update类似的函数，需要将函数添加在MonoEvent的事件中
/// </summary>
/// <typeparam name="T"></typeparam>
public class SingletonTemplate<T> : MonoBehaviour where T : new()
{
    private static T _instance;
    private static readonly object objlock = new object();

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (objlock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                }
            }
            return _instance;
        }
    }
}
