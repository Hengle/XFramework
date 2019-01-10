using System;
using UnityEngine;

public class XLogger : MonoBehaviour
{
    public static bool bDebug = true;

    public static void SetDebug(bool bValue)
    {
        XLogger.bDebug = bValue;
    }

    public static void Log(string info)
    {
        if (!XLogger.bDebug)
            return;
        Debug.Log((object)info);
    }

    public static void Log(string info, UnityEngine.Object obj)
    {
        if (!XLogger.bDebug)
            return;
        Debug.Log((object)info, obj);
    }

    public static void LogWarning(string info, UnityEngine.Object obj)
    {
        if (!XLogger.bDebug)
            return;
        Debug.LogWarning((object)info, obj);
    }

    public static void LogWarning(string info)
    {
        if (!XLogger.bDebug)
            return;
        Debug.LogWarning((object)info);
    }

    public static void LogError(string info)
    {
        if (!XLogger.bDebug)
            return;
        Debug.LogError((object)info);
    }

    public static void LogError(string info, UnityEngine.Object obj)
    {
        if (!XLogger.bDebug)
            return;
        Debug.LogError((object)info, obj);
    }

    public static void LogException(Exception info)
    {
        if (!XLogger.bDebug)
            return;
        Debug.LogException(info);
    }

    public static void LogException(Exception info, UnityEngine.Object obj)
    {
        if (!XLogger.bDebug)
            return;
        Debug.LogException(info, obj);
    }
}
