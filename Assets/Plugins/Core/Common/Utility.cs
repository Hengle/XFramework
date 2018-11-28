// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-24 16:26:10
// 版本： V 1.0
// ==========================================
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

/// <summary>
/// 使用工具类
/// </summary>
public static class Utility {

    public delegate T Del<T>(T a);

    private static RaycastHit hit;
    /// <summary>
    /// 发射射线并范围RaycastInfo
    /// </summary>
    public static RaycastHit SendRay(int layer = 0)
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

    /// <summary>
    /// 创建立方体
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    public static GameObject CreatPrimitiveType(PrimitiveType type, Vector3 pos = default(Vector3), float size = 1,Color color = default(Color))
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.transform.position = pos;
        obj.transform.localScale = Vector3.one * size;
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
        for (int i = 0,length = trans.Length; i < length; i++)
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
    /// 左键点击获取点
    /// </summary>
    /// <param name="positions"></param>
    public static void MouseAddPoints(List<Vector3> positions, Del<Vector3> action = null)
    {
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            hit = SendRay();
            if (hit.Equals(default(RaycastHit)))
            {
                Vector3 worldHitPos = hit.point;

                if (action != null)
                {
                    worldHitPos = action(worldHitPos);
                }

                positions.Add(worldHitPos);
                GameObject gameObject = CreatPrimitiveType(PrimitiveType.Cube, worldHitPos, 1f);
            }
        }
    }
}