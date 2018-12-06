using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Reflection;

/// <summary>
/// 这个类管理一系列的拓展函数
/// 以后把System和UnityEngine的扩展分开
/// </summary>
public static class ExtenFun
{
    #region Vector相关
    public static Vector3 WithX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }

    public static Vector3 WithY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }

    public static Vector3 WithZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }

    public static Vector2 WithX(this Vector2 v, float x)
    {
        return new Vector2(x, v.y);
    }

    public static Vector2 WithY(this Vector2 v, float y)
    {
        return new Vector2(v.x, y);
    }

    public static Vector3Serializer Serializer(this Vector3 v)
    {
        return new Vector3Serializer(v);
    }
    #endregion

    #region Transform

    /// <summary>
    /// 找寻名字为name的子物体
    /// </summary>
    public static Transform FindRecursive(this Transform transform, string name)
    {
        if (transform.name.Equals(name))
        {
            return transform;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform transform2 = transform.GetChild(i).FindRecursive(name);
            if (transform2 != null)
            {
                return transform2;
            }
        }
        return null;
    }
    #endregion

    #region Rigibogy

    /// <summary>
    /// 重置刚体
    /// </summary>
    public static void ResetDynamics(this Rigidbody body)
    {
        Vector3 zero = Vector3.zero;
        body.angularVelocity = zero;
        body.velocity = zero;
    }

    #endregion

    #region Quaternion 加减貌似只在其他两个轴为0的时候起作用

    /// <summary>
    /// 将q加上rotation并返回
    /// </summary>
    public static Quaternion AddRotation(this Quaternion q, Quaternion rotation)
    {
        return q * rotation;
    }
    public static Quaternion AddRotation(this Quaternion q, Vector3 angle)
    {
        return q * Quaternion.Euler(angle);
    }

    /// <summary>
    /// 将减去rotation并返回
    /// </summary>
    public static Quaternion SubtractRotation(this Quaternion q, Quaternion rotation)
    {
        return q * Quaternion.Inverse(rotation);
    }
    public static Quaternion SubtractRotation(this Quaternion q, Vector3 angle)
    {
        return q * Quaternion.Inverse(Quaternion.Euler(angle));
    }

    #endregion

    #region Collection

    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (T obj in value)
        {
            action(obj);
        }
    }

    public static Value GetValue<Key, Value>(this Dictionary<Key, Value> dic, Key key)
    {
        Value value;
        dic.TryGetValue(key, out value);
        return value;
    }

    public static T[,] Concat0<T>(this T[,] array_0, T[,] array_1)
    {
        if (array_0.GetLength(0) != array_1.GetLength(0))
        {
            Debug.LogError("两个数组第一维不一致");
            return null;
        }
        T[,] ret = new T[array_0.GetLength(0), array_0.GetLength(1) + array_1.GetLength(1)];
        for (int i = 0; i < array_0.GetLength(0); i++)
        {
            for (int j = 0; j < array_0.GetLength(1); j++)
            {
                ret[i, j] = array_0[i, j];
            }
        }
        for (int i = 0; i < array_1.GetLength(0); i++)
        {
            for (int j = 0; j < array_1.GetLength(1); j++)
            {
                ret[i, j + array_0.GetLength(1)] = array_1[i, j];
            }
        }
        return ret;
    }

    public static T[,] Concat1<T>(this T[,] array_0, T[,] array_1)
    {
        if (array_0.GetLength(1) != array_1.GetLength(1))
        {
            Debug.LogError("两个数组第二维不一致");
            return null;
        }
        T[,] ret = new T[array_0.GetLength(0) + array_1.GetLength(0), array_0.GetLength(1)];
        for (int i = 0; i < array_0.GetLength(0); i++)
        {
            for (int j = 0; j < array_0.GetLength(1); j++)
            {
                ret[i, j] = array_0[i, j];
            }
        }
        for (int i = 0; i < array_1.GetLength(0); i++)
        {
            for (int j = 0; j < array_1.GetLength(1); j++)
            {
                ret[i + array_0.GetLength(0), j] = array_1[i, j];
            }
        }
        return ret;
    }

    public static T[,] GetPart<T>(this T[,] array, int base_0, int base_1, int length_0, int length_1)
    {
        if (base_0 + length_0 > array.GetLength(0) || base_1 + length_1 > array.GetLength(1))
        {
            Debug.Log(base_0 + length_0 + ":" + array.GetLength(0));
            Debug.Log(base_1 + length_1 + ":" + array.GetLength(1));
            Debug.LogError("索引超出范围");
            return null;
        }
        T[,] ret = new T[length_0, length_1];
        for (int i = 0; i < length_0; i++)
        {
            for (int j = 0; j < length_1; j++)
            {
                ret[i, j] = array[i + base_0, j + base_1];
            }
        }
        return ret;
    }

    #endregion

    #region UI

    /// <summary>
    /// 给Toggle添加受控制的物体
    /// </summary>
    /// <param name="toggle"></param>
    /// <param name="panel"></param>
    public static void AddCotroledPanel(this Toggle toggle, GameObject panel)
    {
        toggle.onValueChanged.AddListener((a) => panel.SetActive(a));
    }

    #endregion

    #region Reflection

    /// <summary>
    /// 通过反射和函数名调用非公有方法
    /// </summary>
    /// <param name="obj">目标对象</param>
    /// <param name="methodName">函数名</param>
    /// <param name="objs">参数数组</param>
    public static void Invoke(this object obj, string methodName, params object[] objs)
    {
        BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
        Type type = obj.GetType();
        MethodInfo m = type.GetMethod(methodName, flags);
        m.Invoke(obj, objs);
    }

    #endregion

    #region Terrain

    /// <summary>
    /// 右边的地形块
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns></returns>
    public static Terrain Right(this Terrain terrain)
    {
        Vector3 rayStart = terrain.GetPosition() + new Vector3(terrain.terrainData.size.x * 1.5f, 1000, terrain.terrainData.size.z * 0.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
    }

    /// <summary>
    /// 上边的地形块
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns></returns>
    public static Terrain Up(this Terrain terrain)
    {
        Vector3 rayStart = terrain.GetPosition() + new Vector3(terrain.terrainData.size.x * 0.5f, 1000, terrain.terrainData.size.z * 1.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
    }

    /// <summary>
    /// 左边的地形块
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns></returns>
    public static Terrain Left(this Terrain terrain)
    {
        Vector3 rayStart = terrain.GetPosition() + new Vector3(-terrain.terrainData.size.x * 0.5f, 1000, terrain.terrainData.size.z * 0.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
    }

    /// <summary>
    /// 下边的地形块
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns></returns>
    public static Terrain Down(this Terrain terrain)
    {
        Vector3 rayStart = terrain.GetPosition() + new Vector3(terrain.terrainData.size.x * 0.5f, 1000, -terrain.terrainData.size.z * 0.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
    }

    #endregion
}