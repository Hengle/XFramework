// ==========================================
// 描述： 
// 作者： HAk
// 时间： 2018-11-12 13:38:45
// 版本： V 1.0
// ==========================================
using System;
using UnityEngine;

namespace XDEDZL
{
    /// <summary>
    /// 序列化的Vector3，用以解决Vector3不能序列化的问题
    /// </summary>
    [Serializable]
    public struct Vector3Serializer
    {
        public float x;
        public float y;
        public float z;

        public Vector3Serializer(Vector3 v3)
        {
            x = v3.x;
            y = v3.y;
            z = v3.z;
        }

        public Vector3 V3
        {
            get
            {
                return new Vector3(x, y, z);
            }
        }

        /// <summary>
        /// 将一个非序列化的V3数组转为序列化的V3数组
        /// </summary>
        public static Vector3Serializer[] Serializers(Vector3[] V3s)
        {
            Vector3Serializer[] VSer = new Vector3Serializer[V3s.Length];
            for (int i = 0, length = V3s.Length; i < length; i++)
            {
                VSer[i] = V3s[i].Serializer();
            }
            return VSer;
        }

        /// <summary>
        /// 将一个序列化的V3数组转为非序列化的V3数组
        /// </summary>
        public static Vector3[] DeSerializers(Vector3Serializer[] VSers)
        {
            Vector3[] V3s = new Vector3[VSers.Length];
            for (int i = 0, length = V3s.Length; i < length; i++)
            {
                V3s[i] = VSers[i].V3;
            }
            return V3s;
        }
    }
}