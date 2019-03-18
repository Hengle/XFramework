// ==========================================
// 描述： 
// 作者： LYG
// 时间： 2018-11-13 08:38:23
// 版本： V 1.0
// ==========================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 提供了一种基于字节流的协议
/// 字节流协议是一种最基本的协议。它把所有参数放入byte[]结构中，客户端和服务端按照约定的数据类型和顺序解析各个参数。本字节流协议支持int、float和string三种数据类型。
/// 不用Array.concat的原因是当数据量太大时，不断的执行concat会非常慢
public class ProtocolBytes
{
    private static byte filter = 255;

    private byte[] bytes;     //传输的字节流
    private List<byte> byteList;

    public int index { get;private set; }

    public ProtocolBytes()
    {
        index = 0;
        byteList = new List<byte>();
    }

    public ProtocolBytes(byte[] _bytes)
    {
        index = 0;
        bytes = _bytes;
        byteList = new List<byte>(_bytes);
    }

    /// <summary>
    /// 编码器
    /// </summary>
    /// <returns></returns>
    public byte[] Encode()
    {
        return byteList.ToArray();
    }

    /// <summary>
    /// 协议内容 提取每一个字节并组成字符串 用于查看消息
    /// </summary>
    /// <returns></returns>
    public string GetDesc()
    {
        string str = "";
        if (bytes == null) return str;
        for (int i = 0; i < bytes.Length; i++)
        {
            int b = (int)bytes[i];
            str += b.ToString() + " ";
        }
        return str;
    }

    #region 添加和获取字符串

    /// <summary>
    /// 将字符转转为字节数组加入字节流
    /// </summary>
    /// <param name="str">要添加的字符串</param>
    public void AddString(string str)
    {
        Int32 len = str.Length;
        byte[] lenBytes = BitConverter.GetBytes(len);
        byte[] strBytes = Encoding.UTF8.GetBytes(str);
        byteList.AddRange(lenBytes);
        byteList.AddRange(strBytes);
    }

    /// <summary>
    /// 将字节数组转化为字符串
    /// </summary>
    /// <param name="index">索引起点</param>
    /// <param name="end">为下一个转换提供索引起点</param>
    /// <returns></returns>
    public string GetString()
    {
        if (bytes == null)
            return "";
        if (bytes.Length < index + sizeof(int))
            return "";
        int strLen = BitConverter.ToInt32(bytes, index);
        if (bytes.Length < index + sizeof(int) + strLen)
            return "";
        string str = Encoding.UTF8.GetString(bytes, index + sizeof(int), strLen);
        index = index + sizeof(int) + strLen;
        return str;
    }

    #endregion

    #region 添加获取整数

    /// <summary>
    /// 将Int32转化成字节数组加入字节流
    /// </summary>
    /// <param name="num">要转化的Int32</param>
    public void AddInt(int num)
    {
        byteList.Add((byte)(num & filter));
        byteList.Add((byte)((num >> 8) & filter));
        byteList.Add((byte)((num >> 16) & filter));
        byteList.Add((byte)((num >> 24) & filter));
    }

    /// <summary>
    /// 将字节数组转化成Int32
    /// </summary>
    /// <param name="index">索引起点</param>
    /// <param name="end">为下一个转换提供索引起点</param>
    /// <returns></returns>
    public int GetInt()
    {
        if (bytes == null)
            return 0;
        if (bytes.Length < index + sizeof(int))
            return 0;

        return (bytes[index++]) + (bytes[index++] << 8) + (bytes[index++] << 16) + (bytes[index++] << 24);
    }

    #endregion

    #region 添加获取浮点数

    /// <summary>
    /// 将float转化成字节数组加入字节流
    /// </summary>
    /// <param name="num">要转化的float</param>
    public void AddFloat(float num)
    {
        byte[] numBytes = BitConverter.GetBytes(num);
        byteList.AddRange(numBytes);
    }

    /// <summary>
    /// 将字节数组转化成float
    /// </summary>
    /// <param name="index">索引起点</param>
    /// <param name="end">为下一个转换提供索引起点</param>
    /// <returns></returns>
    public float GetFloat()
    {
        if (bytes == null)
            return -1;
        if (bytes.Length < index + sizeof(float))
            return -1;
        float value = BitConverter.ToSingle(bytes, index);
        index = index + sizeof(float);
        return value;
    }

    #endregion

    #region 添加获取Vector3

    public void AddVector3(Vector3 v)
    {
        AddFloat(v.x);
        AddFloat(v.y);
        AddFloat(v.z);
    }

    public Vector3 GetVector3()
    {
        float x = GetFloat();
        float y = GetFloat();
        float z = GetFloat();
        return new Vector3(x, y, z);
    }

    #endregion

    /// <summary>
    /// 添加帧同步
    /// </summary>
    public void AddFrameSynInfo(int id,Transform tran)
    {
        AddInt(id);
        AddVector3(tran.position);
        AddVector3(tran.localEulerAngles);
    }
}