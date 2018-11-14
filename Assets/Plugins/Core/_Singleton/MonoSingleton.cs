using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SingletonType
{
    //全局单例，转场景不会销毁
    GlobalInstance,

    GlobalInstanceDataSpecify,

    //场景单例，只在生成场景内有效
    PerSceneInstance
}

/// <summary>
/// 此单例继承于Mono，绝大多情况下，你都不需要使用此单例类型。请使用Singleton
/// 不需要手动挂载
/// </summary>
public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static readonly object _lock = new object();

    private static bool dontDestroyFindObjOnLoad;       // Instance from FindObjectOfType.
    private static bool dontDestroyNewObjOnLoad = true; // Instance from AddComponent.

    static MonoSingleton()
    {
        ApplicationIsQuitting = false;
    }

    protected SingletonType singletonType
    {
        set
        {
            switch (value)
            {
                case SingletonType.GlobalInstance:
                    dontDestroyFindObjOnLoad = true;
                    dontDestroyNewObjOnLoad = true;
                    break;

                case SingletonType.GlobalInstanceDataSpecify:
                    dontDestroyFindObjOnLoad = true;
                    dontDestroyNewObjOnLoad = false;
                    break;

                case SingletonType.PerSceneInstance:
                    dontDestroyFindObjOnLoad = false;
                    dontDestroyNewObjOnLoad = false;
                    break;
            }
        }
    }

    public static T Instance
    {
        get
        {
            if (ApplicationIsQuitting)
            {
                if (Debug.isDebugBuild)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                            "' already destroyed on application quit." +
                                            " Won't create again - returning null.");
                }

                return null;
            }

            lock (_lock)
            {
                if (_instance == null)
                {
                    // 先在场景中找
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogWarning("[Singleton] Something went really wrong - " + typeof(T).Name +
                                                    " should never be more than 1 in scene!");
                        }

                        return _instance;
                    }

                    // 场景中找不到就创建新物体挂载
                    if (_instance == null)
                    {
                        var singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T);

                        if (dontDestroyNewObjOnLoad && Application.isPlaying)
                        {
                            DontDestroyOnLoad(singleton);
                        }

                        return _instance;
                    }
                }

                return _instance;
            }
        }
    }

    protected static bool ApplicationIsQuitting { get; private set; }

    /// <summary>
    /// 当工程运行结束，在退出时机时候，不允许访问单例
    /// </summary>
    public void OnApplicationQuit()
    {
        ApplicationIsQuitting = true;
    }
}
