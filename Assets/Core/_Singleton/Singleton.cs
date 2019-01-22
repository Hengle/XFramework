using System;
/// <summary>
/// 不继承mono的单例基类，如果需要Update，可以将方法注册进MonoEvent的事件中
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : new()
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

public static class SingletonCreator
{
    public static T CreatSingleton<T>() where T : class
    {
        T instance = Activator.CreateInstance(typeof(T)) as T;
        return instance;
    }
}