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
/// 此单例继承于Mono，绝大多情况下，你都不需要使用此单例类型。请使用SingletonTemplate
/// 唯独MonoEvent
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private static readonly object _lock = new object();

    private static bool dontDestroyFindObjOnLoad; // Instance from FindObjectOfType.
    private static bool dontDestroyNewObjOnLoad = true; // Instance from AddComponent.

    static Singleton()
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
                    _instance = (T)FindObjectOfType(typeof(T));

                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogWarning("[Singleton] Something went really wrong " +
                                                    " - there should never be more than 1 singleton!" +
                                                    " Reopenning the scene might fix it.");
                        }

                        return _instance;
                    }

                    if (_instance == null)
                    {
                        var singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T);

                        if (dontDestroyNewObjOnLoad && Application.isPlaying)
                        {
                            DontDestroyOnLoad(singleton);
                        }

                        if (Debug.isDebugBuild)
                        {
                            Debug.LogWarning("[Singleton] An instance of " + typeof(T) +
                                                    " is needed in the scene, so '" + singleton +
                                                    "' was created with DontDestroyOnLoad.");
                        }
                    }
                    else
                    {
                        if (Debug.isDebugBuild)
                        {
                            Debug.LogWarning("[Singleton] Using instance already created: " +
                                                    _instance.gameObject.name);
                        }

                        if (dontDestroyFindObjOnLoad && Application.isPlaying)
                        {
                            DontDestroyOnLoad(_instance.transform.root);
                        }
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
