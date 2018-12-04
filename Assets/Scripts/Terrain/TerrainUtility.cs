using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Terrain工具
/// terrainData.GetHeights和SetHeights的参数都是 值域为[0,1]的比例值
/// </summary>
public static class TerrainUtility
{
    /** 
     * Terrain的HeightMap坐标原点在左下角
     *   y
     *   ↑
     *   0 → x
     */

    private static Dictionary<string, float[,]> brushDic = new Dictionary<string, float[,]>();

    /// <summary>
    /// 用于记录要修改的Terrain目标数据，修改后统一刷新
    /// </summary>
    private static Dictionary<Terrain, float[,]> terrainDic = new Dictionary<Terrain, float[,]>();
    /// <summary>
    /// 记录路面等铺设时自动修改前的Terrain数据
    /// </summary>
    private static Dictionary<Terrain, Stack<TerrainCmdData>> oldTerrainData = new Dictionary<Terrain, Stack<TerrainCmdData>>();

    /// <summary>
    /// 静态构造函数
    /// </summary>
    static TerrainUtility()
    {
        InitBrushs();
        InitTreePrototype();
    }

    #region 高度图相关

    /// <summary>
    /// 初始化笔刷
    /// </summary>
    private static void InitBrushs()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("TerrainBrush");

        for (int i = 0, length = textures.Length; i < length; i++)
        {
            // 获取图片颜色ARGB信息
            Color[] colors = textures[i].GetPixels();
            // terrainData.GetHeightMap得到的二维数组是[y,x]
            float[,] alphas = new float[textures[i].height, textures[i].width];

            for (int j = 0, length0 = textures[i].height, index = 0; j < length0; j++)
            {
                for (int k = 0, length1 = textures[i].width; k < length1; k++)
                {
                    alphas[j, k] = colors[index].a;
                    index++;
                }
            }

            brushDic.Add(textures[i].name, alphas);
        }
    }

    /// <summary>
    /// 返回Terrain上某一点的HeightMap索引。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <returns>该点在HeightMap中的位置索引</returns>
    public static int[] GetHeightmapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.heightmapWidth);
        int z = (int)((point.z - terrain.GetPosition().z) / length * tData.heightmapHeight);

        return new int[2] { x, z };
    }

    /// <summary>
    /// 返回地图Index对应的世界坐标系位置
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public static Vector3 GetIndexWorldPoint(Terrain terrain, int x, int z)
    {
        TerrainData data = terrain.terrainData;
        float _x = data.size.x / (data.heightmapWidth - 1) * x;
        float _z = data.size.z / (data.heightmapHeight - 1) * z;

        float _y = GetPointHeight(terrain, new Vector3(_x, 0, _z));
        return new Vector3(_x, _y, _z) + terrain.GetPosition();
    }

    /// <summary>
    /// 返回GameObject在Terrain上的相对（于Terrain的）位置。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="go">GameObject</param>
    /// <returns>相对位置</returns>
    public static Vector3 GetRelativePosition(Terrain terrain, GameObject go)
    {
        return go.transform.position - terrain.GetPosition();
    }

    /// <summary>
    /// 返回Terrain上指定点在世界坐标系下的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <param name="vertex">true: 获取最近顶点高度  false: 获取实际高度</param>
    /// <returns>点在世界坐标系下的高度</returns>
    public static float GetPointHeight(Terrain terrain, Vector3 point, bool vertex = false)
    {
        // 对于水平面上的点来说，vertex参数没有影响
        if (vertex)
        {
            // GetHeight得到的是离点最近的顶点的高度
            int[] index = GetHeightmapIndex(terrain, point);
            return terrain.terrainData.GetHeight(index[0], index[1]);
        }
        else
        {
            // SampleHeight得到的是点在斜面上的实际高度
            return terrain.SampleHeight(point);
        }
    }

    /// <summary>
    /// 返回Terrain的HeightMap，这是一个 height*width 大小的二维数组，并且值介于 [0.0f,1.0f] 之间。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="xBase">检索HeightMap时的X索引起点</param>
    /// <param name="yBase">检索HeightMap时的Y索引起点</param>
    /// <param name="width">在X轴上的检索长度</param>
    /// <param name="height">在Y轴上的检索长度</param>
    /// <returns></returns>
    public static float[,] GetHeightMap(Terrain terrain, int xBase = 0, int yBase = 0, int width = 0, int height = 0)
    {
        if (xBase + yBase + width + height == 0)
        {
            width = terrain.terrainData.heightmapWidth;
            height = terrain.terrainData.heightmapHeight;
        }

        return terrain.terrainData.GetHeights(xBase, yBase, width, height);
    }

    /// <summary>
    /// 升高Terrain上某点的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的点</param>
    /// <param name="opacity">升高的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经高于笔刷中心点时是否同时提高其他点的高度</param>
    public static void Rise(Terrain terrain, Vector3 point, float opacity, int size, bool amass = true)
    {
        int[] index = GetHeightmapIndex(terrain, point);
        Rise(terrain, index, opacity, size, amass);
    }

    /// <summary>
    /// 升高Terrain上的某点。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="index">HeightMap索引</param>
    /// <param name="opacity">升高的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经高于笔刷中心点时是否同时提高其他点的高度</param>
    public static void Rise(Terrain terrain, int[] index, float opacity, int size, bool amass = true)
    {
        TerrainData tData = terrain.terrainData;

        int bound = size / 2;  // 修改半径

        // 获取起始Index以及在x，z轴上分别要修改的大小
        int xBase = index[0] - bound >= 0 ? index[0] - bound : 0;
        int yBase = index[1] - bound >= 0 ? index[1] - bound : 0;
        int width = xBase + size <= tData.heightmapWidth ? size : tData.heightmapWidth - xBase;
        int height = yBase + size <= tData.heightmapHeight ? size : tData.heightmapHeight - yBase;

        float[,] heights = tData.GetHeights(xBase, yBase, width, height);
        float initHeight = tData.GetHeight(index[0], index[1]) / tData.size.y;
        float deltaHeight = opacity / tData.size.y;  // 计算得到要改变的高度比列

        // 得到的heights数组维度是[height,width]，索引为[y,x]
        ExpandBrush(heights, deltaHeight, initHeight, height, width, bound, amass);
        tData.SetHeights(xBase, yBase, heights);
    }

    /// <summary>
    /// 降低Terrain上某点的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的点</param>
    /// <param name="opacity">降低的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经低于笔刷中心点时是否同时降低其他点的高度</param>
    public static void Sink(Terrain terrain, Vector3 point, float opacity, int size, bool amass = true)
    {
        int[] index = GetHeightmapIndex(terrain, point);
        Sink(terrain, index, opacity, size, amass);
    }

    /// <summary>
    /// 降低Terrain上某点的高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="index">HeightMap索引</param>
    /// <param name="opacity">降低的高度</param>
    /// <param name="size">笔刷大小</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经低于笔刷中心点时是否同时降低其他点的高度</param>
    public static void Sink(Terrain terrain, int[] index, float opacity, int size, bool amass = true)
    {
        TerrainData tData = terrain.terrainData;

        int bound = size / 2;
        int xBase = index[0] - bound >= 0 ? index[0] - bound : 0;
        int yBase = index[1] - bound >= 0 ? index[1] - bound : 0;
        int width = xBase + size <= tData.heightmapWidth ? size : tData.heightmapWidth - xBase;
        int height = yBase + size <= tData.heightmapHeight ? size : tData.heightmapHeight - yBase;

        float[,] heights = tData.GetHeights(xBase, yBase, width, height);
        float initHeight = tData.GetHeight(index[0], index[1]) / tData.size.y;
        float deltaHeight = -opacity / tData.size.y;  // 注意负号

        // 得到的heights数组维度是[height,width]，索引为[y,x]
        ExpandBrush(heights, deltaHeight, initHeight, height, width, bound, amass);
        tData.SetHeights(xBase, yBase, heights);
    }

    /// <summary>
    /// 根据笔刷四角的高度来平滑Terrain，该方法不会改变笔刷边界处的Terrain高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的点</param>
    /// <param name="opacity">平滑灵敏度，值介于 [0.05,1] 之间</param>
    /// <param name="size">笔刷大小</param>
    public static void Smooth(Terrain terrain, Vector3 point, float opacity, int size)
    {
        int[] index = GetHeightmapIndex(terrain, point);
        Smooth(terrain, index, opacity, size);
    }

    /// <summary>
    /// 根据笔刷四角的高度来平滑Terrain，该方法不会改变笔刷边界处的Terrain高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="index">HeightMap索引</param>
    /// <param name="opacity">平滑灵敏度，值介于 [0.05,1] 之间</param>
    /// <param name="size">笔刷大小</param>
    public static void Smooth(Terrain terrain, int[] index, float opacity, int size)
    {
        TerrainData tData = terrain.terrainData;
        if (opacity > 1 || opacity <= 0)
        {
            opacity = Mathf.Clamp(opacity, 0.05f, 1);
            Debug.LogError("Smooth方法中的opacity参数的值应该介于 [0.05,1] 之间，强制将其设为：" + opacity);
        }

        // 取出笔刷范围内的HeightMap数据数组
        int bound = size / 2;
        int xBase = index[0] - bound >= 0 ? index[0] - bound : 0;
        int yBase = index[1] - bound >= 0 ? index[1] - bound : 0;
        int width = xBase + size <= tData.heightmapWidth ? size : tData.heightmapWidth - xBase;
        int height = yBase + size <= tData.heightmapHeight ? size : tData.heightmapHeight - yBase;
        float[,] heights = tData.GetHeights(xBase, yBase, width, height);

        // 利用笔刷4角的高度来计算平均高度
        float avgHeight = (heights[0, 0] + heights[0, width - 1] + heights[height - 1, 0] + heights[height - 1, width - 1]) / 4;
        Vector2 center = new Vector2((float)(height - 1) / 2, (float)(width - 1) / 2);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
#if false
                // 点到矩阵中心点的距离
                float toCenter = Vector2.Distance(center, new Vector2(i, j));
                float diff = avgHeight - heights[i, j];

                // 判断点在4个三角形区块上的位置
                // 利用相似三角形求出点到矩阵中心点与该点连线的延长线与边界交点的距离
                float d = 0;
                if (i == height / 2 && j == width / 2)  // 中心点
                {
                    d = 1;
                    toCenter = 0;
                }
                else if (i >= j && i <= size - j)  // 左三角区
                {
                    // j/((float)width / 2) = d/(d+toCenter)，求出距离d，其他同理
                    d = toCenter * j / ((float)width / 2 - j);
                }
                else if (i <= j && i <= size - j)  // 上三角区
                {
                    d = toCenter * i / ((float)height / 2 - i);
                }
                else if (i <= j && i >= size - j)  // 右三角区
                {
                    d = toCenter * (size - j) / ((float)width / 2 - (size - j));
                }
                else if (i >= j && i >= size - j)  // 下三角区
                {
                    d = toCenter * (size - i) / ((float)height / 2 - (size - i));
                }

                // 进行平滑时对点进行升降的比例
                float ratio = d / (d + toCenter);
                heights[i, j] += diff * ratio * opacity;
#endif
                // 圆外的点不做处理
                if ((i - bound) * (i - bound) + (j - bound) * (j - bound) >= bound * bound)
                    continue;

                float h = Smooth(terrain.terrainData, xBase + i, yBase + j);
                heights[i, j] = h;
            }
        }

        tData.SetHeights(xBase, yBase, heights);
    }

    /// <summary>
    /// 通过给定index周围区域的高度得到平均高度
    /// </summary>
    /// <param name="terrainData"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public static float Smooth(TerrainData terrainData, int x, int y)
    {
        float h = 0.0F;
        float normalizeScale = 1.0F / terrainData.size.y;
        h += terrainData.GetHeight(x, y) * normalizeScale;
        h += terrainData.GetHeight(x + 1, y) * normalizeScale;
        h += terrainData.GetHeight(x - 1, y) * normalizeScale;
        h += terrainData.GetHeight(x + 1, y + 1) * normalizeScale * 0.75F;
        h += terrainData.GetHeight(x - 1, y + 1) * normalizeScale * 0.75F;
        h += terrainData.GetHeight(x + 1, y - 1) * normalizeScale * 0.75F;
        h += terrainData.GetHeight(x - 1, y - 1) * normalizeScale * 0.75F;
        h += terrainData.GetHeight(x, y + 1) * normalizeScale;
        h += terrainData.GetHeight(x, y - 1) * normalizeScale;
        h /= 8.0F;
        return h;
    }

    /// <summary>
    /// 压平Terrain并提升到指定高度。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="height">高度</param>
    public static void Flatten(Terrain terrain, float height)
    {
        TerrainData tData = terrain.terrainData;
        float scaledHeight = height / tData.size.y;

        float[,] heights = new float[tData.heightmapWidth, tData.heightmapHeight];
        for (int i = 0; i < tData.heightmapWidth; i++)
        {
            for (int j = 0; j < tData.heightmapHeight; j++)
            {
                heights[i, j] = scaledHeight;
            }
        }

        tData.SetHeights(0, 0, heights);
    }

    /// <summary>
    /// 设置Terrain的HeightMap。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="heights">HeightMap</param>
    /// <param name="xBase">X起点</param>
    /// <param name="yBase">Y起点</param>
    public static void SetHeights(Terrain terrain, float[,] heights, int xBase = 0, int yBase = 0)
    {
        terrain.terrainData.SetHeights(xBase, yBase, heights);
    }

    /// <summary>
    /// 保存修改后的Terrain数据
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="path">保存路径</param>
    public static void SaveHeightmapData(Terrain terrain/*, string path*/)
    {

    }

    /// <summary>
    /// 扩大笔刷作用范围。
    /// </summary>
    /// <param name="heights">HeightMap</param>
    /// <param name="deltaHeight">高度变化量[-1,1]</param>
    /// <param name="initHeight">笔刷中心点的初始高度</param>
    /// <param name="row">HeightMap行数</param>
    /// <param name="column">HeightMap列数</param>
    /// <param name="amass">当笔刷范围内其他点的高度已经高于笔刷中心点时是否同时提高其他点的高度</param>
    private static void ExpandBrush(float[,] heights, float deltaHeight, float initHeight, int row, int column, int radius, bool amass)
    {
        // 高度限制
        float limit = initHeight + deltaHeight;

        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < column; j++)
            {
                // 圆外的点不做处理
                float rPow = (i - radius) * (i - radius) + (j - radius) * (j - radius);
                if (rPow >= radius * radius)
                    continue;

                float differ = 1 - rPow / (radius * radius);

                if (amass) { heights[i, j] += differ * deltaHeight; }
                else  // 不累加高度时
                {
                    if (deltaHeight > 0)  // 升高地形
                    {
                        heights[i, j] = heights[i, j] >= limit ? heights[i, j] : heights[i, j] + differ * deltaHeight;
                    }
                    else  // 降低地形
                    {
                        heights[i, j] = heights[i, j] <= limit ? heights[i, j] : heights[i, j] + differ * deltaHeight;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 改变指定位置周围四个顶点的高度
    /// </summary>
    /// <param name="terrain">目标地形块</param>
    /// <param name="pos">目标位置</param>
    /// <param name="heights">目标高度</param>
    [Obsolete("已弃用，请使用ChangeHeight和Refresh搭配")]
    public static void ChangeHeights(Terrain terrain, Vector3[] poses, float[] heights)
    {
        TerrainData terrainData = terrain.terrainData;
        float[,] totalHeightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight); // 获取整块地图的高度数据

        // 循环处理每个点
        for (int p = 0; p < poses.Length; p++)
        {
            Vector3 pos = poses[p];

            int[] index = GetHeightmapIndex(terrain, pos);
            heights[p] = heights[p] / terrainData.size.y;    // 获取目标高度的相对比

            // 改变一个方形块的高度
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    int zIndex = Mathf.Clamp(index[1] + j, 0, terrainData.heightmapWidth);
                    int xIndex = Mathf.Clamp(index[0] + i, 0, terrainData.heightmapHeight);
                    totalHeightMap[zIndex, xIndex] = heights[p];
                }
            }
        }

        terrainData.SetHeights(0, 0, totalHeightMap);
    }
    public static void ChangeHeight(Terrain terrain, Vector3 pos, float height)
    {
        if (!terrainDic.ContainsKey(terrain))
        {
            terrainDic.Add(terrain, GetHeightMap(terrain));
        }
        TerrainData terrainData = terrain.terrainData;
        float[,] totalHeightMap = terrainDic[terrain];  // 获取整块地图的高度数据

        int[] index = GetHeightmapIndex(terrain, pos);
        height = height / terrainData.size.y;    // 获取目标高度的相对比

        // 改变一个方形块的高度
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int zIndex = Mathf.Clamp(index[1] + j, 0, terrainData.heightmapWidth);
                int xIndex = Mathf.Clamp(index[0] + i, 0, terrainData.heightmapHeight);
                totalHeightMap[zIndex, xIndex] = height;
            }
        }
    }

    /// <summary>
    /// 改变点周围一圈点的高度
    /// </summary>
    /// <param name="terrain">目标地图块</param>
    /// <param name="poses">目标位置</param>
    /// <param name="heights">目标高度</param>
    /// <param name="radius">圆半径</param>
    [Obsolete("已弃用，ChangeCircleHeight和Refresh搭配")]
    public static void ChangeCircleHeights(Terrain terrain, Vector3[] poses, float[] heights, float radius)
    {
        TerrainData terrainData = terrain.terrainData;
        float[,] totalHeightMap = terrainData.GetHeights(0, 0, terrainData.heightmapWidth, terrainData.heightmapHeight); // 获取整块地图的高度数据

        // 半径为radius的圆在x,z上的所占的索引
        int radiusX = (int)(radius / (terrainData.size.x / terrainData.heightmapWidth)) + 1;
        int radiusZ = (int)(radius / (terrainData.size.z / terrainData.heightmapHeight)) + 1;

        // 循环处理每个点
        for (int p = 0; p < poses.Length; p++)
        {
            Vector3 pos = poses[p];

            int[] index = GetHeightmapIndex(terrain, pos);
            heights[p] = heights[p] / terrainData.size.y;     // 获取目标高度的相对比

            // 改变一个圆心区域的高度
            for (int i = -radiusX; i < radiusX; i++)
            {
                for (int j = -radiusZ; j < radiusZ; j++)
                {
                    // 圆外的点不做处理
                    float rPow = i * i + j * j;
                    if (rPow >= Mathf.Pow((radiusX + radiusZ) / 2, 2))
                        continue;

                    int zIndex = Mathf.Clamp(index[1] + j, 0, terrainData.heightmapWidth);
                    int xIndex = Mathf.Clamp(index[0] + i, 0, terrainData.heightmapHeight);
                    totalHeightMap[zIndex, xIndex] = heights[p];
                }
            }
        }
        terrainData.SetHeights(0, 0, totalHeightMap);
    }
    public static void ChangeCircleHeight(Terrain terrain, Vector3 pos, float height, float radius)
    {
        if (!terrainDic.ContainsKey(terrain))
        {
            terrainDic.Add(terrain, GetHeightMap(terrain));
        }
        TerrainData terrainData = terrain.terrainData;
        float[,] totalHeightMap = terrainDic[terrain];  // 获取整块地图的高度数据

        // 半径为radius的圆在x,z上的所占的索引
        int radiusX = (int)(radius / (terrainData.size.x / terrainData.heightmapWidth)) + 1;
        int radiusZ = (int)(radius / (terrainData.size.z / terrainData.heightmapHeight)) + 1;

        int[] index = GetHeightmapIndex(terrain, pos);
        height = height / terrainData.size.y;     // 获取目标高度的相对比

        // 改变一个圆心区域的高度
        for (int i = -radiusX; i < radiusX; i++)
        {
            for (int j = -radiusZ; j < radiusZ; j++)
            {
                // 圆外的点不做处理
                float rPow = i * i + j * j;
                if (rPow >= Mathf.Pow((radiusX + radiusZ) / 2, 2))
                    continue;

                int zIndex = Mathf.Clamp(index[1] + j, 0, terrainData.heightmapWidth);
                int xIndex = Mathf.Clamp(index[0] + i, 0, terrainData.heightmapHeight);
                totalHeightMap[zIndex, xIndex] = height;
            }
        }
    }

    public static void ChangeHeightWithBrush(Terrain terrain)
    {

    }

    #endregion

    #region 树木

    /// <summary>
    /// 初始化树木原型组
    /// </summary>
    private static void InitTreePrototype()
    {
        GameObject[] objs = Resources.LoadAll<GameObject>("Tree");
        TreePrototype[] trees = new TreePrototype[objs.Length];
        for (int i = 0, length = objs.Length; i < length; i++)
        {
            trees[i] = new TreePrototype();
            trees[i].prefab = objs[i];
        }
        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.treePrototypes = trees;
        }
    }

    /// <summary>
    /// 创建树木
    /// </summary>
    /// <param name="terrain"></param>
    /// <param name="pos"></param>
    public static void CreatTree(Terrain terrain, Vector3 pos, int count, int radius)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 relativePosition;
        Vector3 position;

        for (int i = 0; i < count; i++)
        {
            // 获取世界坐标系的位置和相对位置
            position = pos + new Vector3(UnityEngine.Random.Range(-radius, radius), 0, UnityEngine.Random.Range(-radius, radius));
            relativePosition = position - terrain.GetPosition();

            if (Mathf.Pow(pos.x - position.x, 2) + Mathf.Pow(pos.z - position.z, 2) > radius * radius)
            {
                i--; // 没有创建的数不计入
                continue;
            }

            // 设置新添加的树的参数
            TreeInstance instance = new TreeInstance();
            instance.prototypeIndex = 0;
            instance.color = Color.white;
            instance.lightmapColor = Color.white;
            instance.widthScale = 1;
            instance.heightScale = 1;

            Vector3 p = new Vector3(relativePosition.x / terrainData.size.x, 0, relativePosition.z / terrainData.size.z);
            if (p.x > 1 || p.z > 1)
            {
                if (p.x > 1)
                    p.x = p.x - 1;
                if (p.z > 1)
                    p.z = p.z - 1;
                instance.position = p;
                GetTerrain(position)?.AddTreeInstance(instance);
            }
            else if (p.x < 0 || p.z < 0)
            {
                if (p.x < 0)
                    p.x = p.x + 1;
                if (p.z < 0)
                    p.z = p.z + 1;
                instance.position = p;
                GetTerrain(position)?.AddTreeInstance(instance);
            }
            else
            {
                instance.position = p;
                terrain.AddTreeInstance(instance);
            }
        }
    }

    #endregion

    #region 细节纹理 草

    #endregion

    #region 修改与恢复

    /// <summary>
    /// 添加一个记录点
    /// </summary>
    /// <param name="terrain"></param>
    public static void AddOldData(Terrain terrain)
    {
        if (!oldTerrainData.ContainsKey(terrain))
        {
            oldTerrainData.Add(terrain, new Stack<TerrainCmdData>());
        }
        oldTerrainData[terrain].Push(new TerrainCmdData(GetHeightMap(terrain)));
    }

    /// <summary>
    /// 恢复一次数据
    /// </summary>
    /// <param name="terrain"></param>
    public static void Recover(Terrain terrain)
    {
        Stack<TerrainCmdData> heightsStack = oldTerrainData.GetValue(terrain);
        if (heightsStack != null && heightsStack.Count > 0)
        {
            terrain.terrainData.SetHeights(0, 0, heightsStack.Pop().heights);
            if (heightsStack.Count == 0)
            {
                oldTerrainData.Remove(terrain);
            }
        }
    }

    #endregion

    #region  工具

    /// <summary>
    /// 刷新地图
    /// </summary>
    public static void Refresh()
    {
        foreach (var item in terrainDic)
        {
            item.Key.terrainData.SetHeights(0, 0, item.Value);
        }
        terrainDic.Clear();
    }

    /// <summary>
    /// 获取当前位置所对应的地图块
    /// </summary>
    public static Terrain GetTerrain(Vector3 pos)
    {
        RaycastHit hitInfo;
        Physics.Raycast(pos + Vector3.up * 1000, Vector3.down, out hitInfo, float.MaxValue, LayerMask.GetMask("Terrain"));
        return hitInfo.collider?.GetComponent<Terrain>();
    }

    #endregion


    struct TerrainCmdData
    {
        public float[,] heights;

        public TerrainCmdData(float[,] _heights)
        {
            heights = _heights;
        }
    }
}