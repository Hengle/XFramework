// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-24 16:26:10
// 版本： V 1.0
// ==========================================
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System;

/// <summary>
/// 使用工具类
/// </summary>
public static class Utility
{
    public delegate T Del<T>(T a);

    private static RaycastHit hit;
    /// <summary>
    /// 发射射线并返回RaycastInfo
    /// </summary>
    public static RaycastHit SendRay(int layer = -1)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo, float.MaxValue, layer))
        {
            return hitInfo;
        }
        else
        {
            return default(RaycastHit);
        }
    }
    public static RaycastHit SendRayDown(Vector3 start, int layer = -1)
    {
        RaycastHit hitInfo;
        start.y += 10000;
        if (Physics.Raycast(start, Vector3.down, out hitInfo, float.MaxValue, layer))
        {
            return hitInfo;
        }
        else
        {
            return default(RaycastHit);
        }
    }

    /// <summary>
    /// 创建立方体
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static GameObject CreatPrimitiveType(PrimitiveType type, Vector3 pos = default(Vector3), float size = 1)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * size;
        return obj;
    }
    public static GameObject CreatPrimitiveType(PrimitiveType type, Color color, Vector3 pos = default(Vector3), float size = 1)
    {
        GameObject obj = CreatPrimitiveType(type, pos, size);
        obj.GetComponent<MeshRenderer>().material.color = color;
        return obj;
    }

    ///<summary> 
    /// 序列化 
    /// </summary> 
    /// <param name="data">要序列化的对象</param> 
    /// <returns>返回存放序列化后的数据缓冲区</returns> 
    public static byte[] Serialize(object data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream rems = new MemoryStream();
        formatter.Serialize(rems, data);
        return rems.GetBuffer();
    }

    /// <summary> 
    /// 反序列化 
    /// </summary> 
    /// <param name="data">数据缓冲区</param> 
    /// <returns>对象</returns> 
    public static object Deserialize(byte[] data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream rems = new MemoryStream(data);
        data = null;
        return formatter.Deserialize(rems);
    }

    /// <summary>
    /// 获取一组位置
    /// </summary>
    public static Vector3[] GetPositions(Transform[] trans)
    {
        Vector3[] poses = new Vector3[trans.Length];
        for (int i = 0, length = trans.Length; i < length; i++)
        {
            poses[i] = trans[i].position;
        }
        return poses;
    }
    public static Vector3[] GetPositions(List<Transform> trans)
    {
        Vector3[] poses = new Vector3[trans.Count];
        for (int i = 0, length = trans.Count; i < length; i++)
        {
            poses[i] = trans[i].position;
        }
        return poses;
    }

    /// <summary>
    /// 获取一组欧拉角
    /// </summary>
    public static Vector3[] GetAngles(Transform[] trans)
    {
        Vector3[] angles = new Vector3[trans.Length];
        for (int i = 0, length = trans.Length; i < length; i++)
        {
            angles[i] = trans[i].localEulerAngles;
        }
        return angles;
    }
    public static Vector3[] GetAngles(List<Transform> trans)
    {
        Vector3[] angles = new Vector3[trans.Count];
        for (int i = 0, length = trans.Count; i < length; i++)
        {
            angles[i] = trans[i].localEulerAngles;
        }
        return angles;
    }

    /// <summary>
    /// 打开一个文件
    /// </summary>
    /// <param name="fllePath"></param>
    public static void OpenFile(string filePath)
    {
        Debug.Log(filePath);
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        process.StartInfo.FileName = filePath;
        process.Start();
        process.Dispose();
    }

    /// <summary>
    /// 执行一个方法并返回它的执行时间
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static float DebugActionRunTime(Action action)
    {
        float time =DateTime.Now.Millisecond + DateTime.Now.Second * 1000 + DateTime.Now.Minute * 60000;
        action();
        return DateTime.Now.Millisecond + DateTime.Now.Second * 1000 + DateTime.Now.Minute * 60000 - time;
    }
}