using UnityEngine;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
/** 
* Terrain的HeightMap坐标原点在左下角
*   y
*   ↑
*   0 → x
*/
/// <summary>
/// Terrain工具
/// terrainData.GetHeights和SetHeights的参数都是 值域为[0,1]的比例值
/// </summary>
public static class TerrainUtility
{
    /// <summary>
    /// 用于修改高度的单位高度
    /// </summary>
    private static float deltaHeight;
    private static Vector3 terrainSize;
    private static int heightMapRes;
    /// <summary>
    /// 用于记录要修改的Terrain目标数据，修改后统一刷新
    /// </summary>
    private static Dictionary<int, float[,]> brushDic = new Dictionary<int, float[,]>();
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
        deltaHeight = 1 / Terrain.activeTerrain.terrainData.size.y;
        terrainSize = Terrain.activeTerrain.terrainData.size;
        heightMapRes = Terrain.activeTerrain.terrainData.heightmapResolution;
        InitBrushs();
        InitTreePrototype();
        InitDetailPrototype();
        InitTextures();
    }

    #region 高度图相关

    /// <summary>
    /// 初始化笔刷
    /// </summary>
    private static void InitBrushs()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Terrain/Brushs");

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
            brushDic.Add(i, alphas);
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
    /// 返回Terrain的HeightMap的一部分
    /// 场景中有多块地图时不要直接调用terrainData.getheights
    /// 这个方法会解决跨多块地形的问题
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="xBase">检索HeightMap时的X索引起点</param>
    /// <param name="yBase">检索HeightMap时的Y索引起点</param>
    /// <param name="width">在X轴上的检索长度</param>
    /// <param name="height">在Y轴上的检索长度</param>
    /// <returns></returns>
    public static float[,] GetHeightMap(Terrain terrain, int xBase = 0, int yBase = 0, int width = 0, int height = 0)
    {
        // 如果后四个均为默认参数，则直接返回当前地形的整个高度图
        if (xBase + yBase + width + height == 0)
        {
            width = terrain.terrainData.heightmapWidth;
            height = terrain.terrainData.heightmapHeight;
            return terrain.terrainData.GetHeights(xBase, yBase, width, height);
        }

        TerrainData terrainData = terrain.terrainData;
        int differX = xBase + width - (terrainData.heightmapResolution - 1);   // 右溢出量级
        int differY = yBase + height - (terrainData.heightmapResolution - 1);  // 上溢出量级

        float[,] ret;
        if (differX <= 0 && differY <= 0)  // 无溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width, height);
        }
        else if (differX > 0 && differY <= 0) // 右边溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width - differX, height);
            float[,] right = terrain.Right()?.terrainData.GetHeights(0, yBase, differX, height);
            if (right != null)
                ret = ret.Concat0(right);
        }
        else if (differX <= 0 && differY > 0)  // 上边溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width, height - differY);
            float[,] up = terrain.Up()?.terrainData.GetHeights(xBase, 0, width, differY);
            if (up != null)
                ret = ret.Concat1(up);
        }
        else // 上右均溢出
        {
            ret = terrain.terrainData.GetHeights(xBase, yBase, width - differX, height - differY);
            float[,] right = terrain.Right()?.terrainData.GetHeights(0, yBase, differX, height - differY);
            float[,] up = terrain.Up()?.terrainData.GetHeights(xBase, 0, width - differX, differY);
            float[,] upRight = terrain.Right()?.Up()?.terrainData.GetHeights(0, 0, differX, differY);

            if (right != null)
                ret = ret.Concat0(right);
            if (up != null)
                ret = ret.Concat1(up.Concat0(upRight));
        }

        return ret;
    }

    /// <summary>
    /// 初始化地形高度图编辑所需要的参数
    /// 后四个参数需要在调用前定义
    /// </summary>
    /// <param name="center">目标中心</param>
    /// <param name="radius">半径</param>
    /// <param name="mapIndex">起始修改点在高度图上的索引</param>
    /// <param name="heightMap">要修改的高度二维数组</param>
    /// <param name="mapRadius">修改半径对应的索引半径</param>
    /// <param name="limit">限制高度</param>
    /// <returns></returns>
    public static Terrain InitHMArg(Vector3 center, float radius, ref int[] mapIndex, ref float[,] heightMap, ref int mapRadius, ref float limit)
    {
        Vector3 leftDown = new Vector3(center.x - radius, 0, center.z - radius);
        // 左下方Terrain
        Terrain terrain = Utility.SendRayDown(leftDown, LayerMask.GetMask("Terrain")).collider?.GetComponent<Terrain>();
        // 左下至少有一个方向没有Terrain
        if (terrain != null)
        {
            // 获取相关参数
            mapRadius = (int)(terrain.terrainData.heightmapResolution / terrain.terrainData.size.x * radius);
            mapIndex = GetHeightmapIndex(terrain, leftDown);
            heightMap = GetHeightMap(terrain, mapIndex[0], mapIndex[1], 2 * mapRadius, 2 * mapRadius);
            limit = heightMap[mapRadius, mapRadius];
        }
        return terrain;
    }

    /// <summary>
    /// 改变地形高度
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="opacity"></param>
    /// <param name="amass"></param>
    public static void ChangeHeight(Vector3 center, float radius, float opacity, bool isRise = true, bool amass = true)
    {
        int mapRadius = 0;
        int[] mapIndex = null;
        float[,] heightMap = null;
        float limit = 0;
        Terrain terrain = InitHMArg(center, radius, ref mapIndex, ref heightMap, ref mapRadius, ref limit);
        if (terrain == null) return;

        if (!isRise) opacity = -opacity;

        // 修改高度图
        for (int i = 0, length_0 = heightMap.GetLength(0); i < length_0; i++)
        {
            for (int j = 0, length_1 = heightMap.GetLength(1); j < length_1; j++)
            {
                // 限制范围为一个圆
                float rPow = (i - mapRadius) * (i - mapRadius) + (j - mapRadius) * (j - mapRadius);
                if (rPow > mapRadius * mapRadius)
                    continue;

                float differ = 1 - rPow / (mapRadius * mapRadius);
                if (amass)
                {
                    heightMap[i, j] += differ * deltaHeight * opacity;
                }
                else if (isRise)
                {
                    heightMap[i, j] = heightMap[i, j] >= limit ? heightMap[i, j] : heightMap[i, j] + differ * deltaHeight * opacity;
                }
                else
                {
                    heightMap[i, j] = heightMap[i, j] <= limit ? heightMap[i, j] : heightMap[i, j] + differ * deltaHeight * opacity;
                }
            }
        }
        // 重新设置高度图
        SetHeightMap(terrain, heightMap, mapIndex[0], mapIndex[1]);
    }

    /// <summary>
    /// 通过自定义笔刷编辑地形
    /// </summary>
    /// <param name="terrain"></param>
    public static async void ChangeHeightWithBrush(Vector3 center, float radius, float opacity, int brushIndex = 0, bool isRise = true)
    {
        int mapRadius = 0;
        int[] mapIndex = null;
        float[,] heightMap = null;
        float limit = 0;
        Terrain terrain = InitHMArg(center, radius, ref mapIndex, ref heightMap, ref mapRadius, ref limit);
        if (terrain == null) return;

        // 是否反转透明度
        if (!isRise) opacity = -opacity;

        //修改高度图
        await Task.Run(async () =>
        {
            //float[,] deltaMap = await Utility.BilinearInterp(brushDic[brushIndex], 2 * mapRadius, 2 * mapRadius);
            float[,] deltaMap = await Utility.ZoomBilinearInterpAsync(brushDic[brushIndex], 2 * mapRadius, 2 * mapRadius);

            for (int i = 0; i < 2 * mapRadius; i++)
            {
                for (int j = 0; j < 2 * mapRadius; j++)
                {
                    heightMap[i, j] += deltaMap[i, j] * deltaHeight * opacity;
                }
            }
        });

        // 重新设置高度图
        SetHeightMap(terrain, heightMap, mapIndex[0], mapIndex[1]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="center"></param>
    /// <param name="radius"></param>
    /// <param name="dev"></param>
    /// <param name="level"></param>
    public static void Smooth(Vector3 center, float radius, float dev, int level = 1)
    {
        center.x -= terrainSize.x / (heightMapRes - 1) * level;
        center.z -= terrainSize.z / (heightMapRes - 1) * level;
        radius += terrainSize.x / (heightMapRes - 1) * level;
        int mapRadius = 0;
        int[] mapIndex = null;
        float[,] heightMap = null;
        float limit = 0;
        Terrain terrain = InitHMArg(center, radius, ref mapIndex, ref heightMap, ref mapRadius, ref limit);
        if (terrain == null) return;

        Utility.GaussianBlur(heightMap, dev, level);
        SetHeightMap(terrain, heightMap, mapIndex[0], mapIndex[1]);
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
    /// 设置Terrain的HeightMap
    /// 有不只一块地形的场景不要直接调用terrainData.SetHeights
    /// 这个方法会解决跨多块地形的问题
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="heights">HeightMap</param>
    /// <param name="xBase">X起点</param>
    /// <param name="yBase">Y起点</param>
    public static void SetHeightMap(Terrain terrain, float[,] heights, int xBase = 0, int yBase = 0)
    {
        TerrainData terrainData = terrain.terrainData;
        int length_1 = heights.GetLength(1);
        int length_0 = heights.GetLength(0);

        int differX = xBase + length_1 - (terrainData.heightmapResolution - 1);
        int differY = yBase + length_0 - (terrainData.heightmapResolution - 1);

        if (differX <= 0 && differY <= 0) // 无溢出
        {
            terrain.terrainData.SetHeights(xBase, yBase, heights);
        }
        else if (differX > 0 && differY <= 0) // 右溢出
        {
            terrain.terrainData.SetHeights(xBase, yBase, heights.GetPart(0, 0, length_0, length_1 - differX + 1));  // 最后的 +1是为了和右边的地图拼接
            terrain.Right()?.terrainData.SetHeights(0, yBase, heights.GetPart(0, length_1 - differX, length_0, differX));
        }
        else if (differX <= 0 && differY > 0) // 上溢出
        {
            terrain.terrainData.SetHeights(xBase, yBase, heights.GetPart(0, 0, length_0 - differY + 1, length_1));  // 最后的 +1是为了和上边的地图拼接
            terrain.Up()?.terrainData.SetHeights(xBase, 0, heights.GetPart(length_0 - differY, 0, differY, length_1));
        }
        else  // 右上均溢出
        {
            terrain.terrainData.SetHeights(xBase, yBase, heights.GetPart(0, 0, length_0 - differY + 1, length_1 - differX + 1));  // 最后的 +1是为了和上边及右边的地图拼接
            terrain.Right()?.terrainData.SetHeights(0, yBase, heights.GetPart(0, length_1 - differX, length_0 - differY + 1, differX));
            terrain.Up()?.terrainData.SetHeights(xBase, 0, heights.GetPart(length_0 - differY, 0, differY, length_1 - differX + 1));
            terrain.Up()?.Right().terrainData.SetHeights(0, 0, heights.GetPart(length_0 - differY, length_1 - differX, differY, differX));
        }
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

    #endregion

    #region 树木

    /// <summary>
    /// 初始化树木原型组
    /// </summary>
    private static void InitTreePrototype()
    {
        GameObject[] objs = Resources.LoadAll<GameObject>("Terrain/Trees");
        TreePrototype[] trees = new TreePrototype[objs.Length];
        for (int i = 0, length = objs.Length; i < length; i++)
        {
            trees[i] = new TreePrototype();
            trees[i].prefab = objs[i];
        }
        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.treePrototypes = terrains[i].terrainData.treePrototypes.Concat(trees).ToArray();
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

    /// <summary>
    /// 移除地形上的树
    /// </summary>
    /// <param name="terrain">目标地形</param>
    /// <param name="center">中心点</param>
    /// <param name="radius">半径</param>
    /// <param name="index">树模板的索引</param>
    public static void RemoveTree(Terrain terrain, Vector3 center, float radius, int index)
    {
        center -= terrain.GetPosition();     // 转为相对位置
        Vector2 v2 = new Vector2(center.x, center.z);
        v2.x /= Terrain.activeTerrain.terrainData.size.x;
        v2.y /= Terrain.activeTerrain.terrainData.size.z;

        terrain.Invoke("RemoveTrees", v2, radius / Terrain.activeTerrain.terrainData.size.x, index);
    }

    #endregion

    #region 细节纹理 草

    /// <summary>
    /// 初始化细节原型组
    /// </summary>
    private static void InitDetailPrototype()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Terrain/Details");
        DetailPrototype[] details = new DetailPrototype[textures.Length];

        for (int i = 0, length = details.Length; i < length; i++)
        {
            details[i] = new DetailPrototype();
            details[i].prototypeTexture = textures[i];
            details[i].minWidth = 1;
            details[i].maxWidth = 2;
            details[i].maxHeight = 1;
            details[i].maxHeight = 2;
            details[i].noiseSpread = 0.1f;
            details[i].healthyColor = Color.green;
            details[i].dryColor = Color.yellow;
            details[i].renderMode = DetailRenderMode.GrassBillboard;
        }

        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.detailPrototypes = terrains[i].terrainData.detailPrototypes.Concat(details).ToArray();
        }
    }

    /// <summary>
    /// 返回Terrain上某一点的DetialMap索引。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <returns>该点在DetialMap中的位置索引</returns>
    private static int[] GetDetialMapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.detailWidth);
        int z = (int)((point.z - terrain.GetPosition().z) / length * tData.detailHeight);

        return new int[2] { x, z };
    }

    /// <summary>
    /// 添加细节
    /// </summary>
    /// <param name="terrain">目标地形</param>
    /// <param name="center">目标中心点</param>
    /// <param name="radius">半径</param>
    /// <param name="layer">层级</param>
    public static void AddDetial(Terrain terrain, Vector3 center, float radius, int layer)
    {
        SetDetail(terrain, center, radius, layer, 2);
    }

    /// <summary>
    /// 移除细节
    /// </summary>
    /// <param name="terrain">目标地形</param>
    /// <param name="center">目标中心点</param>
    /// <param name="radius">半径</param>
    /// <param name="layer">层级</param>
    public static void RemoveDetial(Terrain terrain, Vector3 point, float radius, int layer)
    {
        SetDetail(terrain, point, radius, layer, 0);
    }

    /// <summary>
    /// 修改细节
    /// </summary>
    public static void SetDetail(Terrain terrain, Vector3 point, float radius, int layer, int count)
    {
        TerrainData terrainData = terrain.terrainData;

        // 将位置转为细节图索引，半径转为索引半径
        int[] index = GetDetialMapIndex(terrain, point);
        int mapRadius = (int)(radius / terrainData.size.x * terrainData.detailResolution);

        int[,] map = terrainData.GetDetailLayer(index[0] - mapRadius, index[1] - mapRadius, 2 * mapRadius, 2 * mapRadius, layer);

        for (int i = 0, length_0 = map.GetLength(0); i < length_0; i++)
        {
            for (int j = 0, length_1 = map.GetLength(1); j < length_1; j++)
            {
                // 限定圆
                if ((i - mapRadius) * (i - mapRadius) + (j - mapRadius) * (j - mapRadius) > mapRadius * mapRadius)
                    continue;
                map[i, j] = count;
            }
        }

        // 设置细节图层
        terrainData.SetDetailLayer(index[0] - mapRadius, index[1] - mapRadius, layer, map);
    }


    #endregion

    #region 贴图

    /// <summary>
    /// 初始化贴图原型
    /// </summary>
    private static void InitTextures()
    {
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Terrain/Textures");
        SplatPrototype[] splats = new SplatPrototype[textures.Length / 2];

        for (int i = 0, length = splats.Length; i < length; i++)
        {
            SplatPrototype splat = new SplatPrototype
            {
                texture = textures[2 * i],
                normalMap = textures[2 * i + 1]
            };
            splats[i] = splat;
        }

        Terrain[] terrains = Terrain.activeTerrains;
        for (int i = 0, length = terrains.Length; i < length; i++)
        {
            terrains[i].terrainData.splatPrototypes = terrains[i].terrainData.splatPrototypes.Concat(splats).ToArray();
        }
    }

    /// <summary>
    /// 返回Terrain上某一点的AlphalMap索引。
    /// </summary>
    /// <param name="terrain">Terrain</param>
    /// <param name="point">Terrain上的某点</param>
    /// <returns>该点在DetialMap中的位置索引</returns>
    private static int[] GetAlphaMapIndex(Terrain terrain, Vector3 point)
    {
        TerrainData tData = terrain.terrainData;
        float width = tData.size.x;
        float length = tData.size.z;

        // 根据相对位置计算索引
        int x = (int)((point.x - terrain.GetPosition().x) / width * tData.alphamapWidth);
        int z = (int)((point.z - terrain.GetPosition().z) / length * tData.alphamapHeight);

        return new int[2] { x, z };
    }

    /// <summary>
    /// 设置贴图
    /// </summary>
    /// <param name="point"></param>
    /// <param name="index"></param>
    public static void SetTexture(Vector3 point, int index)
    {
        Terrain terrain = Utility.SendRayDown(point, LayerMask.GetMask("Terrain")).collider?.GetComponent<Terrain>();

        if (terrain != null)
        {
            int[] mapIndex = GetAlphaMapIndex(terrain, point);
            float[,,] map = terrain.terrainData.GetAlphamaps(mapIndex[0], mapIndex[1], 1, 1);
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    map[i, j, index] = 0.5f;
                    map[i, j, 0] = 0.5f;
                }
            }
            terrain.terrainData.SetAlphamaps(mapIndex[0], mapIndex[1], map);
        }
    }

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