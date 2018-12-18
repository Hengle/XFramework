// ==========================================
// 描述： 
// 作者： HAK
// 时间： 2018-10-24 16:26:10
// 版本： V 1.0
// ==========================================
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 使用工具类
/// </summary>
public static class Utility
{

    public delegate T Del<T>(T a);

    private static RaycastHit hit;
    /// <summary>
    /// 发射射线并范围RaycastInfo
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


    public static async Task<float[,]> ZoomBilinearInterpAsync(float[,] array_In, int newWidth, int newHeight)
    {
        int originalHeight = array_In.GetLength(0);
        int originalWidth = array_In.GetLength(1);

        float scaleX = ((float)newHeight) / ((float)originalHeight);
        float scaleY = ((float)newWidth) / ((float)originalWidth);

        float[,] array_Out = new float[newHeight, newWidth];
        float u = 0, v = 0, x = 0, y = 0;
        int m = 0, n = 0;
        int i, j;
        await Task.Run(async () =>
        {
            for (i = 0; i < newHeight; ++i)
            {
                await Task.Run(() =>
                {
                    for (j = 0; j < newWidth; ++j)
                    {
                        x = i / scaleX;
                        y = j / scaleY;

                        m = Mathf.FloorToInt(x);
                        n = Mathf.FloorToInt(y);

                        u = x - m;
                        v = y - n;

                        if (m < originalHeight - 1 && n < originalWidth - 1)
                        {

                            array_Out[i, j] = (1.0f - v) * (1.0f - u) * array_In[m, n] + (1 - v) * u * array_In[m, n + 1]
                                                   + v * (1.0f - u) * array_In[m + 1, n] + v * u * array_In[m + 1, n + 1];
                        }
                        else
                        {
                            array_Out[i, j] = array_In[m, n];
                        }
                    }

                });
                //Debug.Log(string.Format("i:{0}", i));
            }
        }
        );
        Debug.Log("新矩阵计算终了");
        return array_Out;
    }

    public static async Task<float[,]> BilinearInterp(float[,] array, int length_0, int length_1)
    {
        float[,] _out = new float[length_0, length_1];
        int original_0 = array.GetLength(0);
        int original_1 = array.GetLength(1);

        float ReScale_0 = original_0 / ((float)length_0);  // 倍数的倒数
        float ReScale_1 = original_1 / ((float)length_1);

        float index_0;
        float index_1;
        int inde_0;
        int inde_1;
        float s_leftUp;
        float s_rightUp;
        float s_rightDown;
        float s_leftDown;

        await Task.Run(async () =>
        {
            for (int i = 0; i < length_0; i++)
            {
                await Task.Run(() =>
                {
                    for (int j = 0; j < length_1; j++)
                    {
                        index_0 = i * ReScale_0;
                        index_1 = j * ReScale_1;
                        inde_0 = Mathf.FloorToInt(index_0);
                        inde_1 = Mathf.FloorToInt(index_1);
                        s_leftUp = (index_0 - inde_0) * (index_1 - inde_1);
                        s_rightUp = (inde_0 + 1 - index_0) * (index_1 - inde_1);
                        s_rightDown = (inde_0 + 1 - index_0) * (inde_1 + 1 - index_1);
                        s_leftDown = (index_0 - inde_0) * (inde_1 + 1 - index_1);
                        _out[i, j] = array[inde_0, inde_1] * s_rightDown + array[inde_0 + 1, inde_1] * s_leftDown + array[inde_0 + 1, inde_1 + 1] * s_leftUp + array[inde_0, inde_1 + 1] * s_rightUp;
                    }
                });
            }
        });

        return _out;
    }

    /// <summary>
    /// 对二维数组做高斯模糊
    /// </summary>
    /// <param name="array">要处理的数组</param>
    /// <param name="dev"></param>
    /// <param name="r">高斯核扩展半径</param>
    /// <param name="isCircle">改变形状是否是圆</param>
    public static void GaussianBlur(float[,] array, float dev, int r = 1, bool isCircle = true)
    {
        // 构造半径为1的高斯核
        int length = r * 2 + 1;
        float[,] gaussianCore = new float[length, length];
        float k = 1 / (2 * Mathf.PI * dev * dev);
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                float pow = -((j - r) * (j - r) + (i - r) * (i - r)) / (2 * dev * dev);
                gaussianCore[i, j] = k * Mathf.Pow(2.71828f, pow);
            }
        }

        // 使权值和为1
        float sum = 0;
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                sum += gaussianCore[i, j];
            }
        }
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < length; j++)
            {
                gaussianCore[i, j] /= sum;
            }
        }

        // 对二维数组进行高斯模糊处理
        int circleR = array.GetLength(0) / 2;
        for (int i = r, length_0 = array.GetLength(0) - r; i < length_0; i++)
        {
            for (int j = r, length_1 = array.GetLength(1) - r; j < length_1; j++)
            {
                if (isCircle && (i - circleR) * (i - circleR) + (j - circleR) * (j - circleR) > (circleR - r) * (circleR - r))
                    continue;

                // 用高斯核处理一个值
                float value = 0;
                for (int u = 0; u < length; u++)
                {
                    for (int v = 0; v < length; v++)
                    {
                        if ((i + u - r) >= array.GetLength(0) || (i + u - r) < 0 || (j + v - r) >= array.GetLength(1) || (j + v - r) < 0)
                            Debug.LogError("滴嘟滴嘟的报错");
                        else
                            value += gaussianCore[u, v] * array[i + u - r, j + v - r];
                    }
                }
                array[i, j] = value;
            }
        }
    }
}