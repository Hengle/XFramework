using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

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

    /// <summary>
    /// 对枚举器的所以数据进行某种操作
    /// </summary>
    /// <typeparam name="T">目标对象</typeparam>
    /// <param name="value">目标</param>
    /// <param name="action">操作事件</param>
    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (T obj in value)
        {
            action(obj);
        }
    }

    /// <summary>
    /// 转变数组类型
    /// </summary>
    /// <typeparam name="T">目标类型</typeparam>
    /// <param name="array">原数组</param>
    /// <returns></returns>
    public static T[] Convert<T>(this Array array) where T : class
    {
        T[] tArray = new T[array.Length];
        for (int i = 0; i < array.Length; i++)
        {
            tArray[i] = array.GetValue(i) as T;
        }
        return tArray;
    }

    /// <summary>
    /// 根据Key获取字典的一个值
    /// </summary>
    public static Value GetValue<Key, Value>(this Dictionary<Key, Value> dic, Key key)
    {
        Value value;
        dic.TryGetValue(key, out value);
        return value;
    }

    /// <summary>
    /// 连接两个数组的第一维返回一个新数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
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

    /// <summary>
    /// 连接两个数组的第二位返回一个新数组
    /// </summary>
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

    /// <summary>
    /// 获取一个二维数组的某一部分并返回
    /// </summary>
    /// <param name="array">目标数组</param>
    /// <param name="base_0">第一维的起始索引</param>
    /// <param name="base_1">第二维的起始索引</param>
    /// <param name="length_0">第一维要获取的数据长度</param>
    /// <param name="length_1">第二维要获取的数据长度</param>
    /// <returns></returns>
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


    /// <summary>
    /// 获取除_exclude之外的集合
    /// </summary>
    /// <param name="_sourList"></param>
    /// <param name="_exclude"></param>
    /// <returns></returns>
    public static List<T> WithOut<T>(this List<T> _sourList, T _exclude) where T : class
    {
        List<T> outList = new List<T>();
        foreach (var item in _sourList)
        {
            if (item != _exclude)
            {
                outList.Add(item);
            }
        }
        return outList;
    }

    /// <summary>
    /// 获取除_exclude点之外的集合
    /// </summary>
    /// <param name="_sourList"></param>
    /// <param name="_exclude"></param>
    /// <returns></returns>
    public static List<Vector3> WithOut(this List<Vector3> _sourList, Vector3 _exclude)
    {
        List<Vector3> outList = new List<Vector3>();
        foreach (var item in _sourList)
        {
            if (item != _exclude)
            {
                outList.Add(item);
            }
        }
        return outList;
    }

    /// <summary>
    /// 获取一组点的平均位置
    /// </summary>
    /// <param name="_sourList"></param>
    /// <returns></returns>
    public static Vector3 AveragePos(this List<Vector3> _sourList)
    {
        float x = 0, y = 0, z = 0, count = _sourList.Count;
        foreach (var item in _sourList)
        {
            x += item.x;
            y += item.y;
            z += item.z;
        }
        return new Vector3(x / count, y / count, z / count);
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
#if UNITY_2018
        return terrain.rightNeighbor;
#else
        Vector3 rayStart = terrain.GetPosition() + new Vector3(terrain.terrainData.size.x * 1.5f, 10000, terrain.terrainData.size.z * 0.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
#endif
    }

    /// <summary>
    /// 上边的地形块
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns></returns>
    public static Terrain Top(this Terrain terrain)
    {
#if UNITY_2018
        return terrain.topNeighbor;
#else
        Vector3 rayStart = terrain.GetPosition() + new Vector3(terrain.terrainData.size.x * 0.5f, 10000, terrain.terrainData.size.z * 1.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
#endif
    }

    /// <summary>
    /// 左边的地形块
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns></returns>
    public static Terrain Left(this Terrain terrain)
    {
#if UNITY_2018
        return terrain.leftNeighbor;
#else
        Vector3 rayStart = terrain.GetPosition() + new Vector3(-terrain.terrainData.size.x * 0.5f, 10000, terrain.terrainData.size.z * 0.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
#endif
    }

    /// <summary>
    /// 下边的地形块
    /// </summary>
    /// <param name="terrain"></param>
    /// <returns></returns>
    public static Terrain Bottom(this Terrain terrain)
    {
#if UNITY_2018
        return terrain.bottomNeighbor;
#else
        Vector3 rayStart = terrain.GetPosition() + new Vector3(terrain.terrainData.size.x * 0.5f, 10000, -terrain.terrainData.size.z * 0.5f);
        RaycastHit hitInfo;
        Physics.Raycast(rayStart, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
#endif
    }

    #endregion

    #region Bounds

    /// <summary>
    /// 绘制一个包围盒
    /// </summary>
    /// <param name="bounds"></param>
    /// <param name="color"></param>
    public static void Draw(this Bounds bounds, Color color)
    {
        var e = bounds.extents;
        Debug.DrawLine(bounds.center + new Vector3(+e.x, +e.y, +e.z), bounds.center + new Vector3(-e.x, +e.y, +e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(+e.x, -e.y, +e.z), bounds.center + new Vector3(-e.x, -e.y, +e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(+e.x, -e.y, -e.z), bounds.center + new Vector3(-e.x, -e.y, -e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(+e.x, +e.y, -e.z), bounds.center + new Vector3(-e.x, +e.y, -e.z), color);

        Debug.DrawLine(bounds.center + new Vector3(+e.x, +e.y, +e.z), bounds.center + new Vector3(+e.x, -e.y, +e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(-e.x, +e.y, +e.z), bounds.center + new Vector3(-e.x, -e.y, +e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(-e.x, +e.y, -e.z), bounds.center + new Vector3(-e.x, -e.y, -e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(+e.x, +e.y, -e.z), bounds.center + new Vector3(+e.x, -e.y, -e.z), color);

        Debug.DrawLine(bounds.center + new Vector3(+e.x, +e.y, +e.z), bounds.center + new Vector3(+e.x, +e.y, -e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(+e.x, -e.y, +e.z), bounds.center + new Vector3(+e.x, -e.y, -e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(-e.x, +e.y, +e.z), bounds.center + new Vector3(-e.x, +e.y, -e.z), color);
        Debug.DrawLine(bounds.center + new Vector3(-e.x, -e.y, +e.z), bounds.center + new Vector3(-e.x, -e.y, -e.z), color);
    }

    #endregion
}